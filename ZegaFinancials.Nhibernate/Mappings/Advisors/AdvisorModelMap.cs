using ZegaFinancials.Nhibernate.Entities.Advisors;

namespace ZegaFinancials.Nhibernate.Mappings.Advisors
{
    public class AdvisorModelMap : ZegaMap<AdvisorModel>
    {
        public AdvisorModelMap()
        {
            References(x => x.Advisor).Column("Advisor").ForeignKey("FK_AdvisorModel_User").Index("IDX_AdvisorModel_User");
            References(x => x.Model).Column("Model").ForeignKey("FK_AdvisorModel_Model").Index("IDX_AdvisorModel_Model");
        }

    }
}
