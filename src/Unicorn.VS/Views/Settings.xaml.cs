using Unicorn.VS.Data;
using Unicorn.VS.ViewModels;
using Unicorn.VS.ViewModels.Interfaces;

namespace Unicorn.VS.Views
{
    public partial class Settings 
    {
        public Settings()
        {
            InitializeComponent();
            var settings = new UnicornSettingsViewModel(SettingsHelper.GetSettings());
            settings.Close = r =>
            {
                DialogResult = r;
                Close();
            };

            DataContext = settings;
        }
    }
}
