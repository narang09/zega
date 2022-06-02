using System.Linq;
using ZegaFinancials.Business.Interfaces.Admin;
using ZegaFinancials.Nhibernate.Dao.Interface.Admin;
using ZegaFinancials.Nhibernate.Entities.Admin;

namespace ZegaFinancials.Business.Impl.Admin
{
    public class GlobalSettingsLogic : ZegaLogic, IGlobalSettingsLogic
    {
        private readonly IGlobalSettingsDao _globalSettingsDao;
        public GlobalSettingsLogic(IGlobalSettingsDao globalSettingsDao)
        {
            _globalSettingsDao = globalSettingsDao;
        }

        public GlobalSettings Get()
        {
            return _globalSettingsDao.GetAll().FirstOrDefault() ?? new();            
        }

        public void Persist(GlobalSettings globalSettings)
        {
            _globalSettingsDao.Persist(globalSettings);
        }

        
    }
}
