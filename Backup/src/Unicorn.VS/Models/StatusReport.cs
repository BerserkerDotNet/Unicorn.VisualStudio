using System;
using Unicorn.VS.Types;

namespace Unicorn.VS.Models
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
        public MessageLevel MessageLevel { get; set; }
        public OperationType OperationType { get; set; }
        public string Message { get; set; }

    }
}