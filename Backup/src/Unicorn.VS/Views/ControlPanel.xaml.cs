using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Unicorn.VS.Data;
using Unicorn.VS.Helpers;
using Unicorn.VS.Models;
using Unicorn.VS.Types;
using Brush = System.Drawing.Brush;

namespace Unicorn.VS.Views
{
    public partial class ControlPanel
    {
        private readonly UnicornData _dataContext;
        private UnicornConnection _selectedConnection;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ControlPanel()
        {
            InitializeComponent();
            _dataContext = new UnicornData(new[] {HttpHelper.DefaultConfiguration}, SettingsHelper.GetAllConnections());
            DataContext = _dataContext;
            //StartLoading();
            //Task.Run(async () =>
            //{
            //    int x = 0;
            //    while (x < 20)
            //    {
            //        x++;
            //        var x1 = x;
            //        Dispatcher.Invoke(
            //            () =>
            //                _dataContext.StatusReports.Add(new StatusReport("test" + x1, MessageLevel.Info,
            //                    OperationType.Added)));
            //        await Task.Delay(100);
            //    }
            //    Dispatcher.Invoke(
            //        StopLoading);
            //});
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Info, OperationType.Added));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Error, OperationType.Deleted));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Info, OperationType.Moved));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Warning, OperationType.Renamed));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Info, OperationType.TemplateChanged));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Debug, OperationType.Updated));
            //_dataContext.StatusReports.Add(new StatusReport("test", MessageLevel.Info, OperationType.None));
            if (_dataContext.Connections.Any())
                sitecoreServer.SelectedIndex = 0;
        }

        private void CmdNewConnection_OnClick(object sender, RoutedEventArgs e)
        {
            ShowConnectionDialog();
        }

        private void CmdRemoveConnection_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = sitecoreServer.SelectedItem as UnicornConnection;
            if (selectedItem == null)
                return;

            var result = MessageBox.Show(string.Format("Delete connection '{0}'?", selectedItem.Name), "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            SettingsHelper.DeleteConnection(selectedItem.Id);
            _dataContext.Connections.Remove(selectedItem);
            if (_dataContext.Connections.Any())
                sitecoreServer.SelectedIndex = 0;
        }

        private void CmdEditConnection_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            if (button == null)
                return;

            var connectionId = button.Tag;
            if (connectionId == null)
                return;

            var info = _dataContext.Connections.First(c => c.Id == connectionId.ToString());
            ShowConnectionDialog(info);
        }

        private async void CmdSync_OnClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await ExecuteJob(HttpHelper.StartSyncCommand, "Synchronization", _cancellationTokenSource.Token);
        }

        private async void CmdReserialize_OnClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var result = MessageBox.Show(string.Format("Are you sure you want to re-serialize database for {0}?", selectedConfig.Text),
                "Reserialization", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                await ExecuteJob(HttpHelper.StartReserializeCommand, "Reserialization", _cancellationTokenSource.Token);
        }

        private void CmdCancel_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to cancel current operation?", "Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                _cancellationTokenSource.Cancel();
        }

        private async void SitecoreServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count == 0)
                return;
            _selectedConnection = e.AddedItems[0] as UnicornConnection;

            await RefreshConfiguration();
        }

        private async Task ExecuteJob(string command, string jobName, CancellationToken ct)
        {
            _dataContext.StatusReports.Clear();
            StartLoading(false);
            try
            {
                var id = Guid.NewGuid().ToString("N");
                var hasFinished = false;
#pragma warning disable 4014
                StartJob(command, id, ct).ContinueWith(_ => hasFinished = true, ct);
#pragma warning restore 4014
                while (true)
                {
                    await Task.Delay(1000, ct);
                    if (hasFinished)
                    {
                        await RefreshStatus(id, ct);
                        await FinishSync(id, ct);
                        Dispatcher.Invoke(StopLoading);
                        break;
                    }
                    await RefreshStatus(id, ct);
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error while performin {0} configuration for '{1}': {2}",jobName, _selectedConnection, ex.Message);
                MessageBox.Show(msg, jobName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StopLoading();
            }
        }

        private async Task RefreshStatus(string id, CancellationToken ct)
        {
            var lastMessage = _dataContext.StatusReports.LastOrDefault();
            var lastCheckData = lastMessage == null ? string.Empty : JsonConvert.SerializeObject(lastMessage.MessageTime.AddMilliseconds(10)).Trim('\"');
            var report = await _selectedConnection.Get(HttpHelper.ReportCommand)
                .WithId(id)
                .WithConfiguration(selectedConfig.Text)
                .WithKey("lastcheck", lastCheckData)
                .Execute<Report>(ct);

            if (report == null)
                return;
            report.StatusReports.ToList().ForEach(r => _dataContext.StatusReports.Add(r));
            _dataContext.Progress = report.Progress;
        }

        private async Task FinishSync(string id, CancellationToken ct)
        {
            await _selectedConnection.Get(HttpHelper.FinishSyncCommand)
                .WithId(id)
                .WithConfiguration(selectedConfig.Text)
                .Execute<object>(ct);
        }

        private async Task StartJob(string command, string jobId, CancellationToken ct)
        {
            await _selectedConnection.Get(command)
                .WithId(jobId)
                .WithConfiguration(selectedConfig.Text)
                .Execute<JobDetails>(ct);
        }

        private async Task RefreshConfiguration()
        {
            StartLoading();
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var configs = await _selectedConnection.Get(HttpHelper.ConfigCommand)
                    .Execute<IEnumerable<string>>(_cancellationTokenSource.Token);

                _dataContext.Configurations.Clear();
                _dataContext.Configurations.Add(HttpHelper.DefaultConfiguration);
                foreach (var config in configs)
                {
                    _dataContext.Configurations.Add(config);
                }
                selectedConfig.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error while retrieving configuration for '{0}': {1}", _selectedConnection.Name,
                    ex.Message);
                MessageBox.Show(msg, "Retrieve configuration", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                StopLoading();
            }
        }

        private void ShowConnectionDialog(UnicornConnection info = null)
        {
            var newConnection = new NewConnection();
            if (info != null)
                newConnection.SetData(info);
            if (newConnection.ShowDialog() == true)
            {
                var data = newConnection.DataContext as UnicornConnectionViewModel;
                var connectionViewModel = data.Connection;
                SettingsHelper.SaveConnection(connectionViewModel);
                if (_dataContext.Connections.All(c => c.Id != connectionViewModel.Id))
                    _dataContext.Connections.Add(connectionViewModel);
                sitecoreServer.SelectedItem = connectionViewModel;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableControls()
        {
            ChangeControlsState(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DisableControls()
        {
            ChangeControlsState(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeControlsState(bool isEnabled)
        {
            cmdSync.IsEnabled = isEnabled;
            cmdNewConnection.IsEnabled = isEnabled;
            cmdReserialize.IsEnabled = isEnabled;
            cmdRemoveConnection.IsEnabled = isEnabled;
            sitecoreServer.IsEnabled = isEnabled;
            cmdCancel.IsEnabled = !isEnabled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartLoading(bool isIndetermine=true)
        {
            _dataContext.Progress = 0;
            _dataContext.IsIndetermine = isIndetermine;
            loadingBlock.Text = "Loading...";
            loadingBlock.Visibility = Visibility.Visible;
            loadingBar.Visibility = Visibility.Visible;
            cmdCancel.Visibility = Visibility.Visible;
            DisableControls();

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StopLoading()
        {
            loadingBlock.Visibility = Visibility.Collapsed;
            loadingBar.Visibility = Visibility.Collapsed;
            cmdCancel.Visibility = Visibility.Collapsed;
            EnableControls();
        }
    }
}