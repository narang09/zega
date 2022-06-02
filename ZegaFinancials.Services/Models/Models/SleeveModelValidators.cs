using FluentValidation;

namespace ZegaFinancials.Services.Models.Models
{
    public class SleeveModelValidators : ZegaValidator<SleeveModel>
    {
        public SleeveModelValidators()
        {
            RuleFor(p => p.Description).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("Description should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
            RuleFor(p => p.Name).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("Name should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
        }
    }
}
