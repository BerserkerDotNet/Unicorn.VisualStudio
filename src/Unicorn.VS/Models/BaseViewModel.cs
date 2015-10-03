using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unicorn.VS.Annotations;

namespace Unicorn.VS.Models
{
    public abstract class BaseViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}