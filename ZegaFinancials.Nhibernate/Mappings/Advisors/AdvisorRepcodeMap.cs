using ZegaFinancials.Nhibernate.Entities.Advisors;

namespace ZegaFinancials.Nhibernate.Mappings.Advisors
{
    public class AdvisorRepcodeMap : ZegaMap<AdvisorRepcode>
    {
        public AdvisorRepcodeMap()
        {
            References(x => x.Advisor).Column("Advisor").ForeignKey("FK_AdvisorRepCode_User").Index("IDX_AdvisorRepcode_User");
            References(x => x.RepCode).Column("RepCode").ForeignKey("FK_AdvisorRepCode_RepCode").Index("IDX_AdvisorRepCode_RepCode");
        }
    }
}
