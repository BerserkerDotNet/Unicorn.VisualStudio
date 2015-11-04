using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unicorn.VS.Data;
using Unicorn.VS.Helpers;
using Unicorn.VS.Models;
using System.Net.Http;
using System.IO;
using Unicorn.VS.Types;
using Unicorn.VS.Types.Logging;
using Unicorn.VS.ViewModels;
using Unicorn.VS.ViewModels.Interfaces;

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