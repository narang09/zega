using FluentValidation;

namespace ZegaFinancials.Services.Models.Users
{
    public class AdvisorRepCodeModelValidators: ZegaValidator<AdvisorRepCodeModel>
    {
        public AdvisorRepCodeModelValidators()
        {
            RuleFor(p => p.Code).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("Code should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
        }
    }
}
