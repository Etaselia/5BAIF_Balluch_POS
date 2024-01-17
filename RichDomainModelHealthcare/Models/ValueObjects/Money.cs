namespace RichDomainModelHealthcare.Models.ValueObjects {
    public class Money {
        public Guid MoneyId { get; set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public Money(decimal amount, string currency) {
            Amount = amount;
            Currency = currency;
        }

        // Override ToString, Equals, and GetHashCode as needed
    }
}
