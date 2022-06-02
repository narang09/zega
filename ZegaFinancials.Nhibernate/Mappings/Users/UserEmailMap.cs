using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Mappings.Users
{
    public class UserEmailMap : ZegaMap<UserEmail>
    {
        public UserEmailMap()
        {
            References(x => x.User).Column("`User`").ForeignKey("FK_UserEmail_User").Index("IDX_UserEmail_User");
            Map(x => x.Email).Length(100);
            Map(x => x.IsPrimary);
        }
    }
}
