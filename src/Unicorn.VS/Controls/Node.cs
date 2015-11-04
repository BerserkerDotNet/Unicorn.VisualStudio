using System;
using Unicorn.VS.Models;
using Unicorn.VS.ViewModels;

namespace Unicorn.VS.Controls
{
    public class Node : BaseViewModel, IEquatable<Node>
    {
        private readonly string _title;
        private bool _isSelected;

        public Node(string title)
        {
            _title = title;
        }

        public string Title => _title;

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

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_title, other._title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return _title?.GetHashCode() ?? 0;
        }
    }
}