namespace Loan.Api.Model
{
    public class ErrorModel
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Details { get; set; }
    }
}
