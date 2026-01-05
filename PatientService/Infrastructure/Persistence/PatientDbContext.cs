namespace PatientService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entity;


public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }
    public DbSet<Patient> Patients { get; set; }
}