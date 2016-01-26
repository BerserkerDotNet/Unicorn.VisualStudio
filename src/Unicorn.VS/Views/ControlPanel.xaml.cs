using Unicorn.VS.ViewModels;

namespace Unicorn.VS.Views
{
    public partial class ControlPanel
    {
        public ControlPanel()
        {
            InitializeComponent();
            DataContext = new UnicornControlPanelViewModel(Dispatcher);
        }
    }
}