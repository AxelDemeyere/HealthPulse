using Common.Application.Service;
using ConsultationService.Application.Dto;
using ConsultationService.Domain.Entity;
using ConsultationService.Domain.Ports;
using PatientService.Application.Service;

namespace ConsultationService.Application.Service;

public class ConsultationService : GenericService<Consultation, ConsultationDtos.Receive>, IConsultationService
{
    private readonly IPatientService _patientService;

    public ConsultationService(IConsultationRepository repository, IPatientService patientService) 
        : base(repository)
    {
        _patientService = patientService;
    }

    public override async Task<Consultation> CreateAsync(ConsultationDtos.Receive dto)
    {
        var consultation = new Consultation
        {
            PatientId = dto.PatientId,
            Motif = dto.Motif,
            DateConsultation = dto.DateConsultation,
            DureeMinutes = dto.DureeMinutes
        };
        
        consultation.Tarif = consultation.GetTarifParDefaut();
        
        await Repository.AddAsync(consultation);
        return consultation;
    }

    public override async Task<Consultation> UpdateAsync(int id, ConsultationDtos.Receive dto)
    {
        var consultation = await Repository.GetByIdAsync(id);
        if (consultation == null) throw new KeyNotFoundException();

        consultation.PatientId = dto.PatientId;
        consultation.Motif = dto.Motif;
        consultation.DateConsultation = dto.DateConsultation;
        consultation.DureeMinutes = dto.DureeMinutes;
        consultation.Tarif = consultation.GetTarifParDefaut();

        await Repository.UpdateAsync(consultation);
        return consultation;
    }

    public async Task<decimal> GetCoutHoraireByIdAsync(int id)
    {
        var consultation = await Repository.GetByIdAsync(id);
        if (consultation == null) throw new KeyNotFoundException();
        
        return consultation.CoutHoraire();
    }

    public async Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(int id)
    {
        var consultations = await Repository.GetAllAsync();
        return consultations.Where(c => c.PatientId == id);
    }

    public async Task<Consultation?> GetConsultationWithPatientAsync(int patientId)
    {
        var patient = await _patientService.GetByIdAsync(patientId);
        if (patient == null) return null;
        var consultation = await Repository.GetByIdAsync(patientId);
        
        return consultation;
    }
}