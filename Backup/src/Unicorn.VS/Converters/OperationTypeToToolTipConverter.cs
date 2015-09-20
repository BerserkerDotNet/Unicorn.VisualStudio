using System;
using System.Globalization;
using System.Windows.Data;
using Unicorn.VS.Types;

namespace Unicorn.VS.Converters
{
    public class OperationTypeToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messageLevel = (OperationType) value;
            switch (messageLevel)
            {
                case OperationType.Added:
                    return "New version added";
                case OperationType.Updated:
                    return "Changed item";
                case OperationType.Deleted:
                    return "Item was deleted";
                case OperationType.Renamed:
                    return "Item was renamed";
                case OperationType.Moved:
                    return "Parent item has changed";
                case OperationType.TemplateChanged:
                    return "Item template was changed";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}