using FluentValidation;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountBasicDetailsModel_Validators: ZegaValidator<AccountBasisDetailsModel>
    {
        public AccountBasicDetailsModel_Validators()
        {
            RuleFor(o => o.ClientName).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("ClientName should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
            RuleFor(o => o.Number).NotEmpty().Must(q => ValidationMethods.DoPenetrationCheck(q)).WithMessage("Number should not start with special symbols  e.g. (=, +, -, @ , etc) and space.").Length(0, 250);
            RuleFor(o => o.CashEq).GreaterThan(0m).LessThanOrEqualTo(1e10m).WithMessage("Cash Equivalent should be less or equal to 10 billions.");
            RuleFor(o => o.CashNetBal).GreaterThan(0m).LessThanOrEqualTo(1e10m).WithMessage("Cash Net Balance should be less or equal to 10 billions.");
        }
    }
}
