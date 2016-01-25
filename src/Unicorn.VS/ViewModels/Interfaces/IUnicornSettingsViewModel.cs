using System;
using System.Windows.Input;

namespace Unicorn.VS.ViewModels.Interfaces
{
    public interface IUnicornSettingsViewModel
    {
        string EndPoint { get; set; }
        bool CheckForConfigurationHealth { get; set; }
        bool AllowMultipleConfigurations { get; set; }
        ICommand Save { get; set; }
        Action<bool> Close { get; set; }
    }
}