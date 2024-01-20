using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RichDomainModelHealthcare.Data;
using System;

namespace RichDomainModelHealthcare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Configure and create the database
            ConfigureAndCreateDatabase(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<HealthcareContext>(options =>
                        options.UseSqlite("Data Source=HealthcareDatabase.db"));
                });
        private static void ConfigureAndCreateDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<HealthcareContext>();

                    // Clear existing tracking
                    context.ChangeTracker.Clear();

                    // Ensure that the database is created and applies any pending migrations
                    context.Database.EnsureCreated();

                    // Check if the database is already seeded
                    if (!context.Patients.Any())
                    {
                        // Generate and add appointments (make sure they have unique Ids)
                        var patients = DataSeeder.GenerateFakePatients(100);
                        var clinicians = DataSeeder.GenerateFakeClinicians(100);
                        var appointments = DataSeeder.GenerateFakeAppointments(patients, clinicians, 100);
                        var treatments = DataSeeder.GenerateFakeTreatments(patients,clinicians, 100);
                        var invoices = DataSeeder.GenerateFakeInvoices(patients, treatments,100);

                        context.Patients.AddRange(patients);
                        context.Clinicians.AddRange(clinicians);

                        // Clear existing tracking again before adding appointments
                        context.ChangeTracker.Clear();

                        context.Appointments.AddRange(appointments);
                        context.Treatments.AddRange(treatments);
                        context.Invoices.AddRange(invoices);

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, possibly log them
                    Console.WriteLine("An error occurred while configuring and creating the database: " + ex.Message);
                }
            }
        }

    }
}
