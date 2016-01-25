using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using Unicorn.VS.Data;
using Unicorn.VS.Helpers;
using Unicorn.VS.Models;
using Unicorn.VS.Types;
using Unicorn.VS.Types.Logging;
using Unicorn.VS.Types.UnicornCommandHandlers;
using Unicorn.VS.Types.UnicornCommands;
using Unicorn.VS.ViewModels.Interfaces;
using Unicorn.VS.Views;
using Command = Unicorn.VS.Types.Command;
using Package = Microsoft.VisualStudio.Shell.Package;

namespace Unicorn.VS.ViewModels
{
    public class UnicornControlPanelViewModel : BaseViewModel, IControlPanelViewModel
    {
        private const string DefaultConfiguration = "All";
        private readonly Dispatcher _dispatcher;
        private ObservableCollection<string> _configurations;
        private ObservableCollection<StatusReport> _statusReports;
        private ObservableCollection<UnicornConnection> _connections;
        private int _progress;
        private bool _isIndetermine;
        private UnicornConnection _selectedConnection;
        private int _selectedConnectionIndex;
        private bool _isLoadingStarted;
        private string _selectedConfigurations;
        private int _selectedConfigurationIndex;
        private CancellationTokenSource _cancellationTokenSource;
        private IVsStatusbar _bar;

        public UnicornControlPanelViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _configurations = new ObservableCollection<string>(new[] { DefaultConfiguration });
            LoadConnections();
            StatusReports = new ObservableCollection<StatusReport>();
            CreateNewConnection = new Command(ExecuteCreateNewConnection);
            RemoveSelectedConnection = new Command(ExecuteRemoveSelectedConnection);
            EditSelectedConnection = new Command<string>(ExecuteEditSelectedConnection);
            ShowSettings = new Command(ExecuteShowSettings);
            CancelCurrentJob = new Command(ExecuteCancelCurrentJob);
            Synchronize = new Command(ExecuteSynchronize);
            Reserialize = new Command(ExecuteReserialize);
            SelectedConnectionIndex = 0;
            CheckForConfigurationHealth = SettingsHelper.GetSettings().CheckForConfigurationHealth;
        }

        public ObservableCollection<string> Configurations
        {
            get { return _configurations; }
            set
            {
                _configurations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StatusReport> StatusReports
        {
            get { return _statusReports; }
            set
            {
                _statusReports = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UnicornConnection> Connections
        {
            get { return _connections; }
            set
            {
                if (Equals(value, _connections)) return;
                _connections = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value == _progress) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndetermine
        {
            get { return _isIndetermine; }
            set
            {
                if (value == _isIndetermine) return;
                _isIndetermine = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingStarted
        {
            get { return _isLoadingStarted; }
            set
            {
                if (value == _isLoadingStarted) return;
                _isLoadingStarted = value;
                OnPropertyChanged();
            }
        }

        public UnicornConnection SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                if (Equals(value, _selectedConnection)) return;
                _selectedConnection = value;
                OnPropertyChanged();
                RefreshConfiguration();
            }
        }

        public int SelectedConnectionIndex
        {
            get { return _selectedConnectionIndex; }
            set
            {
                if (value == _selectedConnectionIndex) return;
                _selectedConnectionIndex = value;
                OnPropertyChanged();
            }
        }

        public string SelectedConfigurations
        {
            get { return _selectedConfigurations; }
            set
            {
                if (value == _selectedConfigurations) return;
                _selectedConfigurations = value;
                OnPropertyChanged();
                CheckConfigurationHealth();
            }
        }

        public int SelectedConfigurationIndex
        {
            get { return _selectedConfigurationIndex; }
            set
            {
                if (value == _selectedConfigurationIndex) return;
                _selectedConfigurationIndex = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateNewConnection { get; }
        public ICommand RemoveSelectedConnection { get; }
        public ICommand EditSelectedConnection { get; }
        public ICommand ShowSettings { get; }
        public ICommand CancelCurrentJob { get; }
        public ICommand Synchronize { get; }
        public ICommand Reserialize { get; }

        public bool CheckForConfigurationHealth { get; set; }
        public bool AllowMultipleConfigurations => SettingsHelper.GetSettings().AllowMultipleConfigurations;

        private void LoadConnections()
        {
            SelectedConfigurationIndex = -1;
            Connections = new ObservableCollection<UnicornConnection>(SettingsHelper.GetAllConnections());
        }

        private void ExecuteCreateNewConnection()
        {
            ShowConnectionDialog();
        }

        private void ExecuteRemoveSelectedConnection()
        {
            if (SelectedConnection == null)
                return;

            var result = MessageBox.Show($"Delete connection '{SelectedConnection.Name}'?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            SettingsHelper.DeleteConnection(SelectedConnection.Id);
            Connections.Remove(SelectedConnection);
            if (Connections.Any())
                SelectedConnectionIndex = 0;
            else
                Configurations.Clear();
        }

        private void ExecuteEditSelectedConnection(string id)
        {
            var info = Connections.First(c => c.Id == id);
            ShowConnectionDialog(info);
        }

        private void ExecuteShowSettings()
        {
            var settingsDialog = new Settings();
            settingsDialog.ShowModal();
            CheckForConfigurationHealth = SettingsHelper.GetSettings().CheckForConfigurationHealth;
            OnPropertyChanged(nameof(AllowMultipleConfigurations));
        }

        private void ExecuteCancelCurrentJob()
        {
            var result = MessageBox.Show("Are you sure to cancel current operation?", "Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                _cancellationTokenSource.Cancel();
        }

        private async void ExecuteSynchronize()
        {
            if (SelectedConnection == null)
            {
                MessageBox.Show("Please select connection.", "No connection", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            SetStatusBarText($"Synchronizing {SelectedConnection.Name}");
            await ExecuteSafe(async t =>
            {
                ResetState();
                using (var progressContext = new ProgressContext(StatusBar, "Synchronizing"))
                {
                    var legacySynchronizeCommand = new SynchronizeCommand(SelectedConnection,
                        SelectedConfigurations, t, s => _dispatcher.Invoke(() => RefreshStatus(s, progressContext)));
                    await UnicornCommandsManager.Execute(legacySynchronizeCommand).ConfigureAwait(false);
                }
            },"Synchronization completed", "Synchronization failed").ConfigureAwait(false);
            
        }

        private async void ExecuteReserialize()
        {
            if (SelectedConnection == null)
            {
                MessageBox.Show("Please select connection.", "No connection", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            await ExecuteSafe(async t =>
            {
                ResetState();
                using (var progressContext = new ProgressContext(StatusBar, "Serializing"))
                {
                    var legacySynchronizeCommand = new ReserializeCommand(SelectedConnection,
                        SelectedConfigurations, t, s => _dispatcher.Invoke(() => RefreshStatus(s, progressContext)));
                    await UnicornCommandsManager.Execute(legacySynchronizeCommand)
                        .ConfigureAwait(false);
                }
            }, $"{SelectedConnection} has been serialized", "Serialization failed").ConfigureAwait(false);
        }

        private void ShowConnectionDialog(UnicornConnection info = null)
        {
            var shouldUpdate = SelectedConnection != null;
            info = info ?? new UnicornConnection();
            var newConnection = new NewConnection(info);
            if (newConnection.ShowModal() != true)
                return;

            LoadConnections();
            if (shouldUpdate)
            {
                SelectedConnection = null;
                SelectedConnection = info;
            }
        }

        private async Task RefreshConfiguration()
        {
            if (SelectedConnection == null)
                return;

            SetStatusBarText("Connecting to Unicorn...");
            await RefreshConnectionState();
            await ExecuteSafe(async t =>
            {
                
                IsIndetermine = true;
                SelectedConfigurations = string.Empty;
                Configurations.Clear();
                Configurations.Add(DefaultConfiguration);

                var configs = await UnicornCommandsManager.Execute(new ConfigurationsCommand(SelectedConnection, t));
                foreach (var config in configs)
                {
                    Configurations.Add(config);
                }
                SelectedConfigurationIndex = 0;
            }, $"Connected to {SelectedConnection.Name}", $"{SelectedConnection.Name} failed to connect").ConfigureAwait(false);
        }

        private async Task CheckConfigurationHealth()
        {
            if (SelectedConnection == null || !CheckForConfigurationHealth)
                return;

            SetStatusBarText("Checking configuration status...");
            await RefreshConnectionState();
            await ExecuteSafe(async t =>
            {
                IsIndetermine = true;
                var healthReport = await UnicornCommandsManager.Execute(new CheckConfigurationHealthCommand(SelectedConnection, SelectedConfigurations, t));
                _statusReports.Clear();
                healthReport.ToList().ForEach(r => _statusReports.Add(r));
            }, "Done", "Failed to check configuration status");
        }

        private async Task RefreshConnectionState()
        {
            if (SelectedConnection == null)
                return;

            await ExecuteSafe(async t =>
            {
                IsIndetermine = true;
                SelectedConnection.IsLegacy = await UnicornCommandsManager.Execute(new IsLegacyCommand(SelectedConnection, t));
            }, "Connecting to Unicorn...", "Failed to read Unicorn version");
        }

        private async Task ExecuteSafe(Func<CancellationToken, Task> stuff, string statusBarOkText, string statusBarFailText)
        {
            IsLoadingStarted = true;
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await stuff(_cancellationTokenSource.Token).ConfigureAwait(false);
                SetStatusBarText(statusBarOkText);
            }
            catch (Exception ex)
            {
                SetStatusBarText(statusBarFailText);
                var msg = $"Error while executing operation on server '{_selectedConnection.Name}': {ex.Message}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingStarted = false;
            }
        }

        private void ResetState()
        {
            StatusReports.Clear();
            Progress = 0;
            IsIndetermine = false;
        }

        private void RefreshStatus(StatusReport report, ProgressContext progressContext)
        {
            try
            {
                if (report.IsProgressReport())
                {
                    Progress = int.Parse(report.Message);
                    progressContext.SetProgress((uint)Progress);
                }
                else
                    StatusReports.Add(report);
            }
            catch (Exception ex)
            {
                StatusReports.Add(StatusReport.CreateOperation("Malformated package. " + ex.Message,
                    MessageLevel.Error, OperationType.None));
            }
        }

        private IVsStatusbar StatusBar => _bar ?? (_bar = Package.GetGlobalService(typeof(SVsStatusbar)) as IVsStatusbar);

        private void SetStatusBarText(string text)
        {
            int frozen;
            StatusBar.IsFrozen(out frozen);
            if (frozen == 0)
            {
                StatusBar.SetText(text);
            }
        }

    }

    public class ProgressContext : IDisposable
    {
        private readonly IVsStatusbar _statusbar;
        private readonly string _operationText;
        uint _cookie = 0;

        public ProgressContext(IVsStatusbar statusbar, string operationText)
        {
            _statusbar = statusbar;
            _operationText = operationText;
            _statusbar.Progress(ref _cookie, 1, operationText, 0, 0);
        }

        public void SetProgress(uint progress)
        {
            _statusbar.Progress(ref _cookie, 1, $"{_operationText} {progress}%", progress, 100);
        }

        public void Dispose()
        {
            _statusbar.Progress(ref _cookie, 0, "", 0, 0);
            _statusbar.FreezeOutput(0);
            _statusbar.Clear();
        }
    }
}