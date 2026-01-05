using ConsultationService.Domain.Entity;

namespace ConsultationService.Application.Dto;

public class ConsultationDtos
{
    public record Receive(int PatientId, Motif Motif, DateTime DateConsultation, int DureeMinutes);
    public record Send(int Id, int PatientId, Motif Motif, DateTime DateConsultation, int DureeMinutes, decimal Tarif);
}
