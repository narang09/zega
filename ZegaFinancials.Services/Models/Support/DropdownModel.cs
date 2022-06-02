 using System.Collections.Generic;

namespace ZegaFinancials.Services.Models.Support
{
    public class DropdownModel
    {
        public DropdownModel()
        {
            Status = new Dictionary<string, int>();
            AccountStatus = new Dictionary<string, int>();
            AccountType = new Dictionary<string, int>();
            Models = new List<ZegaModel>();
            Strategies = new List<ZegaModel>();
            AccountIds = new int[] { };
        }

        public Dictionary<string, int> Status { get; set; }
        public Dictionary<string, int> AccountStatus { get; set; }
        public Dictionary<string, int> AccountType { get; set; }
        public IEnumerable<ZegaModel> Models { get; set; }
        public IEnumerable<ZegaModel> Strategies { get; set; }
        public int[] AccountIds { get; set; }
    }
}