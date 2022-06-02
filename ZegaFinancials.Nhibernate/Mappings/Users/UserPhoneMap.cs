using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Mappings.Users
{
    public class UserPhoneMap : ZegaMap<UserPhone>
    {
        public UserPhoneMap()
        {
            References(x => x.User).Column("`User`").ForeignKey("FK_User_UserPhone").Index("IDX_User_UserPhone");
            Map(x => x.CountryCode).Length(5);
            Map(x => x.PhoneNo).Length(25);
            Map(x => x.IsPrimary);
        }
    }
}
