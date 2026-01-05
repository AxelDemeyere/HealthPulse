# Workshop Pratique : Plateforme HealthPulse
## Suivi Médical Connecté - Microservices ASP.NET Core avec Architecture Hexagonale

**Durée estimée :** 6-8 heures  
**Niveau :** Intermédiaire à Avancé

---

## Table des matières

1. [Introduction](#introduction)
2. [Contexte fonctionnel](#contexte-fonctionnel)
3. [Spécifications des microservices](#spécifications-des-microservices)
4. [Étape 1 : Création de la solution](#étape-1--création-de-la-solution-et-des-projets)
5. [Étape 2 : Structure hexagonale](#étape-2--structure-hexagonale-des-projets)
6. [Étape 3 : PatientService](#étape-3--implémentation-du-patientservice)
7. [Étape 4 : ConsultationService](#étape-4--implémentation-du-consultationservice)
8. [Étape 5 : PrescriptionService](#étape-5--implémentation-du-prescriptionservice)
9. [Étape 6 : HealthDashboardService](#étape-6--implémentation-du-healthdashboardservice)
10. [Étape 7 : Tests et validation](#étape-7--tests-et-validation)
11. [Bonus](#bonus)
12. [Critères d'évaluation](#critères-dévaluation)

---

## Introduction

Ce workshop vous guide pas à pas dans la création d'une plateforme de suivi médical connecté appelée **HealthPulse**. Vous allez concevoir et implémenter 4 microservices indépendants en suivant l'architecture hexagonale (Ports & Adapters).

### Objectifs pédagogiques

1. Maîtriser l'architecture hexagonale dans un contexte microservices
2. Créer des API REST complètes avec ASP.NET Core
3. Implémenter la communication inter-services via HttpClient
4. Appliquer les bonnes pratiques de séparation des responsabilités
5. Configurer Swagger pour la documentation d'API

### Prérequis

- .NET 8 SDK installé
- Visual Studio 2022 ou VS Code avec extensions C#
- Connaissances de base en C# et ASP.NET Core
- Postman ou extension REST Client pour les tests

---

## Contexte fonctionnel

La plateforme **HealthPulse** permet de suivre les indicateurs de santé d'une population ou d'un établissement de soins. Elle est composée de **3 microservices métiers** et **1 service d'agrégation**.

### Vue d'ensemble de l'architecture

```
┌─────────────────────────────────────────────────────────────┐
│              HEALTH DASHBOARD SERVICE (Port 5000)            │
│                    Agrégation des données                    │
└─────────────────────────────────────────────────────────────┘
                    │           │           │
                    ▼           ▼           ▼
┌─────────────┐ ┌─────────────────┐ ┌──────────────────┐
│PatientService│ │ConsultationService│ │PrescriptionService│
│  Port 5001   │ │    Port 5002      │ │    Port 5003       │
└─────────────┘ └─────────────────┘ └──────────────────┘
```

---

## Spécifications des microservices

### Service 1 : PatientService (Port 5001)

Ce service gère l'ensemble des informations relatives aux patients enregistrés dans le système.

#### Modèle de données Patient

| Propriété | Type | Description |
|-----------|------|-------------|
| `Id` | Guid | Identifiant unique du patient |
| `Nom` | string | Nom complet du patient |
| `DateNaissance` | DateTime | Date de naissance |
| `GroupeSanguin` | enum | A+, A-, B+, B-, AB+, AB-, O+, O- |
| `DateInscription` | DateTime | Date d'inscription dans le système |

#### Endpoints REST

| Méthode | Route | Description |
|---------|-------|-------------|
| **GET** | `/api/patients` | Liste tous les patients |
| **GET** | `/api/patients/{id}` | Récupère un patient par ID |
| **POST** | `/api/patients` | Crée un nouveau patient |
| **PUT** | `/api/patients/{id}` | Met à jour un patient |
| **DELETE** | `/api/patients/{id}` | Supprime un patient |

---

### Service 2 : ConsultationService (Port 5002)

Ce service gère le suivi des consultations médicales et inclut un calcul de coût horaire.

#### Modèle de données Consultation

| Propriété | Type | Description |
|-----------|------|-------------|
| `Id` | Guid | Identifiant unique |
| `PatientId` | Guid | Référence vers le patient |
| `Motif` | enum | Routine, Urgence, Suivi, Vaccination |
| `DateConsultation` | DateTime | Date et heure de la consultation |
| `DureeMinutes` | int | Durée en minutes |
| `Tarif` | decimal | Tarif de la consultation en euros |

#### Formule de calcul du coût horaire

```
CoutHoraire = (Tarif / DureeMinutes) × 60
```

#### Barème tarifaire indicatif

| Type de consultation | Tarif indicatif |
|---------------------|-----------------|
| Routine | 25 € |
| Suivi | 35 € |
| Vaccination | 15 € |
| Urgence | 50 € |

#### Endpoints REST

| Méthode | Route | Description |
|---------|-------|-------------|
| **GET** | `/api/consultations` | Liste toutes les consultations |
| **GET** | `/api/consultations/{id}` | Récupère une consultation |
| **GET** | `/api/consultations/patient/{patientId}` | Consultations d'un patient |
| **GET** | `/api/consultations/{id}/cout-horaire` | Calcule le coût horaire |
| **POST** | `/api/consultations` | Crée une consultation |
| **PATCH** | `/api/consultations/{id}` | Mise à jour partielle |

---

### Service 3 : PrescriptionService (Port 5003)

Ce service gère les prescriptions médicales associées aux consultations.

#### Modèle de données Prescription

| Propriété | Type | Description |
|-----------|------|-------------|
| `Id` | Guid | Identifiant unique |
| `ConsultationId` | Guid | Référence vers la consultation |
| `Medicament` | string | Nom du médicament |
| `Dosage` | string | Ex: "500mg", "10ml" |
| `Frequence` | string | Ex: "3 fois/jour" |
| `DureeJours` | int | Durée du traitement en jours |
| `Renouvelable` | bool | Prescription renouvelable ou non |

#### Formule de calcul du total des prises

```
TotalPrises = FrequenceParJour × DureeJours
```

#### Endpoints REST

| Méthode | Route | Description |
|---------|-------|-------------|
| **GET** | `/api/prescriptions` | Liste les prescriptions |
| **GET** | `/api/prescriptions/{id}` | Récupère une prescription |
| **GET** | `/api/prescriptions/consultation/{consultationId}` | Par consultation |
| **GET** | `/api/prescriptions/{id}/total-prises` | Calcule total prises |
| **POST** | `/api/prescriptions` | Crée une prescription |
| **PATCH** | `/api/prescriptions/{id}` | Mise à jour partielle |
| **DELETE** | `/api/prescriptions/{id}` | Supprime |

---

### Service 4 : HealthDashboardService (Port 5000)

Ce service d'agrégation collecte et synthétise les données des trois autres microservices pour fournir une vue globale de l'activité médicale.

#### Fonctionnalités d'agrégation

- Nombre total de patients inscrits
- Nombre de consultations par type (Routine, Urgence, Suivi, Vaccination)
- Chiffre d'affaires total des consultations
- Nombre total de prescriptions actives
- Statistiques par groupe sanguin

#### Endpoints REST

| Méthode | Route | Description |
|---------|-------|-------------|
| **GET** | `/api/dashboard` | Vue globale complète |
| **GET** | `/api/dashboard/patient/{patientId}` | Historique complet d'un patient |

#### Structure de réponse attendue pour `/api/dashboard`

```json
{
  "totalPatients": 150,
  "consultationsParType": {
    "Routine": 45,
    "Urgence": 12,
    "Suivi": 38,
    "Vaccination": 25
  },
  "chiffreAffairesTotal": 4250.00,
  "totalPrescriptions": 89,
  "patientsParGroupeSanguin": {
    "A+": 42,
    "A-": 8,
    "B+": 25,
    "B-": 5,
    "AB+": 15,
    "AB-": 3,
    "O+": 40,
    "O-": 12
  }
}
```

---

## Étape 1 : Création de la solution et des projets

**Durée estimée : 20 minutes**

### Instructions

1. **Créez un dossier pour le projet**

```bash
mkdir HealthPulse && cd HealthPulse
```

2. **Créez la solution .NET**

```bash
dotnet new sln -n HealthPulse
```

3. **Créez les 4 projets Web API**

```bash
dotnet new webapi -n PatientService -o src/PatientService
dotnet new webapi -n ConsultationService -o src/ConsultationService
dotnet new webapi -n PrescriptionService -o src/PrescriptionService
dotnet new webapi -n HealthDashboardService -o src/HealthDashboardService
```

4. **Ajoutez les projets à la solution**

```bash
dotnet sln add src/PatientService/PatientService.csproj
dotnet sln add src/ConsultationService/ConsultationService.csproj
dotnet sln add src/PrescriptionService/PrescriptionService.csproj
dotnet sln add src/HealthDashboardService/HealthDashboardService.csproj
```

5. **Configurez les ports dans chaque fichier `Properties/launchSettings.json`**

### Configuration des ports

| Service | URL |
|---------|-----|
| HealthDashboardService | `http://localhost:5000` |
| PatientService | `http://localhost:5001` |
| ConsultationService | `http://localhost:5002` |
| PrescriptionService | `http://localhost:5003` |

### Exemple de configuration launchSettings.json (PatientService)

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### ✅ Vérification

Lancez chaque service et vérifiez que Swagger est accessible :
- http://localhost:5001/swagger (PatientService)
- http://localhost:5002/swagger (ConsultationService)
- http://localhost:5003/swagger (PrescriptionService)
- http://localhost:5000/swagger (HealthDashboardService)

---

## Étape 2 : Structure hexagonale des projets

**Durée estimée : 30 minutes**

Chaque microservice doit suivre l'architecture hexagonale (Ports & Adapters).

### Structure de dossiers à créer

```
PatientService/
├── Domain/
│   ├── Entities/
│   │   └── Patient.cs
│   └── Ports/
│       └── IPatientRepository.cs
├── Application/
│   ├── DTOs/
│   │   ├── PatientRequestDto.cs
│   │   └── PatientResponseDto.cs
│   ├── Mappers/
│   │   └── PatientMapper.cs
│   └── Services/
│       ├── IPatientService.cs
│       └── PatientService.cs
├── Infrastructure/
│   └── Persistence/
│       └── InMemoryPatientRepository.cs
├── Api/
│   └── Controllers/
│       └── PatientsController.cs
└── Program.cs
```

### Explication des couches

| Couche | Responsabilité |
|--------|----------------|
| **Domain** | Entités métier et interfaces (ports). Aucune dépendance externe. |
| **Application** | Cas d'utilisation, DTOs, mappers. Orchestre la logique métier. |
| **Infrastructure** | Implémentation des ports (adapters). Accès données, HTTP clients. |
| **Api** | Controllers REST. Point d'entrée HTTP. |

### Commandes pour créer la structure

```bash
# Pour PatientService
cd src/PatientService
mkdir -p Domain/Entities Domain/Ports
mkdir -p Application/DTOs Application/Mappers Application/Services
mkdir -p Infrastructure/Persistence
mkdir -p Api/Controllers

# Répétez pour les autres services
```

### Principe de dépendance

```
Api → Application → Domain ← Infrastructure
         ↑                        │
         └────────────────────────┘
```

> **Important** : Le Domain ne dépend de rien. L'Infrastructure implémente les interfaces du Domain.

---

## Étape 3 : Implémentation du PatientService

**Durée estimée : 45 minutes**

### 3.1 Entité Patient

**Fichier : `Domain/Entities/Patient.cs`**

```csharp
namespace PatientService.Domain.Entities;

public enum GroupeSanguin
{
    APositif,
    ANegatif,
    BPositif,
    BNegatif,
    ABPositif,
    ABNegatif,
    OPositif,
    ONegatif
}

public class Patient
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public GroupeSanguin GroupeSanguin { get; set; }
    public DateTime DateInscription { get; set; }
    
    public int CalculerAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateNaissance.Year;
        if (DateNaissance.Date > today.AddYears(-age)) age--;
        return age;
    }
}
```

### 3.2 Port IPatientRepository

**Fichier : `Domain/Ports/IPatientRepository.cs`**

```csharp
namespace PatientService.Domain.Ports;

using PatientService.Domain.Entities;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient?> UpdateAsync(Guid id, Patient patient);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Patient>> GetByGroupeSanguinAsync(GroupeSanguin groupe);
}
```

### 3.3 DTOs

**Fichier : `Application/DTOs/PatientRequestDto.cs`**

```csharp
namespace PatientService.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class PatientRequestDto
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date de naissance est obligatoire")]
    public DateTime DateNaissance { get; set; }

    [Required(ErrorMessage = "Le groupe sanguin est obligatoire")]
    public string GroupeSanguin { get; set; } = string.Empty;
}
```

**Fichier : `Application/DTOs/PatientResponseDto.cs`**

```csharp
namespace PatientService.Application.DTOs;

public class PatientResponseDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public string GroupeSanguin { get; set; } = string.Empty;
    public DateTime DateInscription { get; set; }
    public int Age { get; set; }
}
```

### 3.4 Mapper

**Fichier : `Application/Mappers/PatientMapper.cs`**

```csharp
namespace PatientService.Application.Mappers;

using PatientService.Domain.Entities;
using PatientService.Application.DTOs;

public static class PatientMapper
{
    public static PatientResponseDto ToDto(Patient patient)
    {
        return new PatientResponseDto
        {
            Id = patient.Id,
            Nom = patient.Nom,
            DateNaissance = patient.DateNaissance,
            GroupeSanguin = patient.GroupeSanguin.ToString(),
            DateInscription = patient.DateInscription,
            Age = patient.CalculerAge()
        };
    }

    public static Patient ToEntity(PatientRequestDto dto)
    {
        return new Patient
        {
            Nom = dto.Nom,
            DateNaissance = dto.DateNaissance,
            GroupeSanguin = Enum.Parse<GroupeSanguin>(dto.GroupeSanguin.Replace("+", "Positif").Replace("-", "Negatif"))
        };
    }

    public static IEnumerable<PatientResponseDto> ToDtoList(IEnumerable<Patient> patients)
    {
        return patients.Select(ToDto);
    }
}
```

### 3.5 Service Application

**Fichier : `Application/Services/IPatientAppService.cs`**

```csharp
namespace PatientService.Application.Services;

using PatientService.Application.DTOs;

public interface IPatientAppService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync();
    Task<PatientResponseDto?> GetByIdAsync(Guid id);
    Task<PatientResponseDto> CreateAsync(PatientRequestDto dto);
    Task<PatientResponseDto?> UpdateAsync(Guid id, PatientRequestDto dto);
    Task<bool> DeleteAsync(Guid id);
}
```

**Fichier : `Application/Services/PatientAppService.cs`**

```csharp
namespace PatientService.Application.Services;

using PatientService.Domain.Ports;
using PatientService.Application.DTOs;
using PatientService.Application.Mappers;

public class PatientAppService : IPatientAppService
{
    private readonly IPatientRepository _repository;

    public PatientAppService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        var patients = await _repository.GetAllAsync();
        return PatientMapper.ToDtoList(patients);
    }

    public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id);
        return patient != null ? PatientMapper.ToDto(patient) : null;
    }

    public async Task<PatientResponseDto> CreateAsync(PatientRequestDto dto)
    {
        var patient = PatientMapper.ToEntity(dto);
        var created = await _repository.CreateAsync(patient);
        return PatientMapper.ToDto(created);
    }

    public async Task<PatientResponseDto?> UpdateAsync(Guid id, PatientRequestDto dto)
    {
        var patient = PatientMapper.ToEntity(dto);
        var updated = await _repository.UpdateAsync(id, patient);
        return updated != null ? PatientMapper.ToDto(updated) : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}
```

### 3.6 Repository In-Memory

**Fichier : `Infrastructure/Persistence/InMemoryPatientRepository.cs`**

```csharp
namespace PatientService.Infrastructure.Persistence;

using PatientService.Domain.Entities;
using PatientService.Domain.Ports;

public class InMemoryPatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = new();

    public Task<IEnumerable<Patient>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Patient>>(_patients);
    }

    public Task<Patient?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
    }

    public Task<Patient> CreateAsync(Patient patient)
    {
        patient.Id = Guid.NewGuid();
        patient.DateInscription = DateTime.UtcNow;
        _patients.Add(patient);
        return Task.FromResult(patient);
    }

    public Task<Patient?> UpdateAsync(Guid id, Patient patient)
    {
        var existing = _patients.FirstOrDefault(p => p.Id == id);
        if (existing == null) return Task.FromResult<Patient?>(null);

        existing.Nom = patient.Nom;
        existing.DateNaissance = patient.DateNaissance;
        existing.GroupeSanguin = patient.GroupeSanguin;
        
        return Task.FromResult<Patient?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return Task.FromResult(false);
        
        _patients.Remove(patient);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Patient>> GetByGroupeSanguinAsync(GroupeSanguin groupe)
    {
        return Task.FromResult<IEnumerable<Patient>>(
            _patients.Where(p => p.GroupeSanguin == groupe));
    }
}
```

### 3.7 Controller

**Fichier : `Api/Controllers/PatientsController.cs`**

```csharp
namespace PatientService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Services;
using PatientService.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientAppService _service;

    public PatientsController(IPatientAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAll()
    {
        var patients = await _service.GetAllAsync();
        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientResponseDto>> GetById(Guid id)
    {
        var patient = await _service.GetByIdAsync(id);
        if (patient == null)
            return NotFound(new { message = $"Patient avec l'ID {id} non trouvé" });
        
        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponseDto>> Create([FromBody] PatientRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientResponseDto>> Update(Guid id, [FromBody] PatientRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { message = $"Patient avec l'ID {id} non trouvé" });

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Patient avec l'ID {id} non trouvé" });

        return NoContent();
    }
}
```

### 3.8 Configuration Program.cs

**Fichier : `Program.cs`**

```csharp
using PatientService.Domain.Ports;
using PatientService.Infrastructure.Persistence;
using PatientService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Patient Service API", Version = "v1" });
});

// Dependency Injection
builder.Services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();
builder.Services.AddScoped<IPatientAppService, PatientAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
```

### ✅ Vérification Étape 3

1. Lancez le PatientService : `dotnet run`
2. Accédez à Swagger : http://localhost:5001/swagger
3. Testez la création d'un patient :

```json
{
  "nom": "Jean Dupont",
  "dateNaissance": "1985-03-15",
  "groupeSanguin": "A+"
}
```

---

## Étape 4 : Implémentation du ConsultationService

**Durée estimée : 45 minutes**

### 4.1 Entité Consultation

**Fichier : `Domain/Entities/Consultation.cs`**

```csharp
namespace ConsultationService.Domain.Entities;

public enum MotifConsultation
{
    Routine,
    Urgence,
    Suivi,
    Vaccination
}

public class Consultation
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public MotifConsultation Motif { get; set; }
    public DateTime DateConsultation { get; set; }
    public int DureeMinutes { get; set; }
    public decimal Tarif { get; set; }

    public decimal CalculerCoutHoraire()
    {
        if (DureeMinutes <= 0) return 0;
        return (Tarif / DureeMinutes) * 60;
    }
}
```

### 4.2 Port IConsultationRepository

**Fichier : `Domain/Ports/IConsultationRepository.cs`**

```csharp
namespace ConsultationService.Domain.Ports;

using ConsultationService.Domain.Entities;

public interface IConsultationRepository
{
    Task<IEnumerable<Consultation>> GetAllAsync();
    Task<Consultation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid patientId);
    Task<Consultation> CreateAsync(Consultation consultation);
    Task<Consultation?> UpdateAsync(Guid id, Consultation consultation);
    Task<bool> DeleteAsync(Guid id);
}
```

### 4.3 DTOs

**Fichier : `Application/DTOs/ConsultationRequestDto.cs`**

```csharp
namespace ConsultationService.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class ConsultationRequestDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public string Motif { get; set; } = string.Empty;

    [Required]
    public DateTime DateConsultation { get; set; }

    [Required]
    [Range(1, 480, ErrorMessage = "La durée doit être entre 1 et 480 minutes")]
    public int DureeMinutes { get; set; }

    [Required]
    [Range(0.01, 1000, ErrorMessage = "Le tarif doit être entre 0.01 et 1000 €")]
    public decimal Tarif { get; set; }
}
```

**Fichier : `Application/DTOs/ConsultationResponseDto.cs`**

```csharp
namespace ConsultationService.Application.DTOs;

public class ConsultationResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Motif { get; set; } = string.Empty;
    public DateTime DateConsultation { get; set; }
    public int DureeMinutes { get; set; }
    public decimal Tarif { get; set; }
}

public class CoutHoraireResponseDto
{
    public Guid ConsultationId { get; set; }
    public decimal Tarif { get; set; }
    public int DureeMinutes { get; set; }
    public decimal CoutHoraire { get; set; }
}
```

### 4.4 À vous de jouer !

En suivant le modèle du PatientService, implémentez :

1. **ConsultationMapper.cs** - Conversion Entity ↔ DTO
2. **IConsultationAppService.cs** - Interface du service applicatif
3. **ConsultationAppService.cs** - Implémentation avec calcul du coût horaire
4. **InMemoryConsultationRepository.cs** - Repository en mémoire
5. **ConsultationsController.cs** - Avec endpoint `/cout-horaire`

### Points d'attention

- L'endpoint `GET /api/consultations/{id}/cout-horaire` doit retourner un `CoutHoraireResponseDto`
- L'endpoint `GET /api/consultations/patient/{patientId}` filtre par patient
- Validez que le PatientId existe (optionnel pour ce workshop)

### Exemple de réponse pour `/cout-horaire`

```json
{
  "consultationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tarif": 50.00,
  "dureeMinutes": 30,
  "coutHoraire": 100.00
}
```

---

## Étape 5 : Implémentation du PrescriptionService

**Durée estimée : 45 minutes**

### 5.1 Entité Prescription

**Fichier : `Domain/Entities/Prescription.cs`**

```csharp
namespace PrescriptionService.Domain.Entities;

public class Prescription
{
    public Guid Id { get; set; }
    public Guid ConsultationId { get; set; }
    public string Medicament { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequence { get; set; } = string.Empty;
    public int DureeJours { get; set; }
    public bool Renouvelable { get; set; }

    public int CalculerTotalPrises()
    {
        // Parse la fréquence pour extraire le nombre de prises par jour
        // Ex: "3 fois/jour" -> 3
        var frequenceParJour = ExtractFrequenceParJour();
        return frequenceParJour * DureeJours;
    }

    private int ExtractFrequenceParJour()
    {
        // Extraction simple du premier nombre trouvé
        var match = System.Text.RegularExpressions.Regex.Match(Frequence, @"\d+");
        return match.Success ? int.Parse(match.Value) : 1;
    }
}
```

### 5.2 Port IPrescriptionRepository

**Fichier : `Domain/Ports/IPrescriptionRepository.cs`**

```csharp
namespace PrescriptionService.Domain.Ports;

using PrescriptionService.Domain.Entities;

public interface IPrescriptionRepository
{
    Task<IEnumerable<Prescription>> GetAllAsync();
    Task<Prescription?> GetByIdAsync(Guid id);
    Task<IEnumerable<Prescription>> GetByConsultationIdAsync(Guid consultationId);
    Task<Prescription> CreateAsync(Prescription prescription);
    Task<Prescription?> UpdateAsync(Guid id, Prescription prescription);
    Task<bool> DeleteAsync(Guid id);
}
```

### 5.3 DTOs

**Fichier : `Application/DTOs/PrescriptionRequestDto.cs`**

```csharp
namespace PrescriptionService.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class PrescriptionRequestDto
{
    [Required]
    public Guid ConsultationId { get; set; }

    [Required]
    [StringLength(200)]
    public string Medicament { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Frequence { get; set; } = string.Empty;

    [Required]
    [Range(1, 365)]
    public int DureeJours { get; set; }

    public bool Renouvelable { get; set; } = false;
}
```

**Fichier : `Application/DTOs/PrescriptionResponseDto.cs`**

```csharp
namespace PrescriptionService.Application.DTOs;

public class PrescriptionResponseDto
{
    public Guid Id { get; set; }
    public Guid ConsultationId { get; set; }
    public string Medicament { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequence { get; set; } = string.Empty;
    public int DureeJours { get; set; }
    public bool Renouvelable { get; set; }
}

public class TotalPrisesResponseDto
{
    public Guid PrescriptionId { get; set; }
    public string Medicament { get; set; } = string.Empty;
    public string Frequence { get; set; } = string.Empty;
    public int DureeJours { get; set; }
    public int TotalPrises { get; set; }
}
```

### 5.4 À vous de jouer !

Implémentez les fichiers restants en suivant le même pattern :

1. **PrescriptionMapper.cs**
2. **IPrescriptionAppService.cs**
3. **PrescriptionAppService.cs**
4. **InMemoryPrescriptionRepository.cs**
5. **PrescriptionsController.cs**
6. **Program.cs** avec l'injection de dépendances

### Exemple de test

```json
// POST /api/prescriptions
{
  "consultationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "medicament": "Paracétamol",
  "dosage": "500mg",
  "frequence": "3 fois/jour",
  "dureeJours": 7,
  "renouvelable": false
}
```

```json
// GET /api/prescriptions/{id}/total-prises
{
  "prescriptionId": "...",
  "medicament": "Paracétamol",
  "frequence": "3 fois/jour",
  "dureeJours": 7,
  "totalPrises": 21
}
```

---

## Étape 6 : Implémentation du HealthDashboardService

**Durée estimée : 60 minutes**

Ce service est le plus complexe car il doit communiquer avec les 3 autres microservices.

### 6.1 Configuration des HttpClients

**Fichier : `appsettings.json`**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ServiceUrls": {
    "PatientService": "http://localhost:5001",
    "ConsultationService": "http://localhost:5002",
    "PrescriptionService": "http://localhost:5003"
  }
}
```

### 6.2 DTOs pour les réponses des services externes

**Fichier : `Application/DTOs/ExternalDtos.cs`**

```csharp
namespace HealthDashboardService.Application.DTOs;

// DTOs pour désérialiser les réponses des autres services
public class PatientDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string GroupeSanguin { get; set; } = string.Empty;
}

public class ConsultationDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Motif { get; set; } = string.Empty;
    public decimal Tarif { get; set; }
}

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public Guid ConsultationId { get; set; }
    public string Medicament { get; set; } = string.Empty;
}
```

### 6.3 DTO Dashboard

**Fichier : `Application/DTOs/DashboardResponseDto.cs`**

```csharp
namespace HealthDashboardService.Application.DTOs;

public class DashboardResponseDto
{
    public int TotalPatients { get; set; }
    public Dictionary<string, int> ConsultationsParType { get; set; } = new();
    public decimal ChiffreAffairesTotal { get; set; }
    public int TotalPrescriptions { get; set; }
    public Dictionary<string, int> PatientsParGroupeSanguin { get; set; } = new();
    public DateTime GenereLe { get; set; } = DateTime.UtcNow;
}

public class PatientHistoriqueResponseDto
{
    public PatientDto Patient { get; set; } = null!;
    public List<ConsultationDto> Consultations { get; set; } = new();
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
}
```

### 6.4 Clients HTTP

**Fichier : `Infrastructure/HttpClients/PatientServiceClient.cs`**

```csharp
namespace HealthDashboardService.Infrastructure.HttpClients;

using System.Net.Http.Json;
using HealthDashboardService.Application.DTOs;

public interface IPatientServiceClient
{
    Task<List<PatientDto>> GetAllPatientsAsync();
    Task<PatientDto?> GetPatientByIdAsync(Guid id);
}

public class PatientServiceClient : IPatientServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PatientServiceClient> _logger;

    public PatientServiceClient(HttpClient httpClient, ILogger<PatientServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<PatientDto>> GetAllPatientsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PatientDto>>("api/patients");
            return response ?? new List<PatientDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des patients");
            return new List<PatientDto>();
        }
    }

    public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PatientDto>($"api/patients/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du patient {Id}", id);
            return null;
        }
    }
}
```

**Fichier : `Infrastructure/HttpClients/ConsultationServiceClient.cs`**

```csharp
namespace HealthDashboardService.Infrastructure.HttpClients;

using System.Net.Http.Json;
using HealthDashboardService.Application.DTOs;

public interface IConsultationServiceClient
{
    Task<List<ConsultationDto>> GetAllConsultationsAsync();
    Task<List<ConsultationDto>> GetConsultationsByPatientIdAsync(Guid patientId);
}

public class ConsultationServiceClient : IConsultationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConsultationServiceClient> _logger;

    public ConsultationServiceClient(HttpClient httpClient, ILogger<ConsultationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ConsultationDto>> GetAllConsultationsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ConsultationDto>>("api/consultations");
            return response ?? new List<ConsultationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des consultations");
            return new List<ConsultationDto>();
        }
    }

    public async Task<List<ConsultationDto>> GetConsultationsByPatientIdAsync(Guid patientId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ConsultationDto>>($"api/consultations/patient/{patientId}");
            return response ?? new List<ConsultationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des consultations du patient {Id}", patientId);
            return new List<ConsultationDto>();
        }
    }
}
```

**Fichier : `Infrastructure/HttpClients/PrescriptionServiceClient.cs`**

```csharp
namespace HealthDashboardService.Infrastructure.HttpClients;

using System.Net.Http.Json;
using HealthDashboardService.Application.DTOs;

public interface IPrescriptionServiceClient
{
    Task<List<PrescriptionDto>> GetAllPrescriptionsAsync();
    Task<List<PrescriptionDto>> GetPrescriptionsByConsultationIdAsync(Guid consultationId);
}

public class PrescriptionServiceClient : IPrescriptionServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PrescriptionServiceClient> _logger;

    public PrescriptionServiceClient(HttpClient httpClient, ILogger<PrescriptionServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<PrescriptionDto>> GetAllPrescriptionsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PrescriptionDto>>("api/prescriptions");
            return response ?? new List<PrescriptionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des prescriptions");
            return new List<PrescriptionDto>();
        }
    }

    public async Task<List<PrescriptionDto>> GetPrescriptionsByConsultationIdAsync(Guid consultationId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PrescriptionDto>>($"api/prescriptions/consultation/{consultationId}");
            return response ?? new List<PrescriptionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des prescriptions de la consultation {Id}", consultationId);
            return new List<PrescriptionDto>();
        }
    }
}
```

### 6.5 Service Dashboard

**Fichier : `Application/Services/DashboardService.cs`**

```csharp
namespace HealthDashboardService.Application.Services;

using HealthDashboardService.Application.DTOs;
using HealthDashboardService.Infrastructure.HttpClients;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync();
    Task<PatientHistoriqueResponseDto?> GetPatientHistoriqueAsync(Guid patientId);
}

public class DashboardService : IDashboardService
{
    private readonly IPatientServiceClient _patientClient;
    private readonly IConsultationServiceClient _consultationClient;
    private readonly IPrescriptionServiceClient _prescriptionClient;

    public DashboardService(
        IPatientServiceClient patientClient,
        IConsultationServiceClient consultationClient,
        IPrescriptionServiceClient prescriptionClient)
    {
        _patientClient = patientClient;
        _consultationClient = consultationClient;
        _prescriptionClient = prescriptionClient;
    }

    public async Task<DashboardResponseDto> GetDashboardAsync()
    {
        // Récupération parallèle des données
        var patientsTask = _patientClient.GetAllPatientsAsync();
        var consultationsTask = _consultationClient.GetAllConsultationsAsync();
        var prescriptionsTask = _prescriptionClient.GetAllPrescriptionsAsync();

        await Task.WhenAll(patientsTask, consultationsTask, prescriptionsTask);

        var patients = await patientsTask;
        var consultations = await consultationsTask;
        var prescriptions = await prescriptionsTask;

        return new DashboardResponseDto
        {
            TotalPatients = patients.Count,
            ConsultationsParType = consultations
                .GroupBy(c => c.Motif)
                .ToDictionary(g => g.Key, g => g.Count()),
            ChiffreAffairesTotal = consultations.Sum(c => c.Tarif),
            TotalPrescriptions = prescriptions.Count,
            PatientsParGroupeSanguin = patients
                .GroupBy(p => p.GroupeSanguin)
                .ToDictionary(g => g.Key, g => g.Count()),
            GenereLe = DateTime.UtcNow
        };
    }

    public async Task<PatientHistoriqueResponseDto?> GetPatientHistoriqueAsync(Guid patientId)
    {
        var patient = await _patientClient.GetPatientByIdAsync(patientId);
        if (patient == null) return null;

        var consultations = await _consultationClient.GetConsultationsByPatientIdAsync(patientId);
        
        var prescriptions = new List<PrescriptionDto>();
        foreach (var consultation in consultations)
        {
            var consultPrescriptions = await _prescriptionClient
                .GetPrescriptionsByConsultationIdAsync(consultation.Id);
            prescriptions.AddRange(consultPrescriptions);
        }

        return new PatientHistoriqueResponseDto
        {
            Patient = patient,
            Consultations = consultations,
            Prescriptions = prescriptions
        };
    }
}
```

### 6.6 Controller

**Fichier : `Api/Controllers/DashboardController.cs`**

```csharp
namespace HealthDashboardService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using HealthDashboardService.Application.Services;
using HealthDashboardService.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponseDto>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<PatientHistoriqueResponseDto>> GetPatientHistorique(Guid patientId)
    {
        var historique = await _dashboardService.GetPatientHistoriqueAsync(patientId);
        
        if (historique == null)
            return NotFound(new { message = $"Patient avec l'ID {patientId} non trouvé" });

        return Ok(historique);
    }
}
```

### 6.7 Configuration Program.cs

**Fichier : `Program.cs`**

```csharp
using HealthDashboardService.Application.Services;
using HealthDashboardService.Infrastructure.HttpClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Health Dashboard API", Version = "v1" });
});

// Configuration des HttpClients avec les URLs des services
var serviceUrls = builder.Configuration.GetSection("ServiceUrls");

builder.Services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrls["PatientService"]!);
});

builder.Services.AddHttpClient<IConsultationServiceClient, ConsultationServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrls["ConsultationService"]!);
});

builder.Services.AddHttpClient<IPrescriptionServiceClient, PrescriptionServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrls["PrescriptionService"]!);
});

// Services
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Étape 7 : Tests et validation

**Durée estimée : 30 minutes**

### 7.1 Lancement des services

Ouvrez 4 terminaux et lancez chaque service :

```bash
# Terminal 1
cd src/PatientService && dotnet run

# Terminal 2
cd src/ConsultationService && dotnet run

# Terminal 3
cd src/PrescriptionService && dotnet run

# Terminal 4
cd src/HealthDashboardService && dotnet run
```

### 7.2 Scénario de test complet

#### Étape A : Créer des patients

```bash
# Patient 1
curl -X POST http://localhost:5001/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "nom": "Marie Martin",
    "dateNaissance": "1985-03-15",
    "groupeSanguin": "A+"
  }'

# Patient 2
curl -X POST http://localhost:5001/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "nom": "Pierre Dubois",
    "dateNaissance": "1990-07-22",
    "groupeSanguin": "O-"
  }'

# Patient 3
curl -X POST http://localhost:5001/api/patients \
  -H "Content-Type: application/json" \
  -d '{
    "nom": "Sophie Bernard",
    "dateNaissance": "1978-11-08",
    "groupeSanguin": "B+"
  }'
```

#### Étape B : Créer des consultations

```bash
# Consultation pour Marie (remplacez {patientId} par l'ID réel)
curl -X POST http://localhost:5002/api/consultations \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "{patientId}",
    "motif": "Routine",
    "dateConsultation": "2024-01-15T09:00:00",
    "dureeMinutes": 30,
    "tarif": 25.00
  }'

# Consultation urgence pour Pierre
curl -X POST http://localhost:5002/api/consultations \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "{patientId}",
    "motif": "Urgence",
    "dateConsultation": "2024-01-16T14:30:00",
    "dureeMinutes": 45,
    "tarif": 50.00
  }'
```

#### Étape C : Créer des prescriptions

```bash
# Prescription (remplacez {consultationId} par l'ID réel)
curl -X POST http://localhost:5003/api/prescriptions \
  -H "Content-Type: application/json" \
  -d '{
    "consultationId": "{consultationId}",
    "medicament": "Paracétamol",
    "dosage": "500mg",
    "frequence": "3 fois/jour",
    "dureeJours": 5,
    "renouvelable": false
  }'
```

#### Étape D : Vérifier le Dashboard

```bash
# Dashboard global
curl http://localhost:5000/api/dashboard

# Historique d'un patient
curl http://localhost:5000/api/dashboard/patient/{patientId}
```

### 7.3 Collection Postman

Créez un fichier `healthpulse.postman_collection.json` pour faciliter les tests.

### ✅ Checklist de validation

- [ ] PatientService : CRUD complet fonctionnel
- [ ] ConsultationService : CRUD + calcul coût horaire
- [ ] PrescriptionService : CRUD + calcul total prises
- [ ] HealthDashboardService : Agrégation des 3 services
- [ ] Swagger accessible sur tous les services
- [ ] Gestion des erreurs (404, 400)

---

## Bonus

### Bonus 1 : API Gateway avec YARP

Créez un projet `ApiGateway` qui route les requêtes :

```bash
dotnet new webapi -n ApiGateway -o src/ApiGateway
cd src/ApiGateway
dotnet add package Yarp.ReverseProxy
```

**Configuration YARP (`appsettings.json`) :**

```json
{
  "ReverseProxy": {
    "Routes": {
      "patients-route": {
        "ClusterId": "patients-cluster",
        "Match": { "Path": "/api/patients/{**catch-all}" }
      },
      "consultations-route": {
        "ClusterId": "consultations-cluster",
        "Match": { "Path": "/api/consultations/{**catch-all}" }
      },
      "prescriptions-route": {
        "ClusterId": "prescriptions-cluster",
        "Match": { "Path": "/api/prescriptions/{**catch-all}" }
      },
      "dashboard-route": {
        "ClusterId": "dashboard-cluster",
        "Match": { "Path": "/api/dashboard/{**catch-all}" }
      }
    },
    "Clusters": {
      "patients-cluster": {
        "Destinations": { "destination1": { "Address": "http://localhost:5001" } }
      },
      "consultations-cluster": {
        "Destinations": { "destination1": { "Address": "http://localhost:5002" } }
      },
      "prescriptions-cluster": {
        "Destinations": { "destination1": { "Address": "http://localhost:5003" } }
      },
      "dashboard-cluster": {
        "Destinations": { "destination1": { "Address": "http://localhost:5000" } }
      }
    }
  }
}
```

### Bonus 2 : Dockerisation

**Dockerfile (exemple pour PatientService) :**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PatientService.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientService.dll"]
```

**docker-compose.yml :**

```yaml
version: '3.8'

services:
  patient-service:
    build:
      context: ./src/PatientService
      dockerfile: Dockerfile
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  consultation-service:
    build:
      context: ./src/ConsultationService
      dockerfile: Dockerfile
    ports:
      - "5002:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  prescription-service:
    build:
      context: ./src/PrescriptionService
      dockerfile: Dockerfile
    ports:
      - "5003:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  dashboard-service:
    build:
      context: ./src/HealthDashboardService
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ServiceUrls__PatientService=http://patient-service:80
      - ServiceUrls__ConsultationService=http://consultation-service:80
      - ServiceUrls__PrescriptionService=http://prescription-service:80
    depends_on:
      - patient-service
      - consultation-service
      - prescription-service
```

### Bonus 3 : Résilience avec Polly

```bash
dotnet add package Microsoft.Extensions.Http.Polly
```

```csharp
// Dans Program.cs du HealthDashboardService
builder.Services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrls["PatientService"]!);
})
.AddTransientHttpErrorPolicy(policy => 
    policy.WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
.AddTransientHttpErrorPolicy(policy => 
    policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
```

### Bonus 4 : Health Checks

```bash
dotnet add package AspNetCore.HealthChecks.Uris
```

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("http://localhost:5001/api/patients"), "patient-service")
    .AddUrlGroup(new Uri("http://localhost:5002/api/consultations"), "consultation-service")
    .AddUrlGroup(new Uri("http://localhost:5003/api/prescriptions"), "prescription-service");

app.MapHealthChecks("/health");
```

---

## Critères d'évaluation

| Critère | Points |
|---------|--------|
| Architecture hexagonale respectée | /4 |
| Endpoints fonctionnels | /4 |
| Communication inter-services | /3 |
| Validation des données | /2 |
| Gestion des erreurs | /2 |
| Code propre et organisé | /2 |
| Swagger documenté | /1 |
| Bonus réalisés | /2 |
| **Total** | **/20** |

---

## Conseils

1. **Commencez par PatientService** qui est le plus simple et sert de modèle
2. **Testez chaque service individuellement** avant de passer au Dashboard
3. **Utilisez des données en mémoire** (List ou Dictionary) pour simplifier
4. **N'oubliez pas d'enregistrer les services** dans le DI container
5. **Vérifiez les ports** si vous avez des erreurs de connexion
6. **Utilisez les logs** pour déboguer les communications inter-services

---

## Ressources

- [Documentation ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Architecture Hexagonale](https://alistair.cockburn.us/hexagonal-architecture/)
- [HttpClient Factory](https://docs.microsoft.com/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Polly Documentation](https://github.com/App-vNext/Polly)

---

**Bon courage ! 🚀**