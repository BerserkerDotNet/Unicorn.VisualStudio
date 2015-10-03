using System;
using System.ComponentModel;
using System.Windows.Input;
using Unicorn.VS.Types;

namespace Unicorn.VS.Models
{
    public class UnicornSettings : BaseViewModel, IDataErrorInfo
    {
        private string _endPoint;
        private bool _allowMultipleConfigurations;

        public UnicornSettings()
        {
            Save = new DelagateCommand(ExecuteSave);
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrEmpty(Error))
                Close(true);
        }

        public string EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (value == _endPoint) return;
                _endPoint = value;
                OnPropertyChanged();
            }
        }

        public bool AllowMultipleConfigurations
        {
            get { return _allowMultipleConfigurations; }
            set
            {
                if (value == _allowMultipleConfigurations) return;
                _allowMultipleConfigurations = value;
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "EndPoint")
                    result = ValidateEndPoint();

                Error = result;
                return result;
            }
        }

        private string ValidateEndPoint()
        {
            if (string.IsNullOrEmpty(EndPoint))
                return "EndPoint is required.";

            return null;
        }

        public string Error { get; private set; }

        public Action<bool> Close { get; set; }

        public ICommand Save { get; set; }
    }
}