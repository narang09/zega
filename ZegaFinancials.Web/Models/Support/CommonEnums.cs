using System.ComponentModel;

namespace ZegaFinancials.Web.Models.Support
{
    public enum ZegaResponseCodes
    {
        [Description("User already logged in on another machine")]
        AlreadyLoggedIn = 1001,
        [Description("User is not logged in")]
        NotLoggedIn = 401,

    }
}
