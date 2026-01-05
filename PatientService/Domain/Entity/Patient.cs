namespace PatientService.Domain.Entity;

public class Patient
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public GroupeSanguin GroupeSanguin { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
}