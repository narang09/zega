using NHibernate;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Users
{
    public class UserDao : NHibernateDao<User>, IUserDao
    {
        public UserDao(ISession session) : base(session) { }
      
        public User GetByLoginId(string login)
        {
            var sql = @"
	FROM User AS u
	WHERE u.Login = :login";

            return _session.CreateQuery(sql)
                .SetParameter("login", login)
                .UniqueResult<User>();
        }

        public IEnumerable<User> GetUsersByFilter(DataGridFilterModel model, out int count)
        {
            var sql = @"
	SELECT usr, (SELECT COUNT(*) From AdvisorRepcode ar GROUP BY ar.Advisor HAVING ar.Advisor = usr.Id),
    (SELECT CONCAT(ud.FirstName, ud.LastName) AS Name FROM UserDetails AS ud WHERE usr.Id = ud.User),
    (SELECT ue.Email FROM UserEmail ue WHERE ue.User = usr.Id and ue.IsPrimary = 1)
	FROM User AS usr WHERE 1=1";

            var qg = new QueryGenerator("usr", model, new []
                {
                new QueryReplace("RepCodesCount", @"(SELECT COUNT(*) From AdvisorRepcode ar GROUP BY ar.Advisor HAVING ar.Advisor = usr.Id)"),
                new QueryReplace("StatusValue", "usr.Status"), 
                new QueryReplace("Name", @"(SELECT CONCAT(ud.FirstName, ud.LastName) AS Name From UserDetails AS ud where usr.Id = ud.User)"), 
                new QueryReplace("PrimaryEmailId", @"(SELECT ue.Email FROM UserEmail ue WHERE ue.User = usr.Id and ue.IsPrimary = 1)")
            }, new Dictionary<string, string> { { "StatusValue", "Status"} });
            var qb = qg.ApplyBasicMultiFilters<object[]>(_session, sql);
            var users = qb.GetResult<object[]>(0);

            count = (int)qb.GetResult<long>(1).Single();
            return ConvertToUsers(users.ToList());
        }

        private static List<User> ConvertToUsers(IList<object[]> list)
        {
            var result = new List<User>();
            if (list == null || !list.Any())
                return result;
            
            for (var i = 0; i < list.Count; i++)
            {               
                var currentUser = (User)list[i][0];
                currentUser.RepCodesCount = Convert.ToInt32(list[i][1]);
                currentUser.Name = Convert.ToString(list[i][2]);
                currentUser.PrimaryEmailId = Convert.ToString(list[i][3]);
                result.Add(currentUser);
            }

            return result;
        }
        public RepCode GetRepCodeByCode(string repCode)
        {
            var sql = @"
	       FROM RepCode 
	       WHERE Code = : rCode";

            return _session.CreateQuery(sql)
                .SetParameter("rCode", repCode)
                .UniqueResult<RepCode>();
        }
        public RepCode GetRepCodeById(int repCodeId)
        {
            var sql = @"
	       FROM RepCode 
	       WHERE Id = :repCodeId";

            return _session.CreateQuery(sql)
                .SetParameter("repCodeId", repCodeId)
                .UniqueResult<RepCode>();
        }
        public virtual bool CheckLoginExist(string login, int userId)
        {
            var sql = @"
	SELECT count(*)
	FROM User AS u
	WHERE u.Login = :login AND u.Id <> :userId";

            var query = _session.CreateQuery(sql)
                .SetParameter("login", login)
                .SetParameter("userId", userId)
                .UniqueResult<long>();

            return query > 0;
        }
        public IEnumerable<User> GetUsersByIds(int[] userIds)
        {
            if (userIds == null || !userIds.Any())
                return new User[0];
            var sql = @"
	            FROM User 
	            WHERE Id IN (:userIds)";

            return _session.CreateQuery(sql)
                .SetParameterList("userIds", userIds).List()
                .Cast<User>();

        }


        public User GetByPrimaryEmail(string email)
        { 
            var sql= @"From UserEmail as e Where e.Email =:email and e.IsPrimary=1" ;
            var userEmail = _session.CreateQuery(sql).SetParameter("email",email).SetMaxResults(1).List<UserEmail>().FirstOrDefault();
            return userEmail?.User;
            
        }
        public bool IsPrimaryEmailValid(string primaryEmail, int userId)
        {
            var sql = @"SELECT 1 FROM UserEmail WHERE User <>:userId ANd Email = :primaryEmail AND IsPrimary = true";
            var query = _session.CreateQuery(sql)
                .SetParameter("primaryEmail", primaryEmail).SetParameter("userId", userId).List<int>();
            return query.Any();
        }

        public IEnumerable<RepCode> GetRepCodesByAdvisorId(int advisorId)
        {
            var sql = @"Select RepCode From AdvisorRepcode  Where Advisor =: advisorId";
            var advisors = _session.CreateQuery(sql).SetParameter("advisorId", advisorId).List<RepCode>();
            return advisors;
        }

        public IEnumerable<User> GetAllAdvisors()
        {
            var sql = @"
	            FROM User 
	            WHERE IsAdmin != 1 ";
            var advisors = _session.CreateQuery(sql).List<User>();
            if (advisors == null)
                return new List<User>();
            return advisors;
        }
        public IEnumerable<AdvisorRepcode> GetAllRepCodeAdvisors()
        {
            var sql = @"FROM AdvisorRepcode";
            var advisors = _session.CreateQuery(sql).List<AdvisorRepcode>();
            return advisors != null && advisors.Any() ? advisors : new List<AdvisorRepcode>(); 
        }
        public bool IsPrimaryPhoneNumberValid(string phoneNo, int userId)
        {
            var sql = @"SELECT 1 FROM UserPhone WHERE User <>:userId ANd PhoneNo = :phoneNo AND IsPrimary = true";
            var query = _session.CreateQuery(sql)
                .SetParameter("phoneNo", phoneNo).SetParameter("userId", userId).List<int>();
            return query.Any();
        }

        public void CheckAdvisorDependency(RepCode repCode)
        {
            var sql = @"Select Advisor From AdvisorRepcode where RepCode =:repcodeId";
            var advisors = _session.CreateQuery(sql)
                .SetParameter("repcodeId", repCode.Id).List<User>();
            if(advisors != null && advisors.Any())
            {
                var advisorName = string.Join(",",advisors.Select(o => o.Details != null
                        ? o.Details.FirstName + (o.Details.LastName != null ? o.Details.LastName : "") : "").ToList());
                throw new ZegaDaoException(string.Format("RepCode :{0}, can't be delete, it used by Advisors :{1}", repCode.Code, advisorName));

            }

        }
    }
}
