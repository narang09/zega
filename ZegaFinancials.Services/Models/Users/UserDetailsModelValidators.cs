
using FluentValidation;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserDetailsModelValidators: ZegaValidator<UserDetailsModel>
    {
        public UserDetailsModelValidators()
        {
            RuleFor(p => p.FirstName).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("FirstName should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
            RuleFor(p => p.LastName).Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("LastName should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
        }
    }
}
