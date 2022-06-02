using System.Collections.Generic;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountDropdownModel
    {
        public AccountDropdownModel()
        {
            AccountStatus = new Dictionary<string, int>();
            AccountType = new Dictionary<string, int>();
            Frequency = new Dictionary<string, int>();
            RepCodes = new List<ZegaModel>();
            Broker = new Dictionary<string, int>();
        }
        public Dictionary<string, int> AccountStatus { get; set; }
        public Dictionary<string, int> AccountType { get; set; }
        public Dictionary<string, int> Frequency { get; set; }
        public IEnumerable<ZegaModel> RepCodes { get; set; }
        public Dictionary<string, int> Broker { get; set; }
    }
}
