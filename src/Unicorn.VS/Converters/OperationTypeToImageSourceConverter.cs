using System;
using System.Globalization;
using System.Windows.Data;
using Unicorn.VS.Types;

namespace Unicorn.VS.Converters
{
    public class OperationTypeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messageLevel = (OperationType) value;
            string imageFile;
            switch (messageLevel)
            {
                case OperationType.Added:
                    imageFile = "added.png";
                    break;
                case OperationType.Updated:
                    imageFile = "edited.png";
                    break;
                case OperationType.Deleted:
                    imageFile = "deleted.png";
                    break; 
                case OperationType.Renamed:
                    imageFile = "renamed.png";
                    break;
                case OperationType.Moved:
                    imageFile = "new_parent.png";
                    break;           
                case OperationType.TemplateChanged:
                    imageFile = "changed_template.png";
                    break;  
                case OperationType.Serialized:
                    imageFile = "serialized.png";
                    break;
                default:
                    imageFile = "info.png";
                    break;
            }

            string packUri = "../Resources/operations/" + imageFile;
            return packUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}