using System;
using Unicorn.Remote.Logging;
using Unicorn.VS.Types;

namespace Unicorn.VS.Models
{
    public class StatusReport
    {
        private StatusReport(string text, MessageLevel level, OperationType type, ReportType reportType)
        {
            Message = text;
            MessageLevel = level;
            OperationType = type;
            MessageTime = DateTime.Now;
            ReportType = reportType;
        }

        public DateTime MessageTime { get; private set; }
        public MessageLevel MessageLevel { get; private set; }
        public OperationType OperationType { get; private set; }
        public ReportType ReportType { get; private set; }
        public string Message { get; private set; }

        public static StatusReport CreateOperation(string text, MessageLevel level, OperationType type)
        {
            return new StatusReport(text, level, type, ReportType.Operation);
        }

        public static StatusReport CreateProgress(string value)
        {
            return new StatusReport(value, MessageLevel.Info, OperationType.None, ReportType.Progress);
        }

        public bool IsProgressReport()
        {
            return ReportType == ReportType.Progress;
        }
    }
}