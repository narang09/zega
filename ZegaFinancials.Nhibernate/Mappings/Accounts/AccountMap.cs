using ZegaFinancials.Nhibernate.Entities.Accounts;

namespace ZegaFinancials.Nhibernate.Mappings.Accounts
{
    public class AccountMap : ZegaMap<Account>
    {
        public AccountMap()
        {
            Map(x => x.Number).Length(255);
            Map(x => x.Name).Length(255);
            Map(x => x.ClientName).Length(4001);
            References(x => x.Model).Column("Model").ForeignKey("FK_Account_Model").Index("IDX_Account_Model");
            References(x => x.RepCode).Column("RepCode").ForeignKey("FK_Account_RepCode").Index("IDX_Account_RepCode");
            Map(x => x.AccountStatus).CustomType<AccountStatus>();
            Map(x => x.Broker).CustomType<Broker>();
            Map(x => x.isDeleted);
            HasOne(x => x.AccountDetail).Cascade.All().PropertyRef("Account");
           
        }
    }
}
