namespace ZegaFinancials.Services.Models.Shared
{
    public class UserPermissionModel
    {
        #region System Settings
        public virtual int AutoLogoutIdleTimeout { get; set; }
        public virtual int AutoLogoutWaitTimeout { get; set; }
        public virtual int AutoLogoutTimeout { get; set; }
        #endregion

        public virtual UserContextModel userContext { get; set; }

    }
}
