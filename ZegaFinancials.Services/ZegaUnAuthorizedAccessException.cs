
namespace ZegaFinancials.Services
{
    public class ZegaUnAuthorizedAccessException
    {
        public string Reason { get; set; }
        public ZegaUnAuthorizedAccessException()
        { }
        public ZegaUnAuthorizedAccessException(string reason)
        {
            Reason = reason;
        }
    }
}
