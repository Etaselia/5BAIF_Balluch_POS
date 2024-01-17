using RichDomainModelHealthcare.Interfaces;
using RichDomainModelHealthcare.Models;

namespace RichDomainModelHealthcare.Services {
    public class BillingService {
        private readonly IRepository<Invoice> _invoiceRepository;

        public BillingService(IRepository<Invoice> invoiceRepository) {
            _invoiceRepository = invoiceRepository;
        }

        public async Task GenerateInvoiceAsync(Invoice invoice) {
            // Add business logic for generating an invoice
            await _invoiceRepository.CreateAsync(invoice);
        }

        // ... Additional service methods
    }
}
