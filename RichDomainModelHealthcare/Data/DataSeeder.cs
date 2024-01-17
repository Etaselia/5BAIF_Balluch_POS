using Bogus;
using RichDomainModelHealthcare.Models;
using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Data;

public static class DataSeeder {
    public static List<Patient> GenerateFakePatients(int count) {
        var patientGenerator = new Faker<Patient>()
            .CustomInstantiator(f => new Patient(
                new Name(
                    f.Name.FirstName(), 
                    f.Name.LastName()), 
                new Address(f.Address.StreetAddress(), 
                    f.Address.City(), 
                    f.Address.State(), 
                    f.Address.ZipCode()), 
                f.Date.Past(80, DateTime.Now.AddYears(-18)),
                f.Phone.PhoneNumber()
                ))
            .RuleFor(p => p.PatientId, f => Guid.NewGuid());

        return patientGenerator.Generate(count);
    }

    public static List<Clinician> GenerateFakeClinicians(int count) {
        var clinicianGenerator = new Faker<Clinician>()
            .CustomInstantiator(f => new Clinician(new Name(f.Name.FirstName(), f.Name.LastName()), 
                f.Random.Word()))
            .RuleFor(c => c.ClinicianId, f => Guid.NewGuid());

        return clinicianGenerator.Generate(count);
    }

    public static List<Appointment> GenerateFakeAppointments(List<Patient> patients, List<Clinician> clinicians, int count) {
        Console.WriteLine("Creating Appointment F");
        var appointmentGenerator = new Faker<Appointment>()
            .CustomInstantiator(f => new Appointment(
                f.Date.Future(), 
                f.PickRandom(clinicians), 
                f.PickRandom(patients), 
                f.Lorem.Sentence()))
            .RuleFor(a => a.AppointmentId, f => Guid.NewGuid())
            .FinishWith((f, a) => 
            {
                a.Clinician.AddAppointment(a);
                a.Patient.AddAppointment(a);
            });

        return appointmentGenerator.Generate(count);
    }
    
    public static List<Treatment> GenerateFakeTreatments(List<Patient> patients, List<Clinician> clinicians, int count) {
        var treatmentGenerator = new Faker<Treatment>()
            .CustomInstantiator(f => {
                var clinician = f.PickRandom(clinicians);
                var patient = f.PickRandom(patients);
                var treatment = new Treatment {
                    Description = f.Lorem.Sentence(),
                    DateAdministered = f.Date.Past(),
                    Patient = patient,
                    Clinician = clinician,
                    Cost = new Money(f.Random.Decimal(50, 500), "USD"),
                    MedicalRecord = patient.MedicalRecord
                };
                patient.Treatments.Add(treatment);
                patient.MedicalRecord.AddTreatment(treatment);
                clinician.Treatments.Add(treatment);
                return treatment;
            })
            .RuleFor(t => t.TreatmentId, f => Guid.NewGuid());

        return treatmentGenerator.Generate(count);
    }

    public static List<Invoice> GenerateFakeInvoices(List<Patient> patients, List<Treatment> treatments, int count) {
        var invoiceGenerator = new Faker<Invoice>()
            .CustomInstantiator(f => {
                var patient = f.PickRandom(patients);
                var treatment = f.PickRandom(treatments);
                var invoice = new Invoice {
                    DateIssued = f.Date.Recent(),
                    Patient = patient,
                    TotalAmount = new Money(f.Random.Decimal(100, 1000), "USD"),
                    Treatment = treatment
                };
                patient.Invoices.Add(invoice);
                treatment.Invoices.Add(invoice);
                return invoice;
            })
            .RuleFor(i => i.InvoiceId, f => Guid.NewGuid());

        return invoiceGenerator.Generate(count);
    }

    public static List<MedicalRecord> GenerateFakeMedicalRecords(List<Patient> patients) {
        var medicalRecords = new List<MedicalRecord>();

        foreach (var patient in patients) {
            var medicalRecord = new Faker<MedicalRecord>()
                .CustomInstantiator(f => new MedicalRecord {
                    Patient = patient,
                    Treatments = new List<Treatment>(), // Initially empty, treatments will be added in GenerateFakeTreatments
                    MedicalRecordId = Guid.NewGuid()
                })
                .Generate(); // Generate a single MedicalRecord for each patient

            patient.MedicalRecord = medicalRecord; // Associate the medical record with the patient
            medicalRecords.Add(medicalRecord);
        }

        return medicalRecords;
    }

    // Extended to include the generation of Names
    public static List<Name> GenerateFakeNames(int count) {
        var nameGenerator = new Faker<Name>()
            .CustomInstantiator(f => new Name(f.Name.FirstName(), f.Name.LastName()))
            .RuleFor(n => n.NameId, f => Guid.NewGuid());

        return nameGenerator.Generate(count);
    }

    // Extended to include the generation of Addresses
    public static List<Address> GenerateFakeAddresses(int count) {
        var addressGenerator = new Faker<Address>()
            .CustomInstantiator(f => new Address(f.Address.StreetAddress(), f.Address.City(), f.Address.State(), f.Address.ZipCode()))
            .RuleFor(a => a.AddressId, f => Guid.NewGuid());

        return addressGenerator.Generate(count);
    }

    // Extended to include the generation of Money records
    public static List<Money> GenerateFakeMoneys(int count) {
        var moneyGenerator = new Faker<Money>()
            .CustomInstantiator(f => new Money(f.Random.Decimal(50, 1000), "USD"))
            .RuleFor(m => m.MoneyId, f => Guid.NewGuid());

        return moneyGenerator.Generate(count);
    }

    // Other methods for additional model classes...
}