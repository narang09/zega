using System;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Models.Logging
{
    public class LogModel
    {
        public string Message { get; set; }
        public EntityType Type { get; set; }
        public CommonModel User { get; set; }
        public DateTime Date { get; set; }
    }
}
