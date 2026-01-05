using PatientService.Domain.Entity;

namespace PatientService.Application.Dto;

public class PatientDtos
{
    public record Receive(string Nom, DateTime DateNaissance, GroupeSanguin GroupeSanguin);
    public record Send(int Id, string Nom, DateTime DateNaissance, GroupeSanguin GroupeSanguin);
}