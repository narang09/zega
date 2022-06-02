using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Models
{
    public interface ISleeveService
    {
        DataGridModel LoadSleevesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        SleeveIdModel[] GetSleevesByFilter();
        void SaveSleeveInfo (SleeveModel sleeveModel,UserContextModel userContext);
        SleeveModel GetSleeveById(int sleeveId, UserContextModel userContext);
        void DeleteSleeves(int[] sleeveIds,UserContextModel userContext);
        IList<SleeveModel> GetAllSleeves();
    }
}
