using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Mappings.Logging
{
    public class LoginActivityMap : ZegaMap<LoginActivity>
    {
        public LoginActivityMap()
        {
            References(x => x.User).Column("`User`").ForeignKey("FK_LoginActivity_User").Index("IDX_LoginActivity_User");
            Map(x => x.IPAddress).Length(100);
            Map(x => x.SessionId);
        }
    }
}
