using backend_email.Data.Models;
using backend_email.Data.SmtpServices;
using Microsoft.EntityFrameworkCore;

namespace backend_email.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    public DbSet<Doctor> Doctors { get; set; }        
    public DbSet<Icd10Record> Icd10Records { get; set; }    
    public DbSet<Inspection> Inspections { get; set; }   
    public DbSet<InspectionComment> InspectionComments { get; set; }
    public DbSet<InspectionConsultation> InspectionConsultations { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Speciality> Specialities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Patient>().HasData(
            new Patient 
            {   Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                CreateTime = DateTime.UtcNow,
                Name = "New Patient", 
                Gender = Gender.Male
            }
        );

        modelBuilder.Entity<Doctor>().HasData( 
            new Doctor
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa8"),
                CreateTime = DateTime.UtcNow,
                Name = "New Doctor",
                Gender = Gender.Male,
                Email = "helpmeplz@gmail.com"
            }
        );
    }


}