using System.Collections.Generic;

namespace Unicorn.Remote.Logging
{
    public class Report
    {
        public int Progress { get; set; }
        public IEnumerable<StatusReport> StatusReports { get; set; }
    }
}
