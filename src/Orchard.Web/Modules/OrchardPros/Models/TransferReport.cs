namespace OrchardPros.Models {
    public class TransferReport {
        public int UserId { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }

        public decimal Total {
            get { return TotalPaid + TotalPending; }
        }
    }
}