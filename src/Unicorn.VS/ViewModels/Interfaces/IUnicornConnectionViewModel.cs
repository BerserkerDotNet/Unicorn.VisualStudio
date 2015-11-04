using System;
using System.Windows.Input;
using Unicorn.VS.Models;

namespace Unicorn.VS.ViewModels.Interfaces
{
    public interface IUnicornConnectionViewModel
    {
        bool ShowProgressBar { get; set; }
        ICommand TestConnection { get; set; }
        ICommand Save { get; set; }
        ICommand Install { get; set; }
        ICommand DownloadPackage { get; set; }
        Action<bool> Close { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        string ServerUrl { get; set; }
        string Token { get; set; }
    }
}