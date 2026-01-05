namespace ConsultationService.Domain.Entity;

public class Consultation
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public static Motif Motif { get; set; }
    public DateTime DateConsultation { get; set; }
    public int DureeMinutes { get; set; }
    public decimal Tarif { get; set; } = GetTarifParDefaut();

    public decimal CoutHoraire() {
        return (Tarif / DureeMinutes) * 60;
    }
    
    public static decimal GetTarifParDefaut() {
        return Motif switch
        {
            Motif.Routine => 25m,
            Motif.Suivi => 35m,
            Motif.Vaccination => 15m,
            Motif.Urgence => 50m,
            _ => 0m
        };
    }
    
    
    
}