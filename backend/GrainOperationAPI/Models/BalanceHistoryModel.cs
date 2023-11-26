namespace GrainOperationAPI.Models
{
    public class BalanceHistoryModel
    {
        public int HistoryId { get; set; }
        public int UserId { get; set; }
        public int? TransactionId { get; set; }  // Nullable if not all history records are linked to a transaction
        public decimal ChangeAmount { get; set; }
        public decimal NewBalance { get; set; }
        public string TransactionType { get; set; }
        public string GrainType { get; set; }
        public string GrainClass { get; set; }
        public decimal Weight { get; set; }
        public DateTime TransactionDate { get; set; }

        // Navigation properties
        public UserBalanceModel User { get; set; }
        public TransactionModel Transaction { get; set; } // If applicable
    }

}
