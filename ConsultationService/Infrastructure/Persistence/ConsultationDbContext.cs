using ConsultationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ConsultationService.Infrastructure.Persistence;

public class ConsultationDbContext : DbContext
{
    public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options) : base(options) {}
    public DbSet<Consultation> Consultations { get; set; }
    
}