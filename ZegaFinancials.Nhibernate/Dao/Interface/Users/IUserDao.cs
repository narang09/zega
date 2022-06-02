using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Users
{ 
    public interface IUserDao : INHibernateDao<User>
    {
        User GetByLoginId(string login);
        RepCode GetRepCodeByCode(string repCode);
        IEnumerable<User> GetUsersByIds(int[] userIds);
        IEnumerable<User> GetUsersByFilter(DataGridFilterModel model, out int count);
        bool CheckLoginExist(string login, int userId);
        User GetByPrimaryEmail(string email);
        RepCode GetRepCodeById(int repCodeId);
        bool IsPrimaryEmailValid(string primaryEmail, int userId);
        IEnumerable<RepCode> GetRepCodesByAdvisorId(int advisorId);
        bool IsPrimaryPhoneNumberValid(string phoneNumber, int userId);
        IEnumerable<User> GetAllAdvisors();
        public IEnumerable<AdvisorRepcode> GetAllRepCodeAdvisors();
        public void CheckAdvisorDependency(RepCode repCode);
    }
}
