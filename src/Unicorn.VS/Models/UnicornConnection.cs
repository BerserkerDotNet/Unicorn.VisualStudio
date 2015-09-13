using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Unicorn.VS.Annotations;
using Unicorn.VS.Data;

namespace Unicorn.VS.Models
{
    public class UnicornConnection : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _error;
        private bool _isUpdateRequired;
        private string _name;
        private string _serverUrl;
        private string _token;

        public UnicornConnection()
        {
            Id = Guid.NewGuid().ToString();
            ServerUrl = "http://";
        }

        public string Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string ServerUrl
        {
            get { return _serverUrl; }
            set
            {
                _serverUrl = value;
                OnPropertyChanged();
            }
        }

        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                OnPropertyChanged();
            }
        }

        public bool IsUpdateRequired
        {
            get { return _isUpdateRequired; }
            set
            {
                _isUpdateRequired = value;
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "Name")
                    result = ValidateName();

                if (columnName == "ServerUrl")
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
                return string.Format("Connection with name \"{0}\" already exists", Name);
            return null;
        }

        public string Error
        {
            get { return _error; }
            private set
            {
                _error = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}