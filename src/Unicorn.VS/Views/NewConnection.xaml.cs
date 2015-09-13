using System;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using Unicorn.VS.Models;

namespace Unicorn.VS.Views
{
    public partial class NewConnection : DialogWindow
    {
        public NewConnection()
        {
            InitializeComponent();

            var unicornConnectionViewModel = (DataContext as UnicornConnectionViewModel);
            unicornConnectionViewModel.Close = r =>
            {
                DialogResult = r;
                Close();
            };
        }

        public void SetData(UnicornConnection data)
        {
            if (data != null)
                Title = "Update Unicorn Connection";
            (DataContext as UnicornConnectionViewModel).SetData(data);
        }
    }
}
