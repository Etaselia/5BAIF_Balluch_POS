using Microsoft.AspNetCore.Mvc.RazorPages;
using RichDomainModelHealthcare.Data;

namespace RichDomainModelHealthcareApp.Pages;

public class DebugModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly HealthcareContext _context;

    public DebugModel(IConfiguration configuration, HealthcareContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public IDictionary<string, string> ConfigurationValues { get; set; }
    public IList<string> EntityInformation { get; private set; } = new List<string>();

    public void OnGet()
    {
        ConfigurationValues = new Dictionary<string, string>();
        foreach (var kvp in _configuration.AsEnumerable())
        {
            ConfigurationValues.Add(kvp.Key, kvp.Value);
        }

        // Retrieve and store entity information
        var entityTypes = _context.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            EntityInformation.Add($"Entity: {entityType.ClrType.Name}");
            foreach (var property in entityType.GetProperties())
            {
                EntityInformation.Add($"- Property: {property.Name}, Type: {property.ClrType.Name}");
            }
            // Include navigations, keys, and other aspects as needed
            foreach (var navigation in entityType.GetNavigations())
            {
                EntityInformation.Add($"- Navigation: {navigation.Name}, Type: {navigation.ClrType.Name}");
            }
        }
    }
}