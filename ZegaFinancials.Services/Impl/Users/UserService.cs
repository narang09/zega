using Microsoft.AspNetCore.Http;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Business.Support;
using ZegaFinancials.Business.Support.Extensions;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.RepCodes;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Services.Models.Users;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Services.Models.Models;
using Microsoft.Extensions.Configuration;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Logging;
using NHibernate.Util;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ZegaFinancials.Services.Impl.Users
{
    public class UserService : ZegaService, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IModelLogic _modelLogic;
        private readonly IAccountDao _accountDao;
        private readonly IRepCodeDao _repCodeDao;

        public UserService(IUserLogic userLogic, IConfiguration configuration, IModelLogic modelLogic, ILoginActivityDao loginActivityDao, IAccountDao accountDao , IRepCodeDao repCodeDao) : base(userLogic)
        { 
            _configuration = configuration;
            _modelLogic = modelLogic;
            _accountDao = accountDao;
            _repCodeDao = repCodeDao;
        }
        public UserPermissionModel GetUserPermission(LoginModel userLogin)
        {
            var user = VerifyUserCredentials(userLogin);
            var loginTime = DateTime.Now;
            var userContext = new UserContextModel
            {
                LogedIn = true,
                Id = user.Id,
                Login = user.Login,
                Name = string.Format("{0} {1}", user.Details?.FirstName, user.Details?.LastName),
                LoginTime = loginTime,
                IsAdmin = user.IsAdmin,
                RepcodeIds = user.RepCodes?.Where(x => x.Advisor.Id == user.Id).Select(x => x.RepCode.Id).ToList(),
                ModelIds = user.Models?.Where(x => x.Advisor.Id == user.Id).Select(x => x.Model.Id).ToList()
            };
            userContext.AdditionalInfo = GetUserHash(userContext);

            return new UserPermissionModel
            {
                userContext = userContext
            };
        }

        public User VerifyUserCredentials(LoginModel loginModel)
        {
            User user = _userLogic.GetUserByLogin(loginModel.Login);

            var verifyResult = _userLogic.VerifyUserStateAndPassword(user, loginModel.Password);

            switch (verifyResult)
            {
                case VerifyUserStateAndPasswordResult.Success:
                    break;
                case VerifyUserStateAndPasswordResult.UserNotFound:
                    {
                        throw new ZegaServiceException("The username/password you submitted is incorrect,please try again.");
                    }
                case VerifyUserStateAndPasswordResult.Failed:
                    {
                        throw new ZegaServiceException("The username/password you submitted is incorrect,please try again.");
                    }
                case VerifyUserStateAndPasswordResult.InactiveUser:
                    {
                        throw new ZegaServiceException("Your account is disabled, please contact your system administrator.");
                    }
                default:
                    throw new InvalidOperationException(string.Format("The value {0} is unknown element of VerifyUserStateAndPasswordResult enum", verifyResult));
            }
            return user;
        }
        public DataGridModel LoadUsersByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            var users = _userLogic.GetUsersByFilter(dataGridFilterModel, out count);
            var usermodel = GetUserModelList(users).ToArray();

            var dataGridModel = new DataGridModel();
            dataGridModel.Advisors = usermodel;
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }
        public IList<UserModel> GetUserModelList(IEnumerable<User> users)
        {
            if (users == null)
                throw new ArgumentNullException("users");

            var modelUser = new List<UserModel>();
            foreach (var user in users)
            {
                var ma = new UserModel
                {
                    Id = user.Id,
                    Name = user.Details != null ? user.Details.FirstName + (user.Details.LastName != null ? user.Details.LastName : " ") : null,
                    Login = user.Login,
                    RepCodesCount = user.RepCodesCount,
                    PrimaryEmailId = user.PrimaryEmailId,
                    PrimaryPhoneNumber = user.PrimaryPhoneNumber,
                    Status = user.Status
                };
                modelUser.Add(ma);
            }
            return modelUser;
        }

        public void SaveUserInfo(UserEntityModel userModel, out int modelCount, HttpRequest request, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            UserEmailModel primaryEmail = null;
            UserPhoneModel primaryPhoneNumber = null;
            User user;
            bool isNewUser = false;
            if (userModel.Id != 0)
                user = _userLogic.GetUserById(userModel.Id);
            else
            {
                user = _userLogic.CreateUserEntity();
                user.Login = userModel.Login;
                isNewUser = true;
                if (user.Models == null)

                    user.Models = new List<AdvisorModel>();
                if (user.Emails == null)
                    user.Emails = new List<UserEmail>();
                if (user.PhoneNumbers == null)
                    user.PhoneNumbers = new List<UserPhone>();

            }
            modelCount = 0;
            if (user != null)
            {
                user.Status = userModel.Status;
                if (user.Details == null)
                    user.Details = new UserDetails();

                user.Details.FirstName = userModel.Details.FirstName;
                user.Details.LastName = userModel.Details.LastName;
                user.Details.MiddleName = userModel.Details.MiddleName;
                user.Details.User = user;


                if (userModel.Models != null)
                {
                    //remove models
                    var oldModelList = user.Models;
                    for (var i = 0; i < oldModelList.Count; i++)
                    {
                        var model = userModel.Models.FirstOrDefault(o => o.Id == oldModelList[i].Model.Id);
                        if (model != null)
                        {
                            userModel.Models.Remove(model);
                        }
                        else
                        {
                            _modelLogic.CheckAccountandAdvisorDependency(oldModelList[i].Model, userModel.Id);
                            oldModelList.RemoveAt(i);
                            i--;
                        }
                    }

                    //add models
                    foreach (var model in userModel.Models)
                    {
                        var UserModel = _userLogic.CreateModel();

                        UserModel.Advisor = user;
                        if (model == null)
                            throw new ZegaServiceException(string.Format("Model not found (Id: {0}).", model.Id));

                        UserModel.Model = new Model

                        {
                            Id = model.Id,
                            Name = model?.Name ?? null,
                            Description = model?.Description ?? null,
                            ModelSleeves = model.ModelSleeves != null ? model.ModelSleeves.Select(o => new ModelSleeve
                            {
                                Id = o.Id,
                                Allocation = o.Sleeve.Allocation,
                                Sleeve = new Sleeve
                                {
                                    Name = o.Sleeve.Name,
                                    Id = o.Sleeve.Id,
                                    Description = o.Sleeve.Description
                                }
                            }).ToList() : null,
                        };
                        user.Models.Add(UserModel);
                    }
                }

                if (userModel.Emails != null)
                {
                    primaryEmail = userModel.Emails?.FirstOrDefault(o => o.IsPrimary);
                    //remove emails
                    var oldEmailList = user.Emails;
                    for (var i = 0; i < oldEmailList.Count; i++)
                    {
                        var email = userModel.Emails.FirstOrDefault(o => o.Id == oldEmailList[i].Id);
                        if (email != null)
                            userModel.Emails.Remove(email);
                        else
                        {
                            oldEmailList.RemoveAt(i);
                            i--;
                        }
                    }

                    //add emails
                    foreach (var email in userModel.Emails)
                    {
                        var UserEmail = _userLogic.CreateEmail();

                        if (email.Email == null)
                            throw new ZegaServiceException(string.Format("Email not found (Id: {0}).", email.Id));
                        UserEmail.User = user;
                        UserEmail.Email = email.Email;
                        UserEmail.IsPrimary = email.IsPrimary;
                        user.Emails.Add(UserEmail);
                    }
                }
                if (userModel.PhoneNumbers != null)
                {
                    primaryPhoneNumber = userModel.PhoneNumbers.FirstOrDefault(o => o.IsPrimary);
                    //remove  phone numbers
                    var oldPhoneList = user.PhoneNumbers;
                    for (var i = 0; i < oldPhoneList.Count; i++)
                    {
                        var phoneNumbers = userModel.PhoneNumbers.FirstOrDefault(o => o.Id == oldPhoneList[i].Id);
                        if (phoneNumbers != null)
                            userModel.PhoneNumbers.Remove(phoneNumbers);
                        else
                        {
                            oldPhoneList.RemoveAt(i);
                            i--;
                        }
                    }

                    //add phone numbers
                    foreach (var phoneNumber in userModel.PhoneNumbers)
                    {

                        var userPhoneNumber = _userLogic.CreatePhoneNumber();
                        if (phoneNumber.PhoneNo == null)
                            throw new ZegaServiceException(string.Format("Phone number not found (Id: {0}).", phoneNumber.Id));
                        userPhoneNumber.User = user;
                        userPhoneNumber.PhoneNo = phoneNumber.PhoneNo;
                        userPhoneNumber.IsPrimary = phoneNumber.IsPrimary;
                        userPhoneNumber.CountryCode = phoneNumber.CountryCode;
                        user.PhoneNumbers.Add(userPhoneNumber);
                    }
                }
                AddOrRemoveRepcode(user, userModel);
                if (primaryEmail == null)
                    throw new ZegaServiceException("User Primary Email is Required !");
                if (_userLogic.IsPrimaryEmailValid(primaryEmail.Email, userModel.Id))
                    throw new ZegaServiceException("You Can't use this email As primary ,It is occupied by other user!");
                if (primaryPhoneNumber == null)
                    throw new ZegaServiceException("User Primary Phone Number is Required !");
                if(_userLogic.IsPrimaryPhoneNumberValid(primaryPhoneNumber.PhoneNo ,userModel.Id))
                    throw new ZegaServiceException("You Can't use this Phone number as Primary,It is occupied by other user!");
                foreach (var phone in user.PhoneNumbers)
                    phone.IsPrimary = phone.PhoneNo == primaryPhoneNumber.PhoneNo && phone.CountryCode == primaryPhoneNumber.CountryCode ? true : false;
                foreach (var email in user.Emails)
                    email.IsPrimary = email.Email == primaryEmail.Email ? true : false;
                var token = GenerateTemparyToken();
                if (isNewUser)
                    user.TempPassword = PasswordEncoder.EncryptPassword(token);
                _userLogic.Persist(user);
                if (isNewUser)
                {
                    try
                    {
                        var encryptKey = _configuration["EncryptionKey"];
                        if (encryptKey == null)
                            throw new ZegaServiceException("Encryption Key Not Found !");
                        var url = string.Format("{0}://{1}/resetpassword?login={2}&token={3}", request.Scheme, request.Host, StringEncoderDecoder.Encrypt(user.Login, encryptKey), token);
                        var messege = string.Format("Hello {0},<br /><br />We are glad to have you on COMPASS by Zega Financial.<br /><br /> Please click <a  href='{1}' target='_blank'>Here</a> to set the password for your UserId : {2} .<br /><br /> ", user.Details.FirstName, url, user.Login);
                        _userLogic.SendMail(primaryEmail.Email, messege, "Create Password");
                    }
                    catch (Exception ex)
                    {
                        throw new ZegaServiceException(ex.Message);
                    }
                }
                if (userModel.Models != null)
                    modelCount = userModel.Models.Count;
            }
            else
                throw new Exception(string.Format("Advisor Not Found :{0}", userModel.Id));
        }
        private void AddOrRemoveRepcode(User user, UserEntityModel userModel)
        {
            if (user.RepCodes == null)
                user.RepCodes = new List<AdvisorRepcode>();
            //Remove RepCodes
            if (userModel.RepCodes != null)
            {
                var oldRepcodeList = user.RepCodes;
                for (int i = 0; i < oldRepcodeList.Count; i++)
                {
                    var repCodemodel = userModel.RepCodes.FirstOrDefault(o => o.Id == oldRepcodeList[i].RepCode.Id);
                    if (repCodemodel != null)
                        userModel.RepCodes.Remove(repCodemodel);
                    else
                    {
                        var repCode = oldRepcodeList[i].RepCode;
                        _userLogic.CheckAccountDependency(repCode ,user);
                         oldRepcodeList.RemoveAt(i);
                        i--;
                    }
                }
                // Add RepCodes
                foreach (var repCodemodel in userModel.RepCodes)
                {
                    var repcode = _userLogic.GetRepCodeById(repCodemodel.Id);
                    var advisorRepcode = _userLogic.CreateAdvisorRepCodeEntity();
                    if (advisorRepcode != null)
                    {
                        advisorRepcode.Advisor = user;
                        advisorRepcode.RepCode = repcode;
                        user.RepCodes.Add(advisorRepcode);
                    }
                }
            }
        }
        public DataGridModel LoadRepCodesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext )
          {
            CheckUserContext(userContext);
            int count;
            var repCodes = _userLogic.GetRepCodesByFilter(dataGridFilterModel, out count);
            var repCodesModelList = GetRepCodeModelList(repCodes);
            var dataGridModel = new DataGridModel();
            dataGridModel.RepCodes = repCodesModelList.ToArray();
            dataGridModel.TotalRecords = count;
            return dataGridModel;
        }

        private IList<RepCodeModel> GetRepCodeModelList(IEnumerable<RepCode> repCodes)
        {
            var repCodeModelList = new List<RepCodeModel>();
            foreach (var repCode in repCodes)
            {
                var rc = Map(repCode, new RepCodeModel() {
                    Id = repCode.Id,
                    Code = repCode.Code,
                    Type = repCode.Type
                });
                repCodeModelList.Add(rc);
            }
            return repCodeModelList;

        }
        public void DeleteUsersByIds(int[] Ids, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            foreach (var id in Ids)
            {
                var user = _userLogic.GetUserById(id);
                if (user == null)
                    throw new ZegaServiceException("User not found !");
                if(_accountDao.CheckDependentAccountsbyAdvisor(user.RepCodes.Select( o => o.RepCode.Id).ToList()))
                    throw new ZegaServiceException(string.Format("Advisor Can't be delete, Advisor {0} Exist in Accounts", user.UserName));
                _userLogic.DeleteUsersById(id);
            }
        }
        
        public UserViewModel GetUserById(int id, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (id == 0)
                throw new ZegaServiceException(string.Format("InValid User Id:{0}", id));
            var user = _userLogic.GetUserById(id);
            if (user == null)
                throw new ZegaServiceException("User Doesn't exists");
            var userViewModel = Map(user, new UserViewModel {

                Login = user.Login,
                Status = user.Status,
                IsAdmin = user.IsAdmin,
                RepCodes = user.RepCodes?.Select(o => new RepCodeModel {
                    Id = o.RepCode?.Id ?? 0,
                    Code = o.RepCode?.Code,
                    Type = o.RepCode?.Type ?? 0
                }).ToList(),

                Models = user.Models?.Select(o => new AdvisorModelModel {
                    Id = o.Model.Id,
                    Name = o.Model?.Name,
                    Description = o.Model?.Description,
                    ModelSleeves = o.Model?.ModelSleeves?.Select(o => new ModelSleeveModel {
                        Id = o.Id,
                        Sleeve = new SleeveModel {
                            Name = o.Sleeve?.Name,
                            Description = o.Sleeve?.Description,
                            Allocation = o.Allocation

                        }
                    }).ToList()
                }).ToList(),

                PhoneNumbers = user.PhoneNumbers?.Select(o => new UserPhoneModel {
                    Id = o.Id,
                    IsPrimary = o.IsPrimary,
                    PhoneNo = o.PhoneNo,
                    CountryCode = o.CountryCode,
                }).ToList(),

                Emails = user.Emails?.Select(o => new UserEmailModel {
                    Id = o.Id,
                    IsPrimary = o.IsPrimary,
                    Email = o.Email
                }).ToList(),

                Details = user.Details != null ? new UserDetailsModel {
                    Id = user.Details.Id,
                    FirstName = user.Details.FirstName,
                    LastName = user.Details.LastName,
                    MiddleName = user.Details.MiddleName,
                    Company = user.Details.Company,
                    Designation = user.Details.Designation,
                    Image = user.Details.Image,
                    Name = user.Details.FirstName + user.Details.LastName
                } : null,

                PrimaryEmailId = user?.PrimaryEmailId,
                PrimaryPhoneNumber = user?.PrimaryPhoneNumber


            });
            return userViewModel;
        }

        public SettingsModel GetSettingsData(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var user = _userLogic.GetUserByLogin(userContext.Login);
            if (user == null)
                return new();
            SettingsModel setting = new()
            {
                FirstName = user.Details != null ? user.Details.FirstName : string.Empty,
                Lastname = user.Details != null ? user.Details.LastName : string.Empty,
                Email = user.Emails != null ? user.Emails.FirstOrDefault(o => o.IsPrimary)?.Email : string.Empty,
                PhoneNo = user.PhoneNumbers != null ? user.PhoneNumbers.FirstOrDefault(o => o.IsPrimary)?.PhoneNo : string.Empty,
                CountryCode = user.PhoneNumbers != null ? user.PhoneNumbers.FirstOrDefault(o => o.IsPrimary)?.CountryCode : string.Empty,
                Company = user.Details != null ? user.Details.Company : string.Empty,
                Designation = user.Details != null ? user.Details.Designation : string.Empty,
            };
            return setting;

        }

        public void SaveSettings(SettingsModel settings, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var user = _userLogic.GetUserByLogin(userContext.Login);
            if (user != null)
            { if (user.Details == null)
                    user.Details = new UserDetails();
                user.Details.User = user;
                if (settings.IsPasswordChanged)
                {
                    if (string.IsNullOrEmpty(settings.CurrentPassword) || user.Password != PasswordEncoder.EncryptPassword(settings.CurrentPassword))
                        throw new Exception("Empty or Incorrect Current Password !");
                    user.Password = PasswordEncoder.EncryptPassword(settings.Password);
                }
                else {
                    user.Details.FirstName = settings.FirstName;
                    user.Details.LastName = settings.Lastname;
                    user.Details.Company = settings.Company;
                    user.Details.Designation = settings.Designation;
                    if (settings.Email != null)
                    {
                        if (_userLogic.IsPrimaryEmailValid(settings.Email, user.Id))
                            throw new ZegaServiceException("Other User Already Registerd This Email As Primary Email!");
                        var IsNewPrimaryEmailExistInExistingEmailList = user.Emails != null ? user.Emails.Any(o => o.Email == settings.Email) : false; 
                        if(!IsNewPrimaryEmailExistInExistingEmailList)
                            user.Emails.Add(new UserEmail() { Email = settings.Email, IsPrimary = true, User = user });
                        foreach (var email in user.Emails)
                            email.IsPrimary = email.Email == settings.Email ? true : false;
                    }
                    if (settings.PhoneNo != null)
                    {
                        if (_userLogic.IsPrimaryPhoneNumberValid(settings.PhoneNo, user.Id))
                            throw new ZegaServiceException("Other User Already Registerd this phone number as Primary Phone number!");
                        var isNewPrimaryPhoneExistInPhoneNumberList = user.PhoneNumbers != null ? user.PhoneNumbers.Any(o => o.PhoneNo == settings.PhoneNo) : false;
                        if (!isNewPrimaryPhoneExistInPhoneNumberList)
                            user.PhoneNumbers.Add(new UserPhone() { PhoneNo = settings.PhoneNo, IsPrimary = true, User = user, CountryCode = settings.CountryCode });
                        foreach (var phone in user.PhoneNumbers)
                        {
                            if (phone.PhoneNo == settings.PhoneNo)
                            {
                                phone.IsPrimary = true;
                                phone.CountryCode = settings.CountryCode;
                                phone.PhoneNo = settings.PhoneNo;
                            }
                            else
                                phone.IsPrimary = false;
                        }
                    }
                }
                _userLogic.Persist(user);
            }
        }

        public void UploadSettingsImg(ImageDataModel image, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var user = _userLogic.GetUserByLogin(userContext.Login);

            if (user == null)
            {
                throw new ZegaServiceException("User Details not found.");
            }

            user.Details.Image = image.RawData;
            _userLogic.Persist(user);
        }

        public ImageDataModel GetSettingsImg(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var user = _userLogic.GetUserByLogin(userContext.Login);

            return user == null ? new ImageDataModel { RawData = null } : new ImageDataModel
            {
                RawData = user.Details?.Image,
            };
        }

        public void BulkEditUsers(BulkEditModel bulkChanges, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (!bulkChanges.DataStoreIds.Any())
                return;
            var users = _userLogic.GetUsersByIds(bulkChanges.DataStoreIds.ToArray());
            foreach (var user in users)
            {
                user.Status = bulkChanges.Status;
            }
            _userLogic.Persist(users);
        }

        public string SendForgetPasswordEmail(string userEmail, HttpRequest request)
        {
            try
            {
                var userInfo = _userLogic.GetUserByPrimaryEmail(userEmail);
                if (userInfo != null)
                {
                    if (userInfo.Status == Status.InActive)
                        throw new ZegaServiceException("Your Account is deactivate. Please Contact to administrator");
                    return ForgotPasswordEmail(userInfo, request);
                }
                else
                {
                    throw new ZegaServiceException("No user registered with this as Primary Email!");
                }
            }
            catch (Exception ex)
            {
                throw new ZegaServiceException("Error in sending forgot password email : - " + ex.Message);
            }
        }

        private string ForgotPasswordEmail(User userInfo, HttpRequest request)
        {
            try
            {
                var primaryEmail = userInfo.Emails.FirstOrDefault(o => o.IsPrimary).Email;
                if (string.IsNullOrEmpty(primaryEmail))
                    throw new ZegaServiceException("No User Registered with this Email !");
                var encryptKey = _configuration["EncryptionKey"];
                if (encryptKey == null)
                    throw new ZegaServiceException("Encryption Key Not Found !");
                var token = GenerateTemparyToken();
                var url = string.Format("{0}://{1}/resetpassword?login={2}&token={3}", request.Scheme, request.Host, StringEncoderDecoder.Encrypt(userInfo.Login, encryptKey), token);
                var message = string.Format("Hi {0},<br /><br /> If you've forgotten your password and wish to reset it click on Reset Password below. <br /><br /><a  href='{1}' target='_blank'>Reset Password. </a><br /><br /> If you did not request a password reset, you can safely ignore this email. <br /> Only a person with access to your email can reset your password.", userInfo.Details.FirstName, url);
                _userLogic.SendMail(primaryEmail, message, "Password Reset ");
                userInfo.TempPassword = PasswordEncoder.EncryptPassword(token);
                _userLogic.Persist(userInfo);
                return "Email Sent Successfully!";
            }
            catch (Exception ex)
            {
                throw new ZegaServiceException("Error in sending forgot password email : - " + ex.Message);
            }
        }

        public bool ResetPassword(string userLogin, string newPassword, string outhToken)
        {
            var encryptKey = _configuration["EncryptionKey"];
            if (encryptKey == null)
                throw new ZegaServiceException("Encryption Key Not Found");
            var login = StringEncoderDecoder.Decrypt(userLogin, encryptKey);
            var userInfo = _userLogic.GetUserByLogin(login);
            if (userInfo == null)
                throw new ZegaServiceException("User not Found !");
            if (userInfo.TempPassword == null)
                throw new ZegaServiceException("Reset Password Link has been Expired!");
            if (string.IsNullOrEmpty(outhToken) || PasswordEncoder.EncryptPassword(outhToken) != userInfo.TempPassword)
                throw new ZegaServiceException("User authentication failed ! ");
            if (string.IsNullOrEmpty(newPassword))
                throw new ZegaServiceException("Password Can't be blank !");
            userInfo.TempPassword = null;
            userInfo.Password = PasswordEncoder.EncryptPassword(newPassword);
            var isReseted = _userLogic.ResetPassword(userInfo);

            return isReseted;
        }
        private string GenerateTemparyToken()
        {
            return new Password(16).IncludeNumeric().IncludeUppercase().IncludeLowercase().Next();
        }

        public LoginActivityModel GetLoginActivity(int userId, string sessionId)
        {
            var loginActivityModel = new LoginActivityModel();
            var loginActivity = _userLogic.GetLoginActivity(userId,sessionId);
            if (loginActivity != null)
                Map(loginActivity, loginActivityModel);
            return loginActivityModel;
        }

        public void DeleteLoginActivity(string Sessionid)
        {
            _userLogic.DeleteLoginActivity(Sessionid);
        }

        public void SaveLoginActivity(int userId, string clientIp, string sessionId)
        {
            var activity = new LoginActivity();
            var user = _userLogic.GetUserById(userId);
            if (user == null)
                throw new ZegaServiceException("User Not Found !");
            activity.User = user;
            activity.IPAddress = clientIp;
            activity.SessionId = sessionId;
            _userLogic.SaveLoginActivity(activity);
        }

        public void CheckUserPermissionForLogin(UserContextModel userContext)
        {
            CheckUserContext(userContext);
        }

        public void SaveRepCode(RepCodeModel repCodeModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            RepCode repCode ;
            if (repCodeModel.Id != 0)
            {
                repCode = _userLogic.GetRepCodeById(repCodeModel.Id);
                var accounts = _repCodeDao.GetDependentAccountsbyRepCodeId(repCode.Id);
                if (accounts != null && accounts.Any() && !repCode.Code.Equals(repCodeModel.Code))
                     throw new ZegaServiceException("RepCode can't be update,it used in accounts.");
            }
            else
                repCode = new RepCode();

            if (repCode != null)
            {
                repCode.Id = repCodeModel.Id;
                repCode.Type = repCodeModel.Type;
                repCode.Code = repCodeModel.Code; 
                if (_userLogic.IsRepCodeExist(repCode))
                    throw new ZegaServiceException("RepCode Already Exist!");
                _userLogic.Persist(repCode);
            }
           
        }

        public void DeleteRepCodeByIds(int[] ids, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (ids == null)
                throw new ZegaServiceException("No RepCode Selected !");
            foreach(var id in ids)
            {
                var repcode = _userLogic.GetRepCodeById(id);
                if(repcode != null)
                {
                    _userLogic.CheckAdvisorDependency(repcode);
                    _userLogic.CheckAccountDependency(repcode);
                    _userLogic.DeleteRepCodeById(repcode.Id);
                }
                else
                   throw new ZegaServiceException(string.Format("RepCode not Found Id :{0})",id));
            }
        }
        public string GetJwtAuthToken(string login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretkey = _configuration["SecretKey"];
            if (string.IsNullOrEmpty(secretkey))
                throw new ZegaServiceException("Jwt Secret key not found.");
            var key = Encoding.ASCII.GetBytes(secretkey);
            var tokenDescritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, login)
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescritor);
            return tokenHandler.WriteToken(token);
        }  
    }
}
