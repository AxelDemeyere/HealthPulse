using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Service;
using PatientService.Application.Dto;

namespace PatientService.Api.Controller;

[ApiController] [Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    public PatientController(IPatientService patientService) => this._patientService = patientService;
    
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _patientService.GetAllAsync());
    [HttpGet("{id}")] public async Task<IActionResult> Get(int id) => Ok(await _patientService.GetByIdAsync(id));
    [HttpPost] public async Task<IActionResult> Post(PatientDtos.Receive dto) => Ok(await _patientService.CreateAsync(dto));
    [HttpPut("{id}")] public async Task<IActionResult> Put(int id, PatientDtos.Receive dto) => Ok(await _patientService.UpdateAsync(id, dto));
    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => Ok(await _patientService.DeleteAsync(id));
}