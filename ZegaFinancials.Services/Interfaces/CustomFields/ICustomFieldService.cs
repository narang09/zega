using ZegaFinancials.Services.Models.CustomFields;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.CustomFields
{
    public interface ICustomFieldService
    {
        CustomFieldModel[] LoadFieldsByEntityType(UserContextModel userContext);
    }
}
