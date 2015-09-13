using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using Unicorn.Configuration;
using Unicorn.ControlPanel;
using Unicorn.Data;
using Unicorn.Loader;
using Unicorn.Logging;
using Unicorn.Predicates;
using Unicorn.Remote.Logging;
using Unicorn.Serialization;

namespace Unicorn.Remote.Processor
{
    public class UnicornRemotePipelineProcessor : HttpRequestProcessor
    {
        private static ConcurrentDictionary<string, ProgressReporter> _currentJobs =new ConcurrentDictionary<string, ProgressReporter>();
        private static string _currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private readonly string _activationUrl;

        public UnicornRemotePipelineProcessor(string activationUrl)
        {
            _activationUrl = activationUrl;
        }

        public override void Process(HttpRequestArgs args)
        {
            if (string.IsNullOrWhiteSpace(_activationUrl))
                return;

            if (args.Context.Request.RawUrl.StartsWith(_activationUrl, StringComparison.OrdinalIgnoreCase))
            {
                ProcessRequest(args.Context);
                args.Context.Response.End();
            }
        }

        private void ProcessRequest(HttpContext context)
        {
            context.Server.ScriptTimeout = 86400;

            if (!IsAuthorized)
            {
                context.Response.StatusCode = 401;
                context.Response.StatusDescription = "Not Authorized";
            }
            else
            {
                using (new SecurityDisabler())
                {
                    var verb = context.Request.QueryString["verb"].ToLower();
                    switch (verb)
                    {
                        case "sync":
                            Process(context, ProcessSync);
                            break;
                        case "reserialize":
                            Process(context, ProcessReserialize);
                            break;
                        case "handshake":
                            SetSuccessResponse(context);
                            break;
                        case "report":
                            ProcessReport(context);
                            break;
                        case "finish":
                            CleanUp(context);
                            break;
                        case "config":
                            ProcessConfiguration(context);
                            break;
                        default:
                            SetResponse(context, 404, "Not Found");
                            break;
                    }
                }
            }
        }

        private void CleanUp(HttpContext context)
        {
            var jobId = context.Request.QueryString["id"];
            if(string.IsNullOrEmpty(jobId))
                return;

            ProgressReporter reporter;
            _currentJobs.TryRemove(jobId, out reporter);
            SetSuccessResponse(context);
        }

        private void ProcessReport(HttpContext context)
        {
            var jobId = context.Request.QueryString["id"];
            var timeStamp = context.Request.QueryString["lastcheck"];

            ProgressReporter reporter;
            if (!_currentJobs.TryGetValue(jobId, out reporter) || reporter == null)
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "Not Found";
                return;
            }

            var reports = string.IsNullOrEmpty(timeStamp)
                ? reporter.GetReport()
                : reporter.GetReport(JsonConvert.DeserializeObject<DateTime>("\""+Uri.UnescapeDataString(timeStamp)+"\""));

            SetJsonResponse(context, JsonConvert.SerializeObject(reports));

        }

        private void Process(HttpContext context, Action<ProgressReporter> action)
        {
            var jobId = context.Request.QueryString["id"];
            if (string.IsNullOrEmpty(jobId))
                return;

            var progress = new ProgressReporter();
            _currentJobs.TryAdd(jobId, progress);

            using (new SecurityDisabler())
            {
                using (new ItemFilterDisabler())
                {
                    action(progress);
                }
            }
            SetJsonResponse(context, JsonConvert.SerializeObject(new JobDetails
            {
                Id = jobId,
                Created = DateTime.Now,
            }));
        }

        private void ProcessSync(ProgressReporter progress)
        {
            foreach (var configuration in ResolveConfigurations())
            {
                using (new LoggingContext(progress, configuration))
                {
                    try
                    {
                        progress.ReportSimple("Control Panel Sync: Processing Unicorn configuration " +
                                              configuration.Name, MessageLevel.Info);

                        var pathResolver = configuration.Resolve<PredicateRootPathResolver>();
                        var retryer = configuration.Resolve<IDeserializeFailureRetryer>();
                        var consistencyChecker = configuration.Resolve<IConsistencyChecker>();
                        var loader = configuration.Resolve<SerializationLoader>();
                        var roots = pathResolver.GetRootSerializedItems();
                        int index = 1;
                        loader.LoadAll(roots, retryer, consistencyChecker, item =>
                        {
                            progress.ReportProgress((int)((index / (double)roots.Length) * 100));
                            index++;
                        });
                        progress.ReportSimple("Control Panel Sync: Completed syncing Unicorn configuration " +
                                              configuration.Name, MessageLevel.Info);
                    }
                    catch (Exception ex)
                    {
                        progress.ReportError(ex);
                        break;
                    }
                }
            }
        }

        private void ProcessReserialize(ProgressReporter progress)
        {
            foreach (var configuration in ResolveConfigurations())
            {
                var logger = configuration.Resolve<ILogger>();
                using (new LoggingContext(progress, configuration))
                {
                    try
                    {
                        logger.Info("Control Panel Reserialize: Processing Unicorn configuration " + configuration.Name);
                        var predicate = configuration.Resolve<IPredicate>();
                        var serializationProvider = configuration.Resolve<ISerializationProvider>();
                        var roots = configuration.Resolve<PredicateRootPathResolver>().GetRootSourceItems();
                        int index = 1;
                        foreach (var root in roots)
                        {
                            var rootReference = serializationProvider.GetReference(root);
                            if (rootReference != null)
                            {
                                logger.Warn("[D] existing serialized items under {0}".FormatWith(rootReference.DisplayIdentifier));
                                rootReference.Delete();
                            }

                            logger.Info("[U] Serializing included items under root {0}".FormatWith(root.DisplayIdentifier));
                            Serialize(root, predicate, serializationProvider, logger);
                            progress.ReportProgress((int)((index / (double)roots.Length) * 100));
                            index++;
                        }
                        logger.Info("Control Panel Reserialize: Finished reserializing Unicorn configuration " + configuration.Name);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }
                }
            }
        }

        private IConfiguration[] ResolveConfigurations()
        {
            var config = HttpContext.Current.Request.QueryString["configuration"];
            var configurations = UnicornConfigurationManager.Configurations;
            if (string.IsNullOrWhiteSpace(config)) return configurations;

            var targetConfiguration = configurations.FirstOrDefault(x => x.Name == config);

            if (targetConfiguration == null) throw new ArgumentException("Configuration requested was not defined.");

            return new[] { targetConfiguration };
        }

        private void ProcessConfiguration(HttpContext context)
        {
            var configs = UnicornConfigurationManager.Configurations.Select(c => c.Name);
            SetJsonResponse(context, JsonConvert.SerializeObject(configs));
        }

        private static void SetJsonResponse(HttpContext context, string data)
        {
            SetSuccessResponse(context);
            context.Response.AddHeader("Content-Type", "application/json");
            context.Response.Output.Write(data);
        } 
        
        private static void SetSuccessResponse(HttpContext context)
        {
            SetResponse(context, 200, "OK");
        }

        private static void SetResponse(HttpContext context, int statusCode, string description)
        {
            context.Response.StatusCode = statusCode;
            context.Response.StatusDescription = description;
            context.Response.AddHeader("X-Remote-Version", _currentVersion);
        }

        private void Serialize(ISourceItem root, IPredicate predicate, ISerializationProvider serializationProvider, ILogger logger)
        {
            var predicateResult = predicate.Includes(root);
            if (predicateResult.IsIncluded)
            {
                serializationProvider.SerializeItem(root);

                foreach (var child in root.Children)
                {
                    Serialize(child, predicate, serializationProvider, logger);
                }
            }
            else
            {
                logger.Warn("[S] {0} because {1}".FormatWith(root.DisplayIdentifier, predicateResult.Justification));
            }
        }
        protected virtual bool IsAuthorized
        {
            get
            {
                var user = AuthenticationManager.GetActiveUser();
                if (user.IsAdministrator)
                    return true;

                var authToken = HttpContext.Current.Request.Headers["Authenticate"];
                var correctAuthToken = ConfigurationManager.AppSettings["DeploymentToolAuthToken"];

                if (!string.IsNullOrWhiteSpace(correctAuthToken) && !string.IsNullOrWhiteSpace(authToken) &&
                    authToken.Equals(correctAuthToken, StringComparison.Ordinal))
                    return true;

                // if dynamic debug compilation is enabled, you can use it without auth (eg local dev)
                if (HttpContext.Current.IsDebuggingEnabled)
                    return true;

                return false;
            }
        }

    }
}