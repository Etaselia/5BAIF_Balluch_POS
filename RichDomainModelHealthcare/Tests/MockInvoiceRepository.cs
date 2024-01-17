using RichDomainModelHealthcare.Interfaces;
using RichDomainModelHealthcare.Models;

// A mock repository for unit testing that simulates the behavior of the actual repository
namespace RichDomainModelHealthcare.Tests;

public class MockInvoiceRepository : IRepository<Invoice> {
    public Task CreateAsync(Invoice entity) {
        // Simulate the async behavior of a real repository method
        return Task.CompletedTask;
    }

    // ... Implement other methods of IRepository as needed for testing
    public Task<Invoice> GetByIdAsync(Guid id) {
        // Return a fake invoice for the given id
        return Task.FromResult(new Invoice());
    }

    public Task<IEnumerable<Invoice>> GetAllAsync() {
        // Return a list of fake invoices
        return Task.FromResult<IEnumerable<Invoice>>(new List<Invoice>());
    }

    public Task UpdateAsync(Invoice entity) {
        // Simulate updating an invoice
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id) {
        // Simulate deleting an invoice
        return Task.CompletedTask;
    }
}

// Note: To use this mock repository, you'll need to add the Moq library to your project.
