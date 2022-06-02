using ZegaFinancials.Nhibernate.Entities.Models;

namespace ZegaFinancials.Nhibernate.Mappings.Models
{
    public class ModelSleeveMap : ZegaMap<ModelSleeve>
    {
        public ModelSleeveMap()
        {
            References(x => x.Model).Column("Model").ForeignKey("FK_Model_ModelSleeve").Index("IDX_Model_ModelSleeve");
            References(x => x.Sleeve).Column("Sleeve").ForeignKey("FK_ModelSleeve_Sleeve").Index("IDX_ModelSleeve_Sleeve");
            Map(x => x.Allocation);
        }
    }
}
