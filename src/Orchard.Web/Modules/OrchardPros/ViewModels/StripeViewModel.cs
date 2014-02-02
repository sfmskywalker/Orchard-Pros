using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.ViewModels {
    public class StripeViewModel {
        public Transaction Transaction { get; set; }
        public IUser User { get; set; }
        public int AmountInCents {
            get { return (int) (Transaction.Amount*100); }
        }
    }
}