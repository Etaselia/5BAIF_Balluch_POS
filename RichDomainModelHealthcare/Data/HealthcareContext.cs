using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Models;
using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Data
{
    public class HealthcareContext : DbContext
    {
        public HealthcareContext(DbContextOptions<HealthcareContext> options) : base(options) {
        }

        // DbSet properties
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Money> Moneys { get; set; } // Note: Consider a more appropriate name based on the context
        public DbSet<Name> Names { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Appointment>()
        //         .HasOne(a => a.Clinician)
        //         .WithMany(c => c._appointments)
        //         .HasForeignKey(a => a.ClinicianId);
        //
        //     modelBuilder.Entity<Appointment>()
        //         .HasOne(a => a.Patient)
        //         .WithMany(p => p.Appointments)
        //         .HasForeignKey(a => a.PatientId);
        //
        //     modelBuilder.Entity<Patient>()
        //         .HasOne(p => p.MedicalRecord) // Patient is the principal
        //         .WithOne(mr => mr.Patient)    // MedicalRecord is the dependent
        //         .HasForeignKey<MedicalRecord>(mr => mr.PatientId); // Specify the foreign key property
        //
        //     // Additional relationships can be configured here
        //
        //     base.OnModelCreating(modelBuilder);
        // }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Clinician)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicianId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            // MedicalRecord relationship
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey<MedicalRecord>(mr => mr.PatientId);

            modelBuilder.Entity<MedicalRecord>().HasMany(mr => mr.Treatments);
            
            // Treatment relationships
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Patient)
                .WithMany(p => p.Treatments)
                .HasForeignKey(t => t.PatientId);

            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Clinician)
                .WithMany(c => c.Treatments)
                .HasForeignKey(t => t.ClinitianId);

            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.MedicalRecord)
                .WithMany(mr => mr.Treatments)
                .HasForeignKey(t => t.MedicalRecordId);

            base.OnModelCreating(modelBuilder);
        }

    }
}