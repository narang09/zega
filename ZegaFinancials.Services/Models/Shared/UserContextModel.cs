using System;
using System.Collections.Generic;

namespace ZegaFinancials.Services.Models.Shared
{
    public partial class UserContextModel : ZegaModel
    {
        public string Login { get; set; }
        public bool LogedIn { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsAdmin { get; set; }
        public bool AllManagersAvailable { get; set; }
        public IList<int> RepcodeIds { get; set; }
        public IList<int> ModelIds { get; set; }
        public string AdditionalInfo { get; set; }
        public string Authorization { get; set; }
    }
}
