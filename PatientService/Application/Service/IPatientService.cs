namespace PatientService.Application.Service;
using Dto;

public interface IPatientService
{
    Task<IEnumerable<PatientDtos.Send>> GetAllPatientsAsync();
    Task<PatientDtos.Send?> GetPatientByIdAsync(int id);
    Task<PatientDtos.Send> CreatePatientAsync(PatientDtos.Receive dto);
    Task<PatientDtos.Send> UpdatePatientAsync(int id, PatientDtos.Receive dto);
    Task<bool> DeletePatientAsync(int id);
}