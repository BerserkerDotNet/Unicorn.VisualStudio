using System;
using System.Linq;
using System.Text;
using Unicorn.Remote.Logging;
using Unicorn.VS.Exceptions;
using Unicorn.VS.Models;
using Unicorn.VS.Types;

namespace Unicorn.VS.Helpers
{
    public static class StringExtensions
    {
        const string addedTag = "[A]";
        const string updatedTag = "[U]";
        const string deletedTag = "[D]";
        const string templateChangedTag = "[T]";
        const string renamedTag = "[R]";
        const string movedTag = "[M]";
        const string serializedTag = "[S]";

        public static StatusReport ToReport(this string data)
        {
            var parts = data.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() != 3)
                throw new MalformedPackageReceivedException($"There should be 3 parts of a packet but found {parts.Count()}.");

            var reportType = (ReportType)Enum.Parse(typeof(ReportType), parts[0]);
            var messageLevel = (MessageLevel)Enum.Parse(typeof(MessageLevel), parts[1]);
            var encodedMessage = Convert.FromBase64String(parts[2]);
            var message = Encoding.UTF8.GetString(encodedMessage);

            if (reportType == ReportType.Progress)
                return StatusReport.CreateProgress(message);

            var operationType = OperationType.None;
            message = message.Trim();
            if (message.Contains(addedTag))
            {
                message = message.Replace(addedTag, "").Trim();
                operationType = OperationType.Added;
            }
            else if (message.Contains(updatedTag))
            {
                message = message.Replace(updatedTag, "").Trim();
                operationType = OperationType.Updated;
            }
            else if (message.Contains(deletedTag))
            {
                message = message.Replace(deletedTag, "").Trim();
                operationType = OperationType.Deleted;
            }
            else if (message.Contains(templateChangedTag))
            {
                message = message.Replace(templateChangedTag, "").Trim();
                operationType = OperationType.TemplateChanged;
            }
            else if (message.Contains(renamedTag))
            {
                message = message.Replace(renamedTag, "").Trim();
                operationType = OperationType.Renamed;
            }
            else if (message.Contains(movedTag))
            {
                message = message.Replace(movedTag, "").Trim();
                operationType = OperationType.Moved;
            }
            else if (message.Contains(serializedTag))
            {
                message = message.Replace(serializedTag, "").Trim();
                operationType = OperationType.Serialized;
            }

            return StatusReport.CreateOperation(message, messageLevel, operationType);
        }
    }
}