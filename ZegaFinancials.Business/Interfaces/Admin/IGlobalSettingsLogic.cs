using ZegaFinancials.Nhibernate.Entities.Admin;

namespace ZegaFinancials.Business.Interfaces.Admin
{
    public interface IGlobalSettingsLogic
    {
        GlobalSettings Get();
        void Persist(GlobalSettings globalSettings);

    }
}
