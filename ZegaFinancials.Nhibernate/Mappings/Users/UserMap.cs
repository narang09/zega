using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Mappings.Users
{
    public class UserMap : ZegaMap<User>
    {
        public UserMap()
        {          
            HasOne(x => x.Details).PropertyRef("User").Cascade.All();
            Map(x => x.Login).Length(255);
            Map(x => x.Password).Length(255);
            Map(x => x.TempPassword).Length(255);
            Map(x => x.IsAdmin).Default("0");
            Map(x => x.Status).CustomType<Status>();
            HasMany(x => x.PhoneNumbers).Cascade.All();
            HasMany(x => x.Emails).Cascade.All();
            HasMany(x => x.RepCodes).Cascade.AllDeleteOrphan();
            HasMany(x => x.Models).Cascade.AllDeleteOrphan();
            HasMany(x => x.GridConfigs).Cascade.All();
        }
    }
}
