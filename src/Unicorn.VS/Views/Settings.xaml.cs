using Unicorn.VS.Models;

namespace Unicorn.VS.Views
{
    public partial class Settings 
    {
        public Settings(UnicornSettings settings)
        {
            InitializeComponent();
            settings.Close = r =>
            {
                DialogResult = r;
                Close();
            };

            DataContext = settings;
        }
    }
}
