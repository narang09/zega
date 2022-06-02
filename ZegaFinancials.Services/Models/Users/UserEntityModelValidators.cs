using FluentValidation;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserEntityModelValidators : ZegaValidator<UserEntityModel>
    {
        public UserEntityModelValidators()
        {
            RuleFor(p => p.Login).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("Login should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
        }
    }
}
