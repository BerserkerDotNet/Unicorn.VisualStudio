using System;
using System.ComponentModel;
using System.Windows.Input;
using Unicorn.VS.Data;
using Unicorn.VS.Models;
using Unicorn.VS.Types;
using Unicorn.VS.ViewModels.Interfaces;

namespace Unicorn.VS.ViewModels
{
    public class UnicornSettingsViewModel : BaseViewModel, IDataErrorInfo, IUnicornSettingsViewModel
    {
        private UnicornSettings _settings;
        public UnicornSettingsViewModel(UnicornSettings settings)
        {
            _settings = settings;
            Save = new Command(ExecuteSave);
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrEmpty(Error))
            {
                SettingsHelper.SaveSettings(_settings);
                Close(true);
            }
        }

        public string EndPoint
        {
            get { return _settings.EndPoint; }
            set
            {
                _settings.EndPoint = value;
                OnPropertyChanged();
            }
        }

        public bool AllowMultipleConfigurations
        {
            get { return _settings.AllowMultipleConfigurations; }
            set
            {
                _settings.AllowMultipleConfigurations = value;
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