using System.Collections.ObjectModel;
using System.Windows.Input;
using Unicorn.VS.Models;

namespace Unicorn.VS.ViewModels.Interfaces
{
    public interface IControlPanelViewModel
    {
        ObservableCollection<string> Configurations { get; set; }
        ObservableCollection<StatusReport> StatusReports { get; set; }
        ObservableCollection<UnicornConnection> Connections { get; set; }

        int Progress { get; set; }
        bool IsIndetermine { get; set; }
        bool IsLoadingStarted { get; set; }
        UnicornConnection SelectedConnection { get; set; }
        int SelectedConnectionIndex { get; set; }
        string SelectedConfigurations { get; set; }
        int SelectedConfigurationIndex { get; set; }

        ICommand CreateNewConnection { get; }
        ICommand RemoveSelectedConnection { get; }
        ICommand EditSelectedConnection { get; }
        ICommand ShowSettings { get; }
        ICommand CancelCurrentJob { get; }

        ICommand Synchronize { get; }
        ICommand Reserialize { get; }
        bool AllowMultipleConfigurations { get; }
    }
}
