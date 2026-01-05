using Common.Application.Service;
using ConsultationService.Application.Dto;
using ConsultationService.Domain.Entity;

namespace ConsultationService.Application.Service;

public interface IConsultationService : IGenericService<Consultation, ConsultationDtos.Receive>
{
    public Task<decimal> GetCoutHoraireByIdAsync(int id);
    public Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(int patientId);
    public Task<Consultation?> GetConsultationWithPatientAsync(int consultationId);
}