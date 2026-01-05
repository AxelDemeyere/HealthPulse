using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Service;
using PatientService.Domain.Ports;
using PatientService.Application.Dto;

namespace PatientService.Api.Controller;

[ApiController] [Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    public PatientController(IPatientService patientService) => this._patientService = patientService;
    
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _patientService.GetAllPatientsAsync());
    [HttpGet("{id}")] public async Task<IActionResult> Get(int id) => Ok(await _patientService.GetPatientByIdAsync(id));
    [HttpPost] public async Task<IActionResult> Post(PatientDtos.Receive dto) => Ok(await _patientService.CreatePatientAsync(dto));
    [HttpPut("{id}")] public async Task<IActionResult> Put(int id, PatientDtos.Receive dto) => Ok(await _patientService.UpdatePatientAsync(id, dto));
    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => Ok(await _patientService.DeletePatientAsync(id));
}