using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class PaymentViewModel
    {
        public List<BalanceHistory> Payments { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal PaymentAmount { get; set; }
    }
}
