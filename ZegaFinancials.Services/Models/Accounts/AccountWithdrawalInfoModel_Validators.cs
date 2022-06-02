using FluentValidation;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountWithdrawalInfoModel_Validators: ZegaValidator<AccounWithdrawlInfoModelcs>
    {
        public AccountWithdrawalInfoModel_Validators()
        {
            RuleFor(o => o.Withdrawl_Amount).GreaterThan(0m).LessThanOrEqualTo(1e10m).WithMessage("Withdrawal Amount should be less or equal to 10 billions.");
        }
    }
}
