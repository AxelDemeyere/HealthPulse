using Common.Infrastucture.Persistence;
using ConsultationService.Domain.Entity;
using ConsultationService.Domain.Ports;

namespace ConsultationService.Infrastructure.Persistence;

public class ConsultationRepository : GenericRepository<Consultation>, IConsultationRepository
{
    public ConsultationRepository(ConsultationDbContext context) : base(context)
    {
    }
}