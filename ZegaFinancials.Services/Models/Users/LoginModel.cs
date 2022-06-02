namespace ZegaFinancials.Services.Models.Users
{
    public partial class LoginModel : ZegaModel
    {
		public virtual string Login { get; set; }
		public virtual string Password { get; set; }
		public virtual string ClientId { get; set; }
		public virtual bool IsForceLogin { get; set; }

	}
}
