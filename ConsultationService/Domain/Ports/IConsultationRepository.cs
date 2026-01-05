using Common.Domain.Repository;
using ConsultationService.Domain.Entity;

namespace ConsultationService.Domain.Ports;

public interface IConsultationRepository : IGenericRepository<Consultation> { }