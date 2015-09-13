using System.Collections.Generic;

namespace Unicorn.VS.Models
{
    public class Report
    {
        public int Progress { get; set; }
        public IEnumerable<StatusReport> StatusReports { get; set; }
    }
}