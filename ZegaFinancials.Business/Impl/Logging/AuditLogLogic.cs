
using System;
using System.Linq;
using System.Collections.Generic;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Nhibernate.Dao.Interface.Logging;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Impl.Logging
{
    public class AuditLogLogic : ZegaLogic, IAuditLogLogic
    {
        private readonly IAuditLogDao _auditLogDao;
        private readonly IUserDao _userDao;

        public AuditLogLogic(IAuditLogDao auditLogDao, IUserDao userDao)
        {
            _auditLogDao = auditLogDao;
            _userDao = userDao;
        }

        public IEnumerable<AuditLog> GetAuditLogByFilter(DataGridFilterModel model, string userLogin, bool isAdmin, out int count)
        {
            if (!isAdmin && userLogin == null)
            {
                count = 0;
                return new List<AuditLog>();
            }
            var auditLog = _auditLogDao.GetAuditLogByFilter(model, userLogin, isAdmin ,out count);
            return auditLog;
        }
        public virtual void Log(EntityType type, string message)
        {
            AuditLog log = new();
            log.EntityType = type;
            log.Message = message;
            _auditLogDao.Persist(log);
        }
        public virtual void Log(EntityType type, string message, int userId)
        {
            AuditLog log = new();
            log.EntityType = type;
            log.Message = message;
            log.Date = DateTime.Now;
            log.UserLogin = _userDao.GetUsersByIds(new int[] { userId }).FirstOrDefault()?.Login;
            _auditLogDao.Persist(log);
        }

    }
}
