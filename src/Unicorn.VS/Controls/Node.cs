using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unicorn.VS.Annotations;

namespace Unicorn.VS.Controls
{
    public class Node : INotifyPropertyChanged
    {
        private string _title;
        private bool _isSelected;

        public Node(string title)
        {
            Title = title;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}