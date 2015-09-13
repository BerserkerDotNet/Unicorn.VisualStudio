using System;

namespace Unicorn.Remote.Logging
{
    public class StatusReport
    {
        public StatusReport(string text, MessageLevel level, OperationType type)
        {
            Message = text;
            MessageLevel = level;
            OperationType = type;
            MessageTime = DateTime.Now;
        }

        public DateTime MessageTime { get; set; }
        public MessageLevel MessageLevel { get; private set; }
        public OperationType OperationType { get; private set; }
        public string Message { get; private set; }

    }
}