using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Logging
{
    public class AuditLogModel : ZegaModel
    {
        public string Message { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityTypeValue { get { return EnumFunctions.GetNameEnumByValue<EntityType>((int)EntityType); } }
        public int EntityId { get; set; }
        public DateTime Date { get; set; }
    }
}
