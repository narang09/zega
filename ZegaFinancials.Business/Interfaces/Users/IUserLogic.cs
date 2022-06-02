using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using ZegaFinancials.Business.Support;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Business.Interfaces.Users
{
    public interface IUserLogic
    {
        User GetUserByLogin(string login);
        VerifyUserStateAndPasswordResult VerifyUserStateAndPassword([CanBeNull] User user, [NotNull] String enteredPassword);
        IEnumerable<User> GetUsersByFilter(DataGridFilterModel datagridFilterModel, out int count);
        IEnumerable<RepCode> GetRepCodesByFilter(DataGridFilterModel dataGridFilterModel, out int count);
        User GetUserById(int userId);
        User GetUserByPrimaryEmail(string email);
        RepCode GetRepCodeByCode(string code);
        bool IsPrimaryEmailValid(string primaryEmail,int userId);
        RepCode GetRepCodeById(int repCodeId);
        IEnumerable<User> GetAllAdvisors();
        bool IsRepCodeExist(RepCode repcode);
        void DeleteRepCodes(IEnumerable<RepCode> repCodes, string userName);
        RepCode CreateRepCodeEnitity();
        AdvisorRepcode CreateAdvisorRepCodeEntity();
        void Persist(IEnumerable<RepCode> repCodes);
        void DeleteUsersById(int Id);
        void DeleteRepCodeById(int Id);
        void Persist(User user);
        void Persist(RepCode repCode);
        User CreateUserEntity();
        IList<User> GetUsersByRepCodeIds(IEnumerable<int> Ids);
        public UserPhone CreatePhoneNumber();
        public AdvisorModel CreateModel();
        public UserEmail CreateEmail();
        void Persist(IEnumerable<User> users);
        IEnumerable<User> GetUsersByIds(int[] userIds);
        void SendMail(string primaryEmail, string message, string subject);
        bool ResetPassword(User userInfo);
        public IList<int> GetAllRepCodeIds();
        public IList<int> GetAllModelIds();
        bool IsPrimaryPhoneNumberValid(string phoneNo,int userId);
        IEnumerable<RepCode> GetRepCodesByAdvisorId(int advisorId);
        LoginActivity GetLoginActivity(int userId, string sessionId);
        void DeleteLoginActivity(string sessionId);
        void SaveLoginActivity(LoginActivity loginActivity);
        void CheckAccountDependency(RepCode repCode, User user =null);
        void CheckAdvisorDependency(RepCode repCode);
        IEnumerable<AdvisorRepcode> GetAllRepCodeAdvisors();
        IEnumerable<string> GetRepCodeByIds(IEnumerable<int> repcodeIds);
        IEnumerable<RepCode> GetAllRepCodes();
    }
}
