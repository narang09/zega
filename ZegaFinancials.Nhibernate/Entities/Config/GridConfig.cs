using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Entities.Config
{
    public class GridConfig : ZegaEntity
    {
        public GridConfig()
        {
            User = null;
            IsDefault = false;
            SortingJSonValue = string.Empty;
        }
        public virtual string GridName { get; set; }
        public virtual string GridColumnJSonValue { get; set; }
        public virtual User User { get; set; }
        public virtual DataRequestSource GridType { get; set; }
        public virtual bool IsDefault { get; set;}
        public virtual string SortingJSonValue { get; set; }
    }
}
