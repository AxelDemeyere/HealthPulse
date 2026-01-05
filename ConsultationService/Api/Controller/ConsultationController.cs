using Microsoft.AspNetCore.Mvc;
using ConsultationService.Application.Service;
using ConsultationService.Application.Dto;


namespace ConsultationService.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;
    public ConsultationController(IConsultationService s) => _consultationService = s;

    [HttpGet] public async Task<IActionResult> Get() => Ok(await _consultationService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _consultationService.GetByIdAsync(id));
    
    [HttpGet("{id}/cout-horaire")] public async Task<IActionResult> GetCoutHoraire(int id) => Ok(await _consultationService.GetCoutHoraireByIdAsync(id));
    
    [HttpGet("patient/{patientId}")] public async Task<IActionResult> GetByPatientId(int patientId) => Ok(await _consultationService.GetConsultationsByPatientIdAsync(patientId));

    [HttpPost]
    public async Task<IActionResult> Post(ConsultationDtos.Receive d) => Ok(await _consultationService.CreateAsync(d));
    
    [HttpPut("{id}")] public async Task<IActionResult> Put(int id, ConsultationDtos.Receive d) => Ok(await _consultationService.UpdateAsync(id, d));
    
    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) { await _consultationService.DeleteAsync(id); return NoContent(); }
}