using Common.Application.Service;
using PatientService.Application.Dto;
using PatientService.Domain.Entity;
using PatientService.Domain.Ports;
 

namespace PatientService.Application.Service;

public class PatientService : GenericService<Patient, PatientDtos.Receive>, IPatientService
{
    public PatientService(IPatientRepository repository) : base(repository)
    {
    }

    public override async Task<Patient> CreateAsync(PatientDtos.Receive dto)
    {
        var patient = new Patient
        {
            Nom = dto.Nom,
            DateNaissance = dto.DateNaissance,
            GroupeSanguin = dto.GroupeSanguin
        };
        await Repository.AddAsync(patient);
        return patient;
    }
    
    public override async Task<Patient> UpdateAsync(int id, PatientDtos.Receive dto)
    {
        var patient = await Repository.GetByIdAsync(id);
        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with id {id} not found.");
        }

        patient.Nom = dto.Nom;
        patient.DateNaissance = dto.DateNaissance;
        patient.GroupeSanguin = dto.GroupeSanguin;

        await Repository.UpdateAsync(patient);
        return patient;
    }

}