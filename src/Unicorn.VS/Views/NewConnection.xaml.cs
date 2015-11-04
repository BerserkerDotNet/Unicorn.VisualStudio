using Unicorn.VS.Models;
using Unicorn.VS.ViewModels;

namespace Unicorn.VS.Views
{
    public partial class NewConnection
    {
        public NewConnection()
            :this(new UnicornConnection())
        {

        }

        public NewConnection(UnicornConnection data)
        {
            InitializeComponent();
            var unicornConnectionViewModel = new UnicornConnectionViewModel(data ?? new UnicornConnection());
            unicornConnectionViewModel.Close = r =>
            {
                DialogResult = r;
                Close();
            };
            DataContext = unicornConnectionViewModel;

        }
    }
}
