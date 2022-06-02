using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl.Models
{
    public class SleeveService : ZegaService, ISleeveService
    {
        private readonly ISleeveLogic _sleeveLogic;
        public SleeveService(ISleeveLogic sleeveLogic, IUserLogic userLogic) : base(userLogic)
        {
            _sleeveLogic = sleeveLogic;
        }
        public DataGridModel LoadSleevesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            var sleeves = _sleeveLogic.GetSleevesByFilter(dataGridFilterModel, out count).Select(o => Map(o, new SleeveModel())).ToArray();

            var dataGridModel = new DataGridModel();
            dataGridModel.Sleeves = sleeves;
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }

        public SleeveIdModel[] GetSleevesByFilter()
        {
            int count;
            
            var sleeves = _sleeveLogic.GetSleevesByFilter(new DataGridFilterModel(), out count);
            if (sleeves == null)
                return new SleeveIdModel[0];

            var result = sleeves.Select(o => new SleeveIdModel()
            {
                ModelSleeveId = o.Name
            }).ToArray();
            return result;
        }

        public void SaveSleeveInfo(SleeveModel sleeveModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Sleeve sleeve;
            if (sleeveModel.Id != 0)
                sleeve = _sleeveLogic.GetSleeveById(sleeveModel.Id);
            else
            {
                sleeve = _sleeveLogic.CreateSleeveEntity();
            }

            if (sleeve != null)
            {
                Map(sleeveModel, sleeve);
                _sleeveLogic.Persist(sleeve);

            }

        }

        public SleeveModel GetSleeveById(int sleeveId, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Sleeve sleeve;
            SleeveModel sleeveModel = new();
            if (sleeveId != 0)
                sleeve = _sleeveLogic.GetSleeveById(sleeveId);
            else
                return new();
            Map(sleeve, sleeveModel);
            return sleeveModel;
        }

        public void DeleteSleeves(int[] sleeveIds, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (sleeveIds.Any())
                foreach (var id in sleeveIds)
                    if (_sleeveLogic.IsSleeveExists(id))
                    {
                        var sleeve = _sleeveLogic.GetSleeveById(id);
                        _sleeveLogic.DeleteSleeveById(id);
                    }
        }

        public IList<SleeveModel> GetAllSleeves()
        {
            var sleeves = _sleeveLogic.GetAllSleeves();
            var sleeveDropDownList = new List<SleeveModel>();
            if (sleeves != null)
            {
                sleeveDropDownList = sleeves.Select(o => new SleeveModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description
                }).ToList();
            }
            return sleeveDropDownList;
        }
    }
}
