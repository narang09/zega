
using ZegaFinancials.Nhibernate.Entities.Models;

namespace ZegaFinancials.Nhibernate.Mappings.Models
{
    public class BlendModelDetailsMap :ZegaMap<BlendModelDetails>
    {

        public BlendModelDetailsMap()
        {
            References(x => x.Model).Column("Model").ForeignKey("FK_BlendModelDetails_Model").Index("IDX_BlendModelDetails_Model");
            References(x => x.SubModel).Column("SubModel").ForeignKey("FK_BlendModelDetails_SubModel").Index("IDX_BlendModelDetails_SubModel");
            Map(x => x.Allocation);
        }
    }
}
