using System;

namespace ZegaFinancials.Web.Models.Users
{
    public class UserInfoModel
    {
        public int Id { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }
        public bool IsSuccessfullLogin { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAdmin { get; set; }

    }
}
