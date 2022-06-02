using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Interfaces.Models
{
    public interface IModelLogic
    {
        IEnumerable<Model> GetModelsByFilter(DataGridFilterModel model, IEnumerable<int> modelIds, bool isAdmin, out int count);
        Model GetModelById(int modelId);
        IEnumerable<Model> GetModelsByIds(int[] modelIds);
        void Persist(IEnumerable<Model> models);
        void Persist(Model model);
        Model CreateEntity();
        void DeleteModelsById(int modelId);
        bool IsExist(int id);
        void CheckAdvisorDependency(int id);
        void CheckStrategyDependency(int id);
        void CheckAccountDependency(int modelId);
        void CheckBlendModelDependency(int modelId);
        IEnumerable<Model> GetAllModels();
        IEnumerable<Model> GetModelsListByRepCodeId(DataGridFilterModel model, int? repCodeId, int accountId, out int count , int userId ,bool IsAdmin = false);
        void CheckModelExistence(string modelName, int modelId,bool IsBlendModel,bool IsLocalBlend ,int accountId = 0);
        void CheckAccountandAdvisorDependency(Model model, int advisorId);
        IEnumerable<int> GetModelIdsByUserId(int userId);
    }
}
