using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unicorn.VS.Annotations;

namespace Unicorn.VS.Models
{
    public class UnicornData : INotifyPropertyChanged
    {
        private ObservableCollection<string> _configurations;
        private ObservableCollection<StatusReport> _statusReports;
        private ObservableCollection<UnicornConnection> _connections;
        private int _progress;
        private bool _isIndetermine;

        public UnicornData(IEnumerable<string> configurations, IEnumerable<UnicornConnection> connections)
        {
            _configurations = new ObservableCollection<string>(configurations);
            _connections = new ObservableCollection<UnicornConnection>(connections);
            StatusReports = new ObservableCollection<StatusReport>();
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}