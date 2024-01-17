using RichDomainModelHealthcare.Models.ValueObjects;
using System.Linq;

namespace RichDomainModelHealthcare.Models {
    public class Invoice {
        public Guid InvoiceId { get; set; }
        public Patient Patient { get; set; }
        public DateTime DateIssued { get; set; }
        public Money TotalAmount { get; set; }
        public Treatment Treatment { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        
        // ... Additional properties and methods
        public Invoice() {
            
        }
        
        public void AddInvoice(Invoice invoice) => Invoices.Add(invoice);
    }
}
