namespace PatientService.Infrastructure;
using PatientService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entity;


public class PatientRepository : IPatientRepository
{
    private readonly PatientDbContext _context;
    public PatientRepository(PatientDbContext context) => _context = context;
    
    public async Task<IEnumerable<Patient>> GetAllAsync() => await _context.Patients.ToListAsync();
    public async Task AddAsync(Patient patient) 
    { 
        await _context.Patients.AddAsync(patient); 
        await _context.SaveChangesAsync(); 
    }
    public async Task<Patient?> GetByIdAsync(int id) => await _context.Patients.FindAsync(id);
    public async Task UpdateAsync(Patient patient) 
    { 
        _context.Patients.Update(patient); 
        await _context.SaveChangesAsync(); 
    }
    public async Task DeleteAsync(Patient patient)
    {
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
    
}