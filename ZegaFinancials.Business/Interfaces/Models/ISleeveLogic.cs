using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Interfaces.Models
{
    public interface ISleeveLogic
    {
        IEnumerable<Sleeve> GetSleevesByFilter(DataGridFilterModel model, out int count);
        Sleeve CreateSleeveEntity();
        void Persist(Sleeve sleeve);
        Sleeve GetSleeveById(int sleeveId);
        bool IsSleeveExists(int sleeveId);
        void DeleteSleeveById(int sleeveId);
        IEnumerable<Sleeve> GetAllSleeves();
    }
}
