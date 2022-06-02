using System;
using System.Collections.Generic;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using System.Linq;

namespace ZegaFinancials.Business.Impl.Models
{
    public class ModelLogic : ZegaLogic, IModelLogic
    {
        private readonly IModelDao _modelDao;

        public ModelLogic(IModelDao modelDao)
        {
            _modelDao = modelDao;
        }

        public IEnumerable<Model> GetModelsByFilter(DataGridFilterModel model, IEnumerable<int> modelIds, bool isAdmin, out int count)
        {
            if (!isAdmin && (modelIds == null || !modelIds.Any()))
            {
                count = 0;
                return new List<Model>();
            }
            return _modelDao.GetModelsByFilter(model, modelIds, isAdmin, out count);  
        }

        public Model GetModelById(int modelId)
        {
            return _modelDao.Get(modelId);
        }
        public IEnumerable<Model> GetModelsByIds(int[] modelIds)
        {
            var models = _modelDao.GetModelsByIds(modelIds);
            return models;
        }
        public void Persist(IEnumerable<Model> models)
        {
            if (models == null)
                throw new ArgumentNullException("models");

            _modelDao.Persist(models);
 
          }
        public void Persist(Model model)
        {
            _modelDao.Persist(model);
        }
        public Model CreateEntity()
        {
            return _modelDao.Create();
        }
        public void DeleteModelsById(int modelId)
        {
            _modelDao.Delete(modelId);
        }
        public void CheckAdvisorDependency(int id)
        {
            _modelDao.CheckAdvisorDependency(id);
        }

        public void CheckStrategyDependency(int id)
        {
             _modelDao.CheckStrategyDependency(id);
        }

        public void CheckAccountDependency(int modelId)
        {
            _modelDao.CheckAccountDependency(modelId);
        }

        public bool IsExist(int id)
        {
            return _modelDao.IsExist(id);
        }

        public void CheckBlendModelDependency(int modelId)
        {
            _modelDao.CheckBlendModelDependency(modelId);
        }
        public IEnumerable<Model> GetAllModels()
        {
            return _modelDao.GetAll();
        }
        public IEnumerable<Model> GetModelsListByRepCodeId(DataGridFilterModel model, int? repCodeId, int accountId, out int count , int userId ,bool isAdmin)
        {
            return _modelDao.GetModelsListByRepCodeId(model,repCodeId, accountId, out count , userId, isAdmin);
        }
        public void CheckModelExistence(string modelName,int modelId, bool IsBlendModel, bool IsLocalBlend, int accountId = 0)
        {
            _modelDao.CheckModelExistence(modelName, modelId, IsBlendModel, IsLocalBlend, accountId);
        }
       public void CheckAccountandAdvisorDependency(Model model, int advisorId)
        {
            _modelDao.CheckAccountandAdvisorDependency(model, advisorId);
        }
        public IEnumerable<int> GetModelIdsByUserId(int userId)
        {
            return _modelDao.GetModelIdsByUserId(userId);
        }
    }    
}
    