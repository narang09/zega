using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Services.Models.Shared;
using System.Linq;
using ZegaFinancials.Nhibernate.Entities.Models;
using System;
using System.Collections.Generic;
using ZegaFinancials.Business.Interfaces.Strategies;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Strategies;
using ZegaFinancials.Services.Interfaces.Strategies;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Business.Interfaces.Accounts;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Services.Impl.Models
{
    public class ModelService : ZegaService, IModelService
    {
        private readonly IModelLogic _modelLogic;
        private readonly IStrategyLogic _strategyLogic;
        private readonly IStrategyService _strategyService;
        private readonly ISleeveService _sleeveService;
        private readonly ISleeveLogic _sleeveLogic;
        private readonly IAccountLogic _accountLogic;
        private readonly IAuditLogLogic _auditLogLogic;
        public ModelService(IModelLogic modelLogic, IStrategyLogic strategyLogic, IStrategyService strategyService, ISleeveService sleeveService, ISleeveLogic sleeveLogic, IAccountService accountService, IAccountLogic accountLogic, IUserLogic userLogic, IAuditLogLogic auditLogLogic) : base(userLogic)
        {
            _modelLogic = modelLogic;
            _strategyLogic = strategyLogic;
            _strategyService = strategyService;
            _sleeveService = sleeveService;
            _sleeveLogic = sleeveLogic;
            _accountLogic = accountLogic;
            _auditLogLogic = auditLogLogic;
        }
        public void DeleteModels(int[] modelIds, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (modelIds == null && !modelIds.Any())
                throw new ZegaServiceException("Models are not selected!");
            var strategyModels = _strategyLogic.GetStrategyModelList().Where(x => x.Model != null)?.GroupBy(o => o.Model.Id)?.ToDictionary(o => o.Key, o => o.Select(o => o.Strategy).ToList());
            foreach (var modelId in modelIds)
            {
                var model = _modelLogic.GetModelById(modelId);
                if (model != null)
                {
                    if (strategyModels != null && strategyModels.ContainsKey(modelId))
                    {
                        foreach (var strategy in strategyModels[modelId])
                        {
                            var strategyModel = strategy.Models.FirstOrDefault(o => o.Model.Id == modelId);
                            strategy.Models.Remove(strategyModel);
                            _strategyLogic.Persist(strategy);
                        }
                    }
                    _modelLogic.CheckAccountDependency(modelId);
                    _modelLogic.CheckAdvisorDependency(modelId);
                    _modelLogic.CheckBlendModelDependency(modelId);
                    _modelLogic.DeleteModelsById(modelId);
                }
                else
                    throw new ZegaServiceException(string.Format("Invalid Model Id: {0}", modelId));
            }
        }
        public ModelModel GetModelById(int id, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Model model;
            if (id != 0)
                model = _modelLogic.GetModelById(id);
            else
                throw new ZegaServiceException(string.Format("Model Not Found Id:{0}", id));
            if (model != null)
            {
                var modelModel = Map(model, new ModelModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    IsBlendModel = model.IsBlendModel,
                    ModelItems = model.IsBlendModel ?
                     model.SubModels?.Select(o => new SleeveModel
                     {
                         Id = o.SubModel?.Id ?? 0,
                         Name = o.SubModel?.Name,
                         Description = o.SubModel?.Description,
                         Allocation = o.Allocation,
                         Items = o.SubModel?.ModelSleeves?.Select(x => new SleeveModel
                         {
                             Id = x.Sleeve?.Id ?? 0,
                             Name = x.Sleeve?.Name,
                             Description = x.Sleeve?.Description,
                             Allocation = o.Allocation * x.Allocation
                         }).ToList()
                     }).ToList() : model.ModelSleeves?.Select(o => new SleeveModel
                     {
                         Id = o.Sleeve?.Id ?? 0,
                         Name = o.Sleeve?.Name,
                         Description = o.Sleeve?.Description,
                         Allocation = o.Allocation,
                     }).ToList(),
                    Strategies = _strategyLogic.GetByModel(model.Id).Select(o => o.Id).ToList(),
                });
                return modelModel;
            }
            else
                throw new ZegaServiceException(string.Format("Model Not Found Id :{0}", id));
        }

        public ModelIdModel GetModel(int modelId)
        {
            Model model;
            if (modelId != 0)
                model = _modelLogic.GetModelById(modelId);
            else
                throw new ZegaServiceException(string.Format("Model Not Found Id : {0}", modelId));

            if (model != null)
            {
                var modelModel = Map(model, new ModelIdModel
                {
                    Id = model.Id,
                    ModelId = model.Name,
                    Name = model.Description
                });
                return modelModel;
            }
            else
                throw new ZegaServiceException(string.Format("Model Not Found Id : {0}", modelId));
        }
        public IList<ModelItemsModel> GetModelListForExport(ModelModel[] models)
        {
            var items = new List<ModelItemsModel>();
            foreach (var model in models)
            {
                var modelItem = new ModelItemsModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    StrategyNames = model.StrategyNames

                };
                items.Add(modelItem);
                if (model.ModelItems != null && model.ModelItems.Any())
                {
                    foreach (var subItem in model.ModelItems)
                    {
                        var modelSubItem = new ModelItemsModel
                        {
                            Id = subItem.Id,
                            Name = subItem.Name,
                            AllocationUI = subItem.AllocationUI.ToString(),
                            Description = subItem.Description
                        };
                        items.Add(modelSubItem);
                    }
                }
            }
            return items;
        }

        public DataGridModel LoadModelsByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            var modelStrategyNames = _strategyLogic.GetStrategyModelList().Where(x => x.Model != null)?.GroupBy(o => o.Model.Id)?.ToDictionary(o => o.Key, o => o.Select(o => o.Strategy.Name).ToList());
            IEnumerable<int> modelIds = userContext.IsAdmin ? new List<int>() : _modelLogic.GetModelIdsByUserId(userContext.Id);
            var models = _modelLogic.GetModelsByFilter(dataGridFilterModel, modelIds, userContext.IsAdmin, out count).Select(o => new ModelModel
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                IsBlendModel = o.IsBlendModel,
                StrategyNames = modelStrategyNames != null && modelStrategyNames.ContainsKey(o.Id) ? string.Join(",", modelStrategyNames[o.Id]) : null,
                ModelItems = o.IsBlendModel ?
            o.SubModels?.Select(x => new SleeveModel
            {
                Id = x.Id,
                Name = x.SubModel?.Name,
                Description = x.SubModel?.Description,
                Allocation = x.Allocation
            }).ToList() : o.ModelSleeves?.Select(x => new SleeveModel
            {
                Id = x.Id,
                Name = x.Sleeve?.Name,
                Description = x.Sleeve?.Description,
                Allocation = x.Allocation
            }).ToList(),
            }).ToArray();

            var dataGridModel = new DataGridModel();
            if (dataGridFilterModel.GridAdditionalParameters != null && dataGridFilterModel.GridAdditionalParameters.RepCodeId != null && dataGridFilterModel.GridAdditionalParameters.RepCodeId > 0)
            {
                var Models = _modelLogic.GetModelsListByRepCodeId(dataGridFilterModel, dataGridFilterModel.GridAdditionalParameters.RepCodeId, dataGridFilterModel.GridAdditionalParameters.AccountId, out count, userContext.Id, userContext.IsAdmin);
                var modelsList = Models != null ? Models.Select(o => new ModelModel()
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    IsBlendModel = o.IsBlendModel,
                    AccountId = o.AccountId,
                    StrategyNames = modelStrategyNames != null && modelStrategyNames.ContainsKey(o.Id) ? string.Join(",", modelStrategyNames[o.Id]) : null
                }) : new List<ModelModel>();
                dataGridModel.Models = modelsList.ToArray();
                dataGridModel.TotalRecords = count;
                return dataGridModel;
            }
            dataGridModel.Models = models;
            dataGridModel.TotalRecords = count;
            return dataGridModel;
        }

        public ModelIdModel[] GetByFilter()
        {
            int count;
            var models = _modelLogic.GetModelsByFilter(new DataGridFilterModel(), new int[0], true, out count);
            if (models == null)
                return new ModelIdModel[0];

            return models.Select(o => new ModelIdModel
            {
                Id = o.Id,
                ModelId = o.Name,
                Name = o.Description
            }).ToArray();
        }

        public CommonDropdownsModel GetModelDetailsDropDowns(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var sleeveList = _sleeveService.GetAllSleeves();
            var strategyList = _strategyService.GetAllStrategies();
            var dropdowns = new CommonDropdownsModel()
            {
                Sleeves = sleeveList != null ? sleeveList : new List<SleeveModel>(),
                Strategies = strategyList != null ? strategyList : new List<ZegaModel>()
            };
            return dropdowns;
        }

        public void SaveModel(ModelModel modelModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Model model;
            List<Strategy> removedStrategies = new List<Strategy>();
            List<Strategy> addedStrategies = new List<Strategy>();
            model = modelModel.Id == 0 ? _modelLogic.CreateEntity() : _modelLogic.GetModelById(modelModel.Id);
            if (model != null)
            {
                model.Name = modelModel.Name;
                model.IsBlendModel = modelModel.IsBlendModel;
                model.IsLocalBlend = modelModel.IsLocalBlend;
                model.Description = modelModel.Description;
                if (model.IsBlendModel)
                {
                    AddOrRemoveModelsFromBlendModel(model, modelModel);
                    if (model.SubModels != null && model.SubModels.Any(o => o.SubModel.IsBlendModel))
                        throw new ZegaServiceException("Blend Model can't Contains other Blended Model");
                    if (model.SubModels != null && model.SubModels.Any() && model.SubModels.Sum(o => o.Allocation) != 1)
                        throw new ZegaServiceException("Sum of Blended Models Allocation % should be 100");
                    var sleeves = model.SubModels?.Select(o => o.SubModel)?.SelectMany(o => o.ModelSleeves)?.Select(o => o.Sleeve)?.GroupBy(o => o)?.Select(o => new { sleeve = o.Key, count = o.ToList().Count })?.Where(o => o.count >= 2)?.Select(o => o.sleeve.Name);
                    if (sleeves != null && sleeves.Any())
                        throw new ZegaServiceException("Can't Blend Models that contain Same Sleeves : {0}", String.Join(",", sleeves));
                    if (!model.IsLocalBlend)
                    {
                        var blendedStrategy = _strategyLogic.GetBlendedStrategy();
                        if (blendedStrategy != null && blendedStrategy.IsBlendedStrategy)
                            modelModel.Strategies.Add(blendedStrategy.Id);
                        else
                            throw new ZegaServiceException("Blended Strategy not Found!");
                    }
                }
                else
                {
                    if (modelModel.IsSleeveUpdated)
                        AddOrRemoveSleevesFromModel(model, modelModel, userContext.Id);
                    if (model.ModelSleeves != null && model.ModelSleeves.Any() && model.ModelSleeves?.Sum(o => o.Allocation) != 1)
                        throw new ZegaServiceException("Sum of Sleeves Allocation % should be 100 ");
                }
                if (model.IsBlendModel && model.IsLocalBlend)
                {
                    if (_accountLogic.IsExist(modelModel.AccountId))
                    {
                        model.AccountId = modelModel.AccountId;
                    }
                    else
                        throw new Exception(string.Format("InValid Account Id {0}", modelModel.AccountId));
                    _modelLogic.CheckModelExistence(model.Name, model.Id, true, true, model.AccountId);
                }
                else if (model.IsBlendModel && !model.IsLocalBlend)
                {
                    _modelLogic.CheckModelExistence(model.Name, model.Id, true, false);
                }
                else
                {
                    _modelLogic.CheckModelExistence(model.Name, model.Id, false, false);
                }
                _modelLogic.Persist(model);
                AddOrRemoveModelFromStrategies(model, modelModel.Id, modelModel.Strategies, removedStrategies, addedStrategies);
                addedStrategies.AddRange(removedStrategies);
                if (!model.IsBlendModel && addedStrategies.Any(o => o.IsBlendedStrategy))
                    throw new ZegaServiceException("Only Blended Models can come under Blended Strategy !");
                _strategyLogic.Persist(addedStrategies);
            }
            else
                throw new ZegaServiceException(string.Format("Model Not Found Id:{0}", modelModel.Id));
        }
        private void AddOrRemoveModelFromStrategies(Model model, int modelId, IList<int> modelStrategyIds, List<Strategy> removedStrategies, List<Strategy> addedStrategies)
        {

            if (modelStrategyIds == null)
                modelStrategyIds = new List<int>();
            // Case : when Model is Edited 
            if (modelId != 0)
            {
                var strategyList = _strategyLogic.GetByModel(model.Id).ToList();
                if (modelStrategyIds != null && strategyList.Any())
                {
                    var strategyModelList = modelStrategyIds;
                    //Case When Model is already  attached in  strategies
                    if (strategyList != null && strategyList.Any())
                    {
                        // Remove models from Strategy
                        for (int i = 0; i < strategyList.Count; i++)
                        {
                            if (!strategyModelList.Contains(strategyList[i].Id))
                            {
                                var strategyModel = strategyList[i].Models.FirstOrDefault(o => o.Model.Id == model.Id);
                                strategyList[i].Models.Remove(strategyModel);
                                removedStrategies.Add(strategyList[i]);
                                strategyList.Remove(strategyList[i]);
                                i--;
                            }
                        }
                        //Add models into strategy
                        var strategyIds = strategyList.Select(o => o.Id);
                        foreach (var strategyId in modelStrategyIds)
                        {
                            if (!strategyIds.Contains(strategyId))
                            {
                                var strategy = _strategyLogic.GetStrategyById(strategyId);
                                if (strategy == null)
                                    throw new ZegaServiceException(string.Format("Strategy Not Found Id:{0}", strategyId));
                                strategy.Models.Add(new StrategyModel()
                                {
                                    Model = model,
                                    Strategy = strategy
                                });
                                addedStrategies.Add(strategy);
                            }
                        }
                    }
                }
                else
                {
                    // Case Add into strategy : When the model is not attached in any strategies  
                    if (modelStrategyIds != null && !strategyList.Any())
                    {
                        foreach (var strategyId in modelStrategyIds)
                        {
                            var strategy = _strategyLogic.GetStrategyById(strategyId);
                            if (strategy == null)
                                throw new ZegaServiceException(string.Format("Strategy Not Found Id:{0}", strategyId));
                            strategy.Models.Add(new StrategyModel()
                            {
                                Model = model,
                                Strategy = strategy
                            });
                            addedStrategies.Add(strategy);
                        }
                    }
                }
            }
            else
            {
                //Case : When New Model is Created
                if (modelStrategyIds != null && modelStrategyIds.Any())
                {
                    foreach (var strategyId in modelStrategyIds)
                    {
                        var strategy = _strategyLogic.GetStrategyById(strategyId);
                        if (strategy == null)
                            throw new ZegaServiceException(string.Format("Strategy Not Found Id: {0}", strategyId));
                        strategy.Models.Add(new StrategyModel() { Model = model, Strategy = strategy });
                        addedStrategies.Add(strategy);
                    }
                }

            }
        }
        private void AddOrRemoveModelsFromBlendModel(Model model, ModelModel modelModel)
        {
            if (model.SubModels == null)
                model.SubModels = new List<BlendModelDetails>();
            if (modelModel.ModelItems != null)
            {
                //Remove blendedmodels
                var oldBlendedmodelList = model.SubModels;
                for (var i = 0; i < oldBlendedmodelList.Count; i++)
                {
                    var blendModel = modelModel.ModelItems.FirstOrDefault(o => o.Id == oldBlendedmodelList[i].Id);
                    if (blendModel != null)
                        modelModel.ModelItems.Remove(blendModel);
                    else
                    {
                        oldBlendedmodelList.RemoveAt(i);
                        i--;
                    }
                }
                //Add blended models
                foreach (var blendModel in modelModel.ModelItems)
                {
                    var submodel = _modelLogic.GetModelById(blendModel.Id);
                    if (submodel == null)
                        throw new ZegaServiceException("Model not Found Id:{0} ,Name:{1}", blendModel.Id, blendModel.Name);
                    model.SubModels.Add(new BlendModelDetails
                    {
                        SubModel = new Model
                        {
                            Id = submodel.Id,
                            Name = submodel.Name,
                            Description = submodel.Description,
                            IsBlendModel = submodel.IsBlendModel,
                            ModelSleeves = submodel.ModelSleeves

                        },
                        Allocation = blendModel.Allocation,
                    }
                    );
                }
            }
        }
        private void AddOrRemoveSleevesFromModel(Model model, ModelModel modelModel, int userId)
        {
            if (model.ModelSleeves == null)
                model.ModelSleeves = new List<ModelSleeve>();

            var sleeveMessages = new List<string>();
            var removedMessages = new List<string>();

            if (modelModel.ModelItems != null)
            {
                //Remove Sleeves 
                var oldSleeveList = model.ModelSleeves;
                for (var i = 0; i < oldSleeveList.Count; i++)
                {
                    var sleeve = modelModel.ModelItems.FirstOrDefault(o => o.Id == oldSleeveList[i].Sleeve.Id);
                    if (sleeve != null)
                    {
                        sleeveMessages.Add(string.Format("\"{0}\" (Model Percent was changed from \"{1}\" to \"{2}\". )", sleeve.Name, oldSleeveList[i].Allocation, sleeve.Allocation));
                        oldSleeveList[i].Allocation = sleeve.Allocation;
                        modelModel.ModelItems.Remove(sleeve);
                    }
                    else
                    {
                        removedMessages.Add(oldSleeveList[i].Sleeve.Name);
                        oldSleeveList.RemoveAt(i);
                        i--;
                    }
                }
                //add sleeves
                foreach (var modelsleeve in modelModel.ModelItems)
                {
                    var sleeve = _sleeveLogic.GetSleeveById(modelsleeve.Id);
                    if (sleeve == null)
                        throw new ZegaServiceException("Sleeve Not Found Id:{0} ,Name:{1}", modelsleeve.Id, modelsleeve.Name);

                    sleeveMessages.Add(string.Format("Sleeve \"{0}\" was added (Model Percent \"{1}\".)", sleeve.Name, modelsleeve.Allocation));
                    model.ModelSleeves.Add(new ModelSleeve
                    {
                        Model = model,
                        Sleeve = new Sleeve
                        {
                            Id = sleeve.Id,
                            Name = sleeve.Name,
                            Description = sleeve.Description
                        },
                        Allocation = modelsleeve.Allocation,
                    }
                    );
                }
                if (removedMessages.Count() > 0 || sleeveMessages.Count() > 0)
                {
                    var message = string.Format("Sleeves for model \"{0}\" were changed. {1}{2}", model.Name,
                        removedMessages.Count() > 0 ? (string.Format("Removed sleeves: {0}. ", string.Join(", ", removedMessages))) : string.Empty,
                        sleeveMessages.Count() > 0 ? (string.Format("Modified sleeves: {0}. ", string.Join(", ", sleeveMessages))) : string.Empty);

                    _auditLogLogic.Log(EntityType.Model, message, userId);
                }
            }
        }
        public IList<ZegaModel> GetModelsByAccountIds(UserContextModel userContext, int[] accountIds)
        {
            CheckUserContext(userContext);
            var modelDropdownList = new List<ZegaModel>();
            var accountRepCodeIds = _accountLogic.GetAccountsByIds(accountIds)?.Select(o => o.RepCode.Id);
            var advisors = accountRepCodeIds != null ? _userLogic.GetUsersByRepCodeIds(accountRepCodeIds)?.Distinct() : new List<User>();
            var modelsWithItsCount = advisors?.SelectMany(o => o.Models)?.Select(x => x.Model)?.GroupBy(o => o).Select(x => new { key = x.Key, count = x.ToList().Count });
            var commonModelsAmongAdvisors = modelsWithItsCount != null ? modelsWithItsCount.Where(x => x.count == advisors.Count())?.Select(o => new ZegaModel { Id = o.key.Id, Name = o.key.Name }) : null;
            if (commonModelsAmongAdvisors != null)
                modelDropdownList.AddRange(commonModelsAmongAdvisors);
            return modelDropdownList;
        }
        public void BulkEditModels(BulkEditModel bulkChanges, UserContextModel userContext)
        {
            var removedStrategies = new List<Strategy>();
            var addedStrategies = new List<Strategy>();
            CheckUserContext(userContext);
            if (!bulkChanges.DataStoreIds.Any())
                return;
            var models = _modelLogic.GetModelsByIds(bulkChanges.DataStoreIds.ToArray());
            var strategies = _strategyLogic.GetStrategyByIds(bulkChanges.StrategyIds.ToArray());
            if (models.Any(o => o.IsBlendModel))
                throw new ZegaServiceException("Blended Model can't be Bulk Edited!");
            if (strategies.Any(o => o.IsBlendedStrategy))
                throw new ZegaServiceException("Model Can't be Addded in Blended Strategy, Blended Strategy only Contains Blended Model!");
            foreach (var model in models)
            {
                AddOrRemoveModelFromStrategies(model, model.Id, bulkChanges.StrategyIds, removedStrategies, addedStrategies);
            }
            _modelLogic.Persist(models);
            _strategyLogic.Persist(addedStrategies);

        }
    }
}