using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    public interface IAuditableEntity
    {
        DateTime? UpdatedAt { get; set; }
        DateTime? CreateDate { get; set; }
    }
    public interface IsOfDeleteEntity
    {
        DateTime? DeletedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}
