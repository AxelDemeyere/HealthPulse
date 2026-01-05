using Common.Domain.Repository;
using PatientService.Domain.Entity;

namespace PatientService.Domain.Ports;

public interface IPatientRepository : IGenericRepository<Patient>
{
}