using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Impl.Models
{
    public class SleeveLogic : ZegaLogic, ISleeveLogic
    {
        private readonly ISleeveDao _sleeveDao;
        private readonly IModelDao _modelDao;

        public SleeveLogic(ISleeveDao sleeveDao, IModelDao modelDao)
        {
            _sleeveDao = sleeveDao;
            _modelDao = modelDao;
        }
        public IEnumerable<Sleeve> GetSleevesByFilter(DataGridFilterModel model, out int count)
        {
            return _sleeveDao.GetSleevesByFilter(model, out count);
        }
        public Sleeve CreateSleeveEntity()
        {
            return _sleeveDao.Create();
        }
        public void Persist(Sleeve sleeve)
        {
            if (sleeve == null)
                throw new ArgumentNullException("sleeve");
            if (_sleeveDao.IsExists(sleeve.Id,sleeve.Name))
                throw new ZegaLogicException(string.Format("Sleeve with the same name already exists: '{0}'.", sleeve.Name));
            _sleeveDao.Persist(sleeve);
        }
        public Sleeve GetSleeveById(int sleeveId)
        {
            return _sleeveDao.Get(sleeveId);
        }
        public bool IsSleeveExists(int sleeveId)
        {
            return _sleeveDao.IsExists(sleeveId);
        }
        public void DeleteSleeveById(int sleeveId)
        {
            var modelsCount = _modelDao.GetModelsCountBySleeve(sleeveId);
            if (modelsCount > 0)
                throw new ZegaLogicException("Sleeve  is attached to the models.");
            _sleeveDao.Delete(sleeveId);
        }
        public IEnumerable<Sleeve> GetAllSleeves()
        {
            var sleeveList = _sleeveDao.GetAll();
            if (sleeveList != null)
                return sleeveList;
            else
                return new List<Sleeve>();
        }
    }
}
