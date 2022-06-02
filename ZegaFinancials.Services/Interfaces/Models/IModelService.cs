using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Models
{
    public interface IModelService
    {
        DataGridModel LoadModelsByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        void SaveModel(ModelModel model, UserContextModel userContext);
        void DeleteModels(int[] modelIds,UserContextModel userContext);
        ModelModel GetModelById(int model, UserContextModel userContext);
        IList<ModelItemsModel> GetModelListForExport(ModelModel[] models);
        CommonDropdownsModel GetModelDetailsDropDowns(UserContextModel userContext);
        IList<ZegaModel> GetModelsByAccountIds(UserContextModel userContext, int[] accountIds);
        void BulkEditModels(BulkEditModel bulkChanges, UserContextModel userContext);
        ModelIdModel[] GetByFilter();
        ModelIdModel GetModel(int modelId);
    }
}