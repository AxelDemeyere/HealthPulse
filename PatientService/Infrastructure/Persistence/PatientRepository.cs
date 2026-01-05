namespace PatientService.Infrastructure.Persistence;

using Common.Infrastucture.Persistence;
using Domain.Entity;
using Domain.Ports;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(PatientDbContext context) : base(context)
    {
    }
}