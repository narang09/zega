using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Mappings.Users
{
    public class UserDetailsMap : ZegaMap<UserDetails>
    {
        public UserDetailsMap()
        {
            References(x => x.User).Column("`User`").ForeignKey("FK_UserDetails_User").UniqueKey("UQ_UserDetails_User").Index("IDX_UserDetails_User").Cascade.All();
            Map(x => x.FirstName).Length(255);
            Map(x => x.MiddleName).Length(255);
            Map(x => x.LastName).Length(255);
            Map(x => x.Company);
            Map(x => x.Designation);
            Map(x => x.Image).CustomSqlType("varbinary(MAX)").Length(int.MaxValue);
        }
    }
}
