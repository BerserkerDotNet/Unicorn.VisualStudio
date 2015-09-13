using System;
using System.Collections.Generic;
using System.Linq;
using Unicorn.ControlPanel;
using Unicorn.Logging;

namespace Unicorn.Remote.Logging
{
    public class ProgressReporter: ILogger
    {
        private readonly List<StatusReport> _reports;
        private int _progress;

        public ProgressReporter()
        {
            _reports = new List<StatusReport>(100);
            _progress = 0;
        }

        public void ReportSimple(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.None));
        }

        public void ReportAdded(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Added));
        }

        public void ReportUpdated(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Updated));
        }

        public void ReportDeleted(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Deleted));
        }

        public void ReportTemplateChanged(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.TemplateChanged));
        }

        public void ReportRenamed(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Renamed));
        }

        public void ReportMoved(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Moved));
        }
        
        public void ReportSerialized(string text, MessageLevel level)
        {
            _reports.Add(new StatusReport(text, level, OperationType.Serialized));
        }

        private void ReportError(string exception)
        {
            _reports.Add(new StatusReport(exception, MessageLevel.Error, OperationType.None));
        }

        public void ReportError(Exception exception)
        {
            _reports.Add(new StatusReport(exception.ToString(), MessageLevel.Error, OperationType.None));
        }

        public void Info(string message)
        {
            ReportMessage(message, MessageLevel.Info);
        }

        public void Debug(string message)
        {
            ReportMessage(message, MessageLevel.Debug);
        }

        public void Warn(string message)
        {
            ReportMessage(message, MessageLevel.Warning);
        }

        public void Error(string message)
        {
            ReportError(message);
        }

        public void Error(Exception exception)
        {
            ReportError(exception);
        }

        public void ReportProgress(int progress)
        {
            _progress = progress;
        }

        public Report GetReport(DateTime lastCheck)
        {
            var status = _reports.Where(r => r.MessageTime >= lastCheck).ToList();
            return new Report
            {
                Progress = _progress,
                StatusReports = status
            };
        }

        public Report GetReport()
        {
            return new Report
            {
                Progress = _progress,
                StatusReports = _reports
            };
        }

        private void ReportMessage(string message, MessageLevel level)
        {
            const string addedTag = "[A]";
            const string updatedTag = "[U]";
            const string deletedTag = "[D]";
            const string templateChangedTag = "[T]";
            const string renamedTag = "[R]";
            const string movedTag = "[M]";
            const string serializedTag = "[S]";

            message = message.Trim();

            if (message.Contains(addedTag))
                ReportAdded(message.Replace(addedTag, "").Trim(), level);
            else if (message.Contains(updatedTag))
                ReportUpdated(message.Replace(updatedTag, "").Trim(), level);
            else if (message.Contains(deletedTag))
                ReportDeleted(message.Replace(deletedTag, "").Trim(), level);
            else if (message.Contains(templateChangedTag))
                ReportTemplateChanged(message.Replace(templateChangedTag, "").Trim(), level);
            else if (message.Contains(renamedTag))
                ReportRenamed(message.Replace(renamedTag, "").Trim(), level);
            else if (message.Contains(movedTag))
                ReportMoved(message.Replace(movedTag, "").Trim(), level); 
            else if (message.Contains(serializedTag))
                ReportSerialized(message.Replace(serializedTag, "").Trim(), level);
            else
                ReportSimple(message, level);
        }
    }
}