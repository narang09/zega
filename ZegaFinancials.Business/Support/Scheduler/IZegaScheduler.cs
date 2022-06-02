using System.Threading.Tasks;

namespace ZegaFinancials.Business.Support.Scheduler
{
    public interface IZegaScheduler
    {
        Task ChangeImportTrigger(int hour, int minute, bool autoImportEnable);
    }
}
