

using System.Collections.Generic;
using ZegaFinancials.Services.Models.Models;

namespace ZegaFinancials.Services.Models
{
  public  class CommonDropdownsModel
    {
        public IList<ZegaModel> Strategies { get; set; }
        public IList<SleeveModel> Sleeves { get; set; }
    }
}
