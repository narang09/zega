using FluentValidation;
namespace ZegaFinancials.Services.Models
{
    public class ZegaValidator<T> : AbstractValidator<T> where T : ZegaModel
    {
    }
}
