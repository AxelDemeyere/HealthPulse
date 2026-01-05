using ConsultationService.Application.Service;
using ConsultationService.Domain.Ports;
using ConsultationService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PatientService.Application.Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ConsultationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var patientServiceUrl = builder.Configuration["PatientServiceUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient<IPatientService, PatientService.Application.Service.PatientService>(client =>
{
    client.BaseAddress = new Uri(patientServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add services to the container.

builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IConsultationService, ConsultationService.Application.Service.ConsultationService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ConsultationDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Migrations Patient appliquées.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();