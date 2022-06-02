using System.Collections.Generic;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    public sealed class LogInfo
    {
        public LogInfo()
        {
            PropertyIndexes = new HashSet<int>();
            ShowValues = new HashSet<int>();
        }
        public HashSet<int> PropertyIndexes { get; set; }
        public HashSet<int> ShowValues { get; set; }
    }
}
