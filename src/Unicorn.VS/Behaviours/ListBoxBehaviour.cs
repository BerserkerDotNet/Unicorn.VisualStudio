using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unicorn.VS.Models;
using Unicorn.VS.Views;

namespace Unicorn.VS.Behaviours
{
    public static class ListBoxBehaviour
    {
        public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy", typeof(bool),typeof(ListBoxBehaviour),new UIPropertyMetadata(AutoCopyChanged));

        public static bool GetAutoCopy(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoCopyProperty);
        }

        public static void SetAutoCopy(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoCopyProperty, value);
        }

        private static void AutoCopyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            if (listBox == null || !(bool)e.NewValue)
                return;

            ExecutedRoutedEventHandler handler = (s, a) =>
            {
                if (listBox.SelectedItem == null)
                    return;
                Clipboard.SetText(((StatusReport) listBox.SelectedValue).Message);
            };
            var command = new RoutedCommand("Copy", typeof(ListBox));
            command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
            listBox.CommandBindings.Add(new CommandBinding(command, handler));
        }
    }
}
