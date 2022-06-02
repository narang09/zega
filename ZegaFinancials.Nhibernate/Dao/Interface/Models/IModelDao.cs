using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Dao.Impl;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Models
{
    public interface IModelDao : INHibernateDao<Model>
    {
        IEnumerable<Model> GetModelsByFilter(DataGridFilterModel dataGridFilterModel, IEnumerable<int> modelIds, bool isAdmin, out int count);
        IEnumerable<Model> GetModelsByIds(int[] modelIds);
        void CheckModelExistence(string modelName, int modelId ,bool IsBlendModel, bool IsLocalBlend,int accountId = 0);
        public void CheckAdvisorDependency(int modelId);
        public void CheckStrategyDependency(int modelId);
        public void CheckAccountDependency(int modelId);
        long GetModelsCountBySleeve(int sleeveId);
        void CheckBlendModelDependency(int modelId);
        IEnumerable<Model> GetModelsListByRepCodeId(DataGridFilterModel model, int? repCodeId, int accountId, out int count ,int userId, bool isAdmin = false);
        void CheckAccountandAdvisorDependency(Model model, int advisorId);
        Model GetModelByName(string modelName);
        IEnumerable<int> GetModelIdsByUserId(int userId);
    }
}
