using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities.Models;

namespace ZegaFinancials.Services.Models.Models
{
    public class ModelSleeveModel :ZegaModel
    {
        public virtual SleeveModel Sleeve { get; set; }
    }
}
