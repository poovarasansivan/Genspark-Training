using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Contexts
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; } = null!;
        public DbSet<Speciality> Specialities { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentNumber).HasName("PK_AppointmentNumber");

            modelBuilder.Entity<Appointment>().HasOne(a => a.Patient)
                                               .WithMany(p => p.Appointments)
                                               .HasForeignKey(a => a.PatientId)
                                               .HasConstraintName("FK_Appointments_Patients")
                                               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>().HasOne(a => a.Doctor)
                                               .WithMany(d => d.Appointments)
                                               .HasForeignKey(a => a.DoctorId)
                                               .HasConstraintName("FK_Appointments_Doctors")
                                               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>().HasKey(ds => ds.SerialNumber);

            modelBuilder.Entity<DoctorSpeciality>().HasOne(ds => ds.Doctor)
                                                    .WithMany(d => d.DoctorSpecialities)
                                                    .HasForeignKey(ds => ds.DoctorId)
                                                    .HasConstraintName("FK_DoctorSpeciality_Doctors")
                                                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>().HasOne(ds => ds.Speciality)
                                                    .WithMany(s => s.DoctorSpecialities)
                                                    .HasForeignKey(ds => ds.SpecialityId)
                                                    .HasConstraintName("FK_DoctorSpeciality_Specialities")
                                                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}