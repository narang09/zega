using JetBrains.Annotations;
using System;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Dao.Interface.Advisors;
using ZegaFinancials.Business.Support;
using ZegaFinancials.Business.Support.Extensions;
using ZegaFinancials.Nhibernate.Entities.Users;
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Linq;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;

namespace ZegaFinancials.Business.Impl.Users
{
    public class UserLogic : ZegaLogic, IUserLogic
    {
        private readonly IUserDao _userDao;
        private readonly IRepCodeDao _repCodeDao;
        private readonly IAdvisorRepCodeDao _advisorRepCodeDao;
        private readonly IAdvisorModelDao _advisorModelDao;
        private readonly IAdvisorEmailDao _advisorEmailDao;
        private readonly IAdvisorPhoneDao _advisorPhoneDao;
        private readonly IConfiguration _appConfiguration;
        private readonly IModelDao _modelDao;
        private readonly ILoginActivityDao _loginActivityDao;
        private readonly IAccountDao _accountDao;

        public UserLogic(IUserDao userDao , IRepCodeDao repCodeDao, IAdvisorRepCodeDao advisorRepCodeDao, IAdvisorEmailDao advisorEmailDao, IAdvisorModelDao advisorModelDao, IAdvisorPhoneDao advisorPhoneDao, IConfiguration appConfiguration, IModelDao modelDao ,ILoginActivityDao loginActivityDao ,IAccountDao accountDao)
        {
            _userDao = userDao;
            _advisorEmailDao = advisorEmailDao;
            _advisorPhoneDao = advisorPhoneDao;
            _advisorModelDao = advisorModelDao;
            _repCodeDao = repCodeDao;
            _advisorRepCodeDao = advisorRepCodeDao;
            _appConfiguration = appConfiguration;
            _modelDao = modelDao;
            _loginActivityDao = loginActivityDao;
            _accountDao = accountDao;

        }
        public User GetUserByLogin(string login)
        {
            return _userDao.GetByLoginId(login);          
        }

        public IEnumerable<User> GetUsersByFilter(DataGridFilterModel datagridFilterModel, out int count)
        {
             return _userDao.GetUsersByFilter(datagridFilterModel, out count);          
        }

        public VerifyUserStateAndPasswordResult VerifyUserStateAndPassword([CanBeNull] User user, [NotNull] String enteredPassword)
        { 
            if (user == null)
                return VerifyUserStateAndPasswordResult.UserNotFound;

            if (user.Status == Status.InActive)
                return VerifyUserStateAndPasswordResult.InactiveUser;
            if (string.IsNullOrEmpty(user.Password))
                throw new ZegaLogicException("Please Set New Password Using Temporary Password and Link given in Email.");
            if (PasswordEncoder.ComparePasswords(user.Password, enteredPassword))
            {
               user.TempPassword = null;
               return VerifyUserStateAndPasswordResult.Success;
            }
            return VerifyUserStateAndPasswordResult.Failed;
        }
        public User GetUserById(int userId)
        {
            return _userDao.Get(userId);
        }
       
        public RepCode GetRepCodeByCode(string code)
        {
            var repCode = _userDao.GetRepCodeByCode(code);
            return repCode;
        }
        public RepCode GetRepCodeById(int repCodeId)
        {
            var repCode = _userDao.GetRepCodeById(repCodeId);
            return repCode;
        }
        public User CreateUserEntity()
        {
          return  _userDao.Create();
        }

         public void Persist(User user)
        { 
             if (user == null)
                throw new ArgumentNullException("user");

            var exists = _userDao.CheckLoginExist(user.Login, user.Id);
            if (exists)
                throw new ZegaLogicException(string.Format("The user with the same login already exists: '{0}'.", user.Login));

            _userDao.Persist(user);
        }
        public void Persist(IEnumerable<User> users)
        {
            if (users == null)
                throw new ArgumentNullException("users");
            _userDao.Persist(users);
        }
        public void Persist(IEnumerable<RepCode> repCodes)
        {
            _repCodeDao.Persist(repCodes);
        }
        public AdvisorModel CreateModel()
        {
            return _advisorModelDao.Create();
        }

        public AdvisorModel GetModelById(int id)
        {
             return _advisorModelDao.Get(id);
        }

        public UserEmail CreateEmail()
        {
             return _advisorEmailDao.Create();
        }

        public UserEmail GetEmailById(int id)
        {
             return _advisorEmailDao.Get(id);
        }

        public UserPhone CreatePhoneNumber()
        {
             return _advisorPhoneDao.Create();
        }

        public UserPhone GetPhoneNumberById(int id)
        {
            return _advisorPhoneDao.Get(id);
        }
        public RepCode CreateRepCodeEnitity()
        {
            return _repCodeDao.Create();
        }
        public IEnumerable<RepCode> GetRepCodesByFilter(DataGridFilterModel dataGridFilterModel, out int count)
        {
           
           var repCodes= _repCodeDao.GetRepCodesByFilter(dataGridFilterModel, out count);
             return repCodes;

        }
        public void DeleteUsersById(int Id)
        {
            _userDao.Delete(Id);
        }

        public bool IsRepCodeExist(RepCode repCode)
        {
            return _repCodeDao.IsRepCodeExist(repCode);
        }

        public void DeleteRepCodes(IEnumerable<RepCode> repCodes ,string userName)
        {
            foreach (var repCode in repCodes)
            {  
                if (_repCodeDao.IsExist(repCode.Id))
                {
                    var dependentAccounts = _repCodeDao.GetDependentAccountsbyRepCodeId(repCode.Id);
                    if (dependentAccounts != null && dependentAccounts.Any())
                        throw new ZegaLogicException(string.Format("Advisor Can't be update or delete , Repcode {0} Exist in Accounts {1}", repCode.Code, string.Join(",", dependentAccounts)));
                    _repCodeDao.Delete(repCode);
                }
                else
                    throw new ZegaLogicException(string.Format("Invalid Repcode {0}",repCode.Code));
            
            }
        }

        public AdvisorRepcode CreateAdvisorRepCodeEntity()
        {
            return  _advisorRepCodeDao.Create();
        }
        public IList<User> GetUsersByRepCodeIds(IEnumerable<int> Ids)
        {
            var advisors = _advisorRepCodeDao.GetUsersByRepCodeIds(Ids);
            if (advisors != null)
                return advisors;
            else
                return new List<User>();
            
        }
        
        public IEnumerable<User> GetUsersByIds(int[] userIds)
        {
            var users = _userDao.GetUsersByIds(userIds);
            return users;
        }

        public void SendMail(string toMail, string message, string subject)
        {
             
            if (!string.IsNullOrEmpty(_appConfiguration["ZegaEmailID"]) && !string.IsNullOrEmpty(_appConfiguration["ZegaSMPTServer"])&& !string.IsNullOrEmpty(_appConfiguration["ZegaSMPTPort"]))
            {
                var zegaEmailId = _appConfiguration["ZegaEmailID"];
                var password = _appConfiguration["Password"];
                var zegaSmtpServer = _appConfiguration["ZegaSMPTServer"];
                int.TryParse(_appConfiguration["ZegaSMPTPort"], out var zegaSmtpPort);
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtp = new SmtpClient(zegaSmtpServer, zegaSmtpPort);
                    mail.From = new MailAddress(zegaEmailId);
                    mail.To.Add(new MailAddress(toMail));
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = message;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(zegaEmailId, password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    smtp.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception Message: " + ex.Message);
                    if (ex.InnerException != null)
                        Debug.WriteLine("Exception Inner:   " + ex.InnerException);
                    throw;
                }
            }
            else
            {
                throw new ZegaLogicException("Zega Email Configs are incorrect! Please Check Email Configration, It may not send Email to User.");
            }
        }

        public bool ResetPassword(User userInfo)
        {
            
            _userDao.Persist(userInfo);
             
            return true;
        }

        public User GetUserByPrimaryEmail(string email)
        {
           return  _userDao.GetByPrimaryEmail(email);
        }
        public bool IsPrimaryEmailValid (string primaryEmail ,int userId)
        {
            return _userDao.IsPrimaryEmailValid(primaryEmail, userId);
        }
        public IEnumerable<User> GetAllAdvisors()
        {
            return _userDao.GetAllAdvisors();
        }

        public IList<int> GetAllRepCodeIds()
        {
            return _repCodeDao.GetAll()?.Select(o => o.Id).ToList();
        }

        public IList<int> GetAllModelIds()
        {
            return _modelDao.GetAll()?.Select(o => o.Id).ToList();
        }
        public bool IsPrimaryPhoneNumberValid(string phoneNo, int userId)
        {
           return _userDao.IsPrimaryPhoneNumberValid(phoneNo,userId);
        }
        public IEnumerable<RepCode> GetRepCodesByAdvisorId(int advisorId)
        {
            return _userDao.GetRepCodesByAdvisorId(advisorId);
        }

        public LoginActivity GetLoginActivity( int userId, string sessionId)
        {
           return _loginActivityDao.GetLoginActivity(userId ,sessionId);
        }

        public void DeleteLoginActivity(string SessionId)
        {
            _loginActivityDao.DeleteBySessionId(SessionId);
        }

        public void SaveLoginActivity(LoginActivity loginActivity)
        {
            _loginActivityDao.Persist(loginActivity);
        }
        public void Persist(RepCode repCode)
        {
            if (repCode == null)
                throw new ZegaLogicException("RepCode");
            else
                _repCodeDao.Persist(repCode);

        }
        public void DeleteRepCodeById(int Id)
        {
            _repCodeDao.Delete(Id);
        }
        public void CheckAccountDependency(RepCode repCode,User user = null)
        {
            _accountDao.CheckAccountDependencyByRepCode(repCode ,user);
        }

        public IEnumerable<AdvisorRepcode> GetAllRepCodeAdvisors()
        {
             return _userDao.GetAllRepCodeAdvisors();
        }

        public IEnumerable<string> GetRepCodeByIds(IEnumerable<int> repcodeIds)
        {
            if (repcodeIds != null && repcodeIds.Any())
                return _repCodeDao.GetRepCodesByIds(repcodeIds);
            else
                return new List<string>();
        }

        public void CheckAdvisorDependency(RepCode repCode)
        {
            _userDao.CheckAdvisorDependency(repCode);
        }

        public IEnumerable<RepCode> GetAllRepCodes()
        {
            return _advisorRepCodeDao.GetAllRepCodes();
        }
    }
}
       
        
