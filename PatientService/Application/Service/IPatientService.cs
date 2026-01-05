using Common.Application.Service;
using PatientService.Domain.Entity;

namespace PatientService.Application.Service;
using Dto;

public interface IPatientService : IGenericService<Patient, PatientDtos.Receive>
{
}
