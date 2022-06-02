using ZegaFinancials.Nhibernate.Entities.Shared;
using System.Linq;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Business.Interfaces.Strategies;
using ZegaFinancials.Services.Interfaces.Strategies;
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Strategies;
using System.Diagnostics.CodeAnalysis;
using System;
using ZegaFinancials.Services.Models.Strategies;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Business.Interfaces.Users;

namespace ZegaFinancials.Services.Impl.Strategies
{
    public class StrategyService : ZegaService, IStrategyService
    {
        private readonly IStrategyLogic _strategyLogic;
        public StrategyService(IStrategyLogic strategyLogic, IUserLogic userLogic): base(userLogic)
        {
            _strategyLogic = strategyLogic;
        }

        public DataGridModel LoadStrategiesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {

            int count;
            CheckUserContext(userContext);
            var strategies = _strategyLogic.GetStrategiesByFilter(dataGridFilterModel, out count,userContext.IsAdmin);
            var dataGridModel = new DataGridModel();
            dataGridModel.Strategies = GetStrategyModelList(strategies).ToArray() ;
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }

        public static IList<StrategyListingModel> GetStrategyModelList([NotNull] IEnumerable<Strategy> strategies)
        {
            if (strategies == null)
                throw new ArgumentNullException("strategies");

            var modelStrategy = new List<StrategyListingModel>();
            foreach (var strategy in strategies)
            {
                var ms = Map(strategy, new StrategyListingModel
                {
                    Description = strategy.Description,
                    Name = strategy.Name,
                    ModelsCount = strategy.ModelsCount
                });
                modelStrategy.Add(ms);
            }
            return modelStrategy;
        }

        public void SaveStrategyInfo(StrategyModels strategyModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Strategy strategy;
            if (strategyModel.Id != 0)
                strategy = _strategyLogic.GetStrategyById(strategyModel.Id);
            else
                strategy = _strategyLogic.CreateStrategy();
            if (strategy == null)
                throw new ZegaServiceException("No such Strategy");
            if (!strategy.IsBlendedStrategy)
            {
                if (string.IsNullOrEmpty(strategyModel.Name))
                    throw new ZegaServiceException("Invalid Strategy Name");
                strategy.Name = strategyModel.Name;
                strategy.Description = strategyModel.Description ?? "";
                _strategyLogic.Persist(strategy);

                if (strategy != null)
                {
                    var addedModel = new List<ModelModel>();
                    strategy.Name = strategyModel?.Name ?? null;
                    strategy.Description = strategyModel?.Description ?? null;
                    if (strategy.Models == null)
                        strategy.Models = new List<StrategyModel>();
                    if (strategyModel.Models != null)
                    {
                        //Remove Models 
                        var oldModelList = strategy.Models;
                        for (var i = 0; i < oldModelList.Count; i++)
                        {
                            var model = strategyModel.Models.FirstOrDefault(o => o.Id == oldModelList[i].Model.Id);
                            if (model != null)
                                strategyModel.Models.Remove(model);
                            else
                            {
                                oldModelList.RemoveAt(i);
                                i--;
                            }
                        }
                        //add models
                        foreach (var model in strategyModel.Models)
                        {
                            addedModel.Add(model);
                            strategy.Models.Add(new StrategyModel
                            {
                                Strategy = strategy,
                                Model = new Model
                                {
                                    Id = model.Id,
                                    Name = model.Name
                                }
                            }
                            );
                        }
                        if (addedModel.Any(o => o.IsBlendModel))
                            throw new ZegaServiceException("Blended Models can only be a part of Blended Strategy!");
                    }
                }
            }
        }
        public void DeleteStrategiesByIds(int[] strategyIds, UserContextModel userContext)
        {

            CheckUserContext(userContext);
            if (strategyIds.Any())
            foreach (var id in strategyIds)
                 if (_strategyLogic.IsStrategyExists(id))
                    {
                        var strategy = _strategyLogic.GetStrategyById(id);
                        if(strategy.IsBlendedStrategy)
                           throw new ZegaServiceException("Blended Strategy Can't be delete !");
                        _strategyLogic.DeleteStrategyById(id);
                    }
        }

        public StrategyModels GetStrategyById(int id, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Strategy strategy;
                if (id != 0)
                    strategy = _strategyLogic.GetStrategyById(id);
                else
                    return new();
           var ms=  Map(strategy, new StrategyModels
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Description = strategy.Description,
                Models = strategy.Models?.Select(o => new ModelModel
                { 
                     
                        Id = o.Model.Id,
                        Name = o.Model.Name,
                        Description = o.Model.Description

                }).ToList() ?? null
            });

          return  ms;
            }

        public IList<ZegaModel> GetAllStrategies()
        {
           var strategies = _strategyLogic.GetAllStrategies();
           var strategyDropDownList = new List<ZegaModel>();
            if (strategies != null)
            {
                strategyDropDownList = strategies.Select(o => new ZegaModel
                {
                    Id = o.Id,
                    Name = o.Name,
                }).ToList();
            }
            return strategyDropDownList;
        }
        public IEnumerable<ZegaModel> GetAllStrategies(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var strategies = _strategyLogic.GetAllStrategies().Where(o => !o.IsBlendedStrategy).Select(x => new ZegaModel {
               Id= x.Id,
               Name= x.Name
            });
            return strategies;
        }
    }
}

