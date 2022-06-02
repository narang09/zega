using ZegaFinancials.Nhibernate.Entities.Models;

namespace ZegaFinancials.Nhibernate.Mappings.Models
{
    public class ModelMap : ZegaMap<Model>
    {
        public ModelMap()
        {
            Map(x => x.Name).Length(255);
            Map(x => x.Description).Length(4001);
            Map(x => x.IsBlendModel);
            Map(x => x.IsLocalBlend);
            Map(x => x.AccountId);
            HasMany(x => x.ModelSleeves).AsBag().KeyColumns.Add("Model").Cascade.AllDeleteOrphan();
            HasMany(x => x.SubModels).AsBag().KeyColumns.Add("Model").Cascade.AllDeleteOrphan();
        }
    }
}
