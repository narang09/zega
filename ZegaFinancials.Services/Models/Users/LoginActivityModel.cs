

namespace ZegaFinancials.Services.Models.Users
{
    public class LoginActivityModel :ZegaModel
    {
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
    }
}
