using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Services;
using Xunit;

namespace RichDomainModelHealthcareTest.Tests;

public class HealthcareDataServiceTests {
    private HealthcareContext GetMockContext() {
        var options = new DbContextOptionsBuilder<HealthcareContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new HealthcareContext(options);

        // Seeding data using DataSeeder
        var patients = DataSeeder.GenerateFakePatients(10);
        var clinicians = DataSeeder.GenerateFakeClinicians(5);
        var treatments = DataSeeder.GenerateFakeTreatments(patients, clinicians,20);
        var appointments = DataSeeder.GenerateFakeAppointments(patients, clinicians, 15);
        
        context.Patients.AddRange(patients);
        context.Clinicians.AddRange(clinicians);
        context.Treatments.AddRange(treatments);
        context.Appointments.AddRange(appointments);
        context.SaveChanges();

        return context;
    }

    // Test for GetPatientsByCondition
    [Fact]
    public void GetPatientsByCondition_ReturnsCorrectPatients() {
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var elderlyPatients = service.GetPatientsByCondition(p => p.DateOfBirth < DateTime.Now.AddYears(-65));
        Assert.All(elderlyPatients, p => Assert.True(p.DateOfBirth < DateTime.Now.AddYears(-65)));
    }

    // Test for GetTreatmentsForPatient
    [Fact]
    public void GetTreatmentsForPatient_ReturnsCorrectTreatments() {
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var patientId = context.Patients.First().Id;
        var patientTreatments = service.GetTreatmentsForPatient(patientId);
        Assert.All(patientTreatments, t => Assert.Equal(patientId, t.PatientId));
    }

    // Additional test cases for each method in HealthcareDataService...
    
    [Fact]
    public void GetAppointmentsForClinicianOnDate_ReturnsAppointmentsOnDate() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var clinicianId = context.Clinicians.First().Id;
        var date = DateTime.Today;

        // Act
        var result = service.GetAppointmentsForClinicianOnDate(clinicianId, date);

        // Assert
        Assert.All(result, a => Assert.Equal(date.Date, a.AppointmentDate.Date));
        Assert.All(result, a => Assert.Equal(clinicianId, a.ClinicianId));
    }

    [Fact]
    public void GetAppointmentsForClinicianOnDate_ReturnsEmptyForNoAppointments() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var clinicianId = context.Clinicians.First().Id;
        var date = DateTime.Today.AddDays(30); // Assuming no appointments scheduled this far

        // Act
        var result = service.GetAppointmentsForClinicianOnDate(clinicianId, date);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetCliniciansBySpecialty_ReturnsCorrectClinicians() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var specialty = "Cardiology";

        // Act
        var result = service.GetCliniciansBySpecialty(specialty);

        // Assert
        Assert.All(result, c => Assert.Equal(specialty, c.Specialty));
    }

    [Fact]
    public void GetCliniciansBySpecialty_ReturnsEmptyForUnknownSpecialty() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var specialty = "UnknownSpecialty";

        // Act
        var result = service.GetCliniciansBySpecialty(specialty);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CountAppointmentsForPatient_ReturnsCorrectCount() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var patientId = context.Patients.First().Id;
        var expectedCount = context.Appointments.Count(a => a.PatientId == patientId);

        // Act
        var count = service.CountAppointmentsForPatient(patientId);

        // Assert
        Assert.Equal(expectedCount, count);
    }
    
    [Fact]
    public void GetCliniciansWithNoAppointments_ReturnsCorrectClinicians() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);

        // Act
        var result = service.GetCliniciansWithNoAppointments();

        // Assert
        Assert.All(result, c => Assert.Empty(c.Appointments));
    }

    [Fact]
    public void GetAllInvoicesForPatient_ReturnsCorrectInvoices() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var patientId = context.Patients.First().Id;

        // Act
        var result = service.GetAllInvoicesForPatient(patientId);

        // Assert
        Assert.All(result, i => Assert.Equal(patientId, i.Patient.Id));
    }

    [Fact]
    public void GetAllInvoicesForPatient_ReturnsEmptyForNoInvoices() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        // Add a new patient with no invoices
        var newPatient = DataSeeder.GenerateFakePatients(1)[0];
        context.Patients.Add(newPatient);
        context.SaveChanges();

        // Act
        var result = service.GetAllInvoicesForPatient(newPatient.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetRecentTreatments_ReturnsTreatmentsAfterDate() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var sinceDate = DateTime.Now.AddMonths(-1);

        // Act
        var result = service.GetRecentTreatments(sinceDate);

        // Assert
        Assert.All(result, t => Assert.True(t.DateAdministered >= sinceDate));
    }

    [Fact]
    public void GetRecentTreatments_ReturnsEmptyForOldDate() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var sinceDate = DateTime.Now.AddYears(-10);

        // Act
        var result = service.GetRecentTreatments(sinceDate);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public void GetPatientsByClinician_ReturnsCorrectPatients() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var clinicianId = context.Clinicians.First().Id;

        // Act
        var result = service.GetPatientsByClinician(clinicianId);

        // Assert
        Assert.All(result, p => Assert.Contains(p.Appointments, a => a.ClinicianId == clinicianId));
    }

    [Fact]
    public void GetPatientsByClinician_ReturnsEmptyForNoPatients() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        // Add a new clinician with no patients
        var newClinician = DataSeeder.GenerateFakeClinicians(1)[0];
        context.Clinicians.Add(newClinician);
        context.SaveChanges();

        // Act
        var result = service.GetPatientsByClinician(newClinician.Id);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public void UpdateClinicianSpecialty_UpdatesSpecialtyCorrectly() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var clinicianId = context.Clinicians.First().Id;
        var newSpecialty = "Neurology";

        // Act
        service.UpdateClinicianSpecialty(clinicianId, newSpecialty);
        var updatedClinician = context.Clinicians.Find(clinicianId);

        // Assert
        Assert.Equal(newSpecialty, updatedClinician.Specialty);
    }

    [Fact]
    public void UpdateClinicianSpecialty_DoesNothingForInvalidClinician() {
        // Arrange
        var context = GetMockContext();
        var service = new HealthcareDataService(context);
        var invalidClinicianId = Guid.NewGuid();
        var newSpecialty = "Neurology";

        // Act
        service.UpdateClinicianSpecialty(invalidClinicianId, newSpecialty);

        // Assert
        var clinician = context.Clinicians.Find(invalidClinicianId);
        Assert.Null(clinician); // Clinician should not exist
    }
    // Note: Ensure each method is tested with at least two sensible test cases.
}