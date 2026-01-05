using PatientService.Application.Dto;
using PatientService.Domain.Entity;
using PatientService.Domain.Ports;
 

namespace PatientService.Application.Service;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<PatientDtos.Send>> GetAllPatientsAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return patients.Select(p => new PatientDtos.Send(p.Id, p.Nom, p.DateNaissance, p.GroupeSanguin));
    }
    public async Task<PatientDtos.Send?> GetPatientByIdAsync(int id)
    {
        var p= await _patientRepository.GetByIdAsync(id);
        return p == null ? 
            throw new KeyNotFoundException() : 
            new PatientDtos.Send(p.Id, p.Nom, p.DateNaissance, p.GroupeSanguin);
    }
    public async Task<PatientDtos.Send> CreatePatientAsync(PatientDtos.Receive dto)
    {
        var p = new Patient 
        { 
            Nom = dto.Nom, 
            DateNaissance = dto.DateNaissance, 
            GroupeSanguin = dto.GroupeSanguin 
        };
        await _patientRepository.AddAsync(p);
        return new PatientDtos.Send(p.Id, p.Nom, p.DateNaissance, p.GroupeSanguin);
    }
    public async Task<PatientDtos.Send> UpdatePatientAsync(int id, PatientDtos.Receive dto)
    {
        var existingPatient = await _patientRepository.GetByIdAsync(id);
        if (existingPatient == null)
        {
            throw new KeyNotFoundException();
        }
        
        existingPatient.Nom = dto.Nom;
        existingPatient.DateNaissance = dto.DateNaissance;
        existingPatient.GroupeSanguin = dto.GroupeSanguin;

        await _patientRepository.UpdateAsync(existingPatient);
        return new PatientDtos.Send(existingPatient.Id, existingPatient.Nom, existingPatient.DateNaissance, existingPatient.GroupeSanguin);
    }
    public async Task<bool> DeletePatientAsync(int id)
    {
        var existingPatient = await _patientRepository.GetByIdAsync(id);
        if (existingPatient == null)
        {
            throw new KeyNotFoundException();
        }
        
        await _patientRepository.DeleteAsync(existingPatient);
        return true;
    }
}