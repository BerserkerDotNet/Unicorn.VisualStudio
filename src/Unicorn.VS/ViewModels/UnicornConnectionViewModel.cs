using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell.Interop;
using Unicorn.VS.Data;
using Unicorn.VS.Helpers;
using Unicorn.VS.Models;
using Unicorn.VS.Types;
using Unicorn.VS.Types.UnicornCommands;
using Unicorn.VS.ViewModels.Interfaces;

namespace Unicorn.VS.ViewModels
{
    public class UnicornConnectionViewModel : BaseViewModel, IDataErrorInfo, IUnicornConnectionViewModel
    {
        private readonly UnicornConnection _connection;
        private bool _showProgressBar;

        public UnicornConnectionViewModel(UnicornConnection connection)
        {
            _connection = connection;
            TestConnection = new Command(ExecuteTestConnection, () => !ShowProgressBar);
            Save = new Command(ExecuteSave, () => !ShowProgressBar);
            Install = new Command(ExecuteInstall, () => !ShowProgressBar);
            DownloadPackage = new Command(ExecuteDownloadPackage, () => !ShowProgressBar);
        }

        public string Id { get; set; }

        public string Name
        {
            get { return _connection.Name; }
            set
            {
                _connection.Name = value;
                OnPropertyChanged();
            }
        }

        public string ServerUrl
        {
            get { return _connection.ServerUrl; }
            set
            {
                _connection.ServerUrl = value;
                OnPropertyChanged();
            }
        }

        public string Token
        {
            get { return _connection.Token; }
            set
            {
                _connection.Token = value;
                OnPropertyChanged();
            }
        }

        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set
            {
                _showProgressBar = value;
                OnPropertyChanged();
            }
        }

        public ICommand TestConnection { get; set; }

        public ICommand Save { get; set; }

        public ICommand Install { get; set; }

        public ICommand DownloadPackage { get; set; }

        public Action<bool> Close { get; set; }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(Name))
                    result = ValidateName();

                if (columnName == nameof(ServerUrl))
                    result = ValidateServerUrl();

                Error = result;
                return result;
            }
        }

        private string ValidateServerUrl()
        {
            if (string.IsNullOrEmpty(ServerUrl))
                return "Server Url is required.";
            Uri uri;
            if (!Uri.TryCreate(ServerUrl, UriKind.Absolute, out uri))
                return ServerUrl + " - is invalid URL. Url must be absolute. For example http://localsitecore";
            return null;
        }

        private string ValidateName()
        {
            if (string.IsNullOrEmpty(Name))
                return "Name is required.";
            if (SettingsHelper.IsConnectionExists(Name))
                return $"Connection with name \"{Name}\" already exists";
            return null;
        }

        public string Error { get; private set; }

        private async void ExecuteTestConnection()
        {
            try
            {
                ShowProgressBar = true;
                await Handshake();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ooops..." + ex.Message, "Connection test", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            finally
            {
                ShowProgressBar = false;
            }
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrEmpty(Error))
            {
                SettingsHelper.SaveConnection(_connection);
                Close(true);
            }
            
        }

        private async void ExecuteInstall()
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                try
                {
                    if (folderBrowser.ShowDialog() != DialogResult.OK)
                        return;
                    ShowProgressBar = true;
                    var binPath = Path.Combine(folderBrowser.SelectedPath, "bin");
                    var configPath = Path.Combine(folderBrowser.SelectedPath, "App_Config\\Include");
                    if (!Directory.Exists(binPath))
                        Directory.CreateDirectory(binPath);

                    var unicornPath = Path.Combine(binPath, "Unicorn.dll");
                    if (File.Exists(unicornPath))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(unicornPath);
                        var isUnicornRemoteIntegrated = VersionHelper.IsSupportedUnicornVersion(versionInfo.FileVersion);
                        if (isUnicornRemoteIntegrated)
                        {
                            MessageBox.Show(
                                $"Unicorn {versionInfo.FileVersion} supports Remote out of the box, no need to install external library.",
                                "Install canceled", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            return;
                        }
                    }

                    using (var wStream = File.OpenWrite(Path.Combine(binPath, "Unicorn.Remote.dll")))
                    {
                        var unicornRemote = Resources.Unicorn_Remote;
                        await wStream.WriteAsync(unicornRemote, 0, unicornRemote.Length);
                    }

                    if (!Directory.Exists(configPath))
                        Directory.CreateDirectory(configPath);

                    using (var wStream = File.OpenWrite(Path.Combine(binPath, Path.Combine(configPath, "Unicorn.Remote.config"))))
                    {
                        var unicornRemoteConfig = Encoding.UTF8.GetBytes(Resources.Unicorn_Remote_Config);
                        await wStream.WriteAsync(unicornRemoteConfig, 0, unicornRemoteConfig.Length);
                    }
                    await Handshake();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ooops... " + ex.Message, "Installing Unicorn Remote", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    ShowProgressBar = false;
                }
            }
        }

        private async Task Handshake()
        {
            _connection.IsLegacy = await UnicornCommandsManager.Execute(new IsLegacyCommand(_connection));
            var isSuccess = await UnicornCommandsManager.Execute(new HandshakeCommand(_connection, CancellationToken.None));
            if (isSuccess)
                MessageBox.Show("All good!", "Connection test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Ooops...Something went wrong.", "Connection test", MessageBoxButtons.OK,  MessageBoxIcon.Exclamation);
        }

        private void ExecuteDownloadPackage()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                try
                {
                    ShowProgressBar = true;
                    saveFileDialog.FileName = $"UnicornRemote-{VersionHelper.SupportedClientVersion}.zip";
                    saveFileDialog.DefaultExt = ".zip";
                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;
                    var bytes = (byte[]) Resources.ResourceManager.GetObject("UnicornRemote_1_1");
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }

                    Process.Start("explorer", Path.GetDirectoryName(saveFileDialog.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ooops... " + ex.Message, "Downloading Unicorn Remote package", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    ShowProgressBar = false;
                }
            }
        }
    }
}
