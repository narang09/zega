

namespace ZegaFinancials.Services.Models.Users
{
    public class SettingsModel : ZegaModel
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNo { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
        public bool IsPasswordChanged { get; set; }
    }
}
