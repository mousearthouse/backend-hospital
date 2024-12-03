using backend_email.Data;
using backend_email.Data.Models;
using backend_email.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Xml.Linq;
using backend_email.Data.SmtpServices;
using Org.BouncyCastle.Asn1.Ocsp;

namespace backend_email.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatient patient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var newPatient = new Patient
        {
            Name = patient.Name,
            Birthday = patient.Birthday,
            Gender = patient.Gender
        };
        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();

        return Ok(newPatient);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = patient.Id,
            name = patient.Name,
            birthday = patient.Birthday,
            gender = patient.Gender,

        });
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPatients(
        string? request = "",
        int page = 1,
        int size = 10,
        PatientSorting sortBy = PatientSorting.NameAsc)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(request))
        {
            query = query.Where(patient =>
                EF.Functions.ILike(patient.Name, $"%{request}%"));
        }


        query = sortBy switch
        {
            PatientSorting.NameAsc => query.OrderBy(p => p.Name),
            PatientSorting.NameDesc => query.OrderByDescending(p => p.Name),
            PatientSorting.CreateAsc => query.OrderBy(p => p.CreateTime),
            PatientSorting.CreateDesc => query.OrderByDescending(p => p.CreateTime),
            PatientSorting.InspectionAsc => query.OrderBy(p => p.CreateTime),
            // тут надо разобраться с PatientSorting.InspectionAsc :(
            _ => query.OrderBy(p => p.Name)
        };

        var totalItems = await query.CountAsync();
        var patients = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var response = new
        {
            TotalItems = totalItems,
            Page = page,
            Size = size,
            Patients = patients
        };

        return Ok(response);
    }


    [HttpPost("{id}/inspections")]
    [Authorize]
    public async Task<IActionResult> CreateInspection([FromBody] InspectionCreate createdInspection, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // ограничения

        if (createdInspection.Date > DateTime.UtcNow)
        {
            return BadRequest("Дата осмотра не может быть в будущем");
        }

        var lastInspection = await _context.Inspections
        .Where(i => i.PatientId == id)
        .OrderByDescending(i => i.CreateTime)
        .FirstOrDefaultAsync();

        if (lastInspection != null && createdInspection.Date < lastInspection.Date)
        {
            return BadRequest("Новый осмотр не может быть сделан ранее предыдущего осмотра");
        }

        var mainDiagnosisCount = createdInspection.Diagnoses.Count(d => d.Type == DiagnosisType.Main);
        if (mainDiagnosisCount != 1)
        {
            return BadRequest("Осмотр должен иметь только один диагноз с типом 'Основной'");
        }

        if (createdInspection.Conclusion == Conclusion.Disease && createdInspection.NextVisitDate == null)
        {
            return BadRequest("Необходимо указать дату следующего визита при заключении 'Болезнь'");
        }
        else if (createdInspection.Conclusion == Conclusion.Death && createdInspection.DeathDate == null)
        {
            return BadRequest("Необходимо указать дату смерти при заключении 'Смерть'");
        }
        else if (createdInspection.Conclusion == Conclusion.Recovery && (createdInspection.NextVisitDate != null || createdInspection.DeathDate != null))
        {
            return BadRequest("Для заключения 'Выздоровление' не нужно указывать дату следующего визита или смерти");
        }

        var hasDeathConclusion = await _context.Inspections
        .AnyAsync(i => i.PatientId == id && i.Conclusion == Conclusion.Death);

        if (createdInspection.Conclusion == Conclusion.Death && hasDeathConclusion)
        {
            return BadRequest("Пациент не может иметь более одного осмотра с заключением 'Смерть'");
        }

        var specialities = createdInspection.Consultations.Select(c => c.SpecialityId).ToList();
        if (specialities.Count != specialities.Distinct().Count())
        {
            return BadRequest("Осмотр не может иметь несколько консультаций с одинаковой специальностью");
        }


        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        var inspection = new Inspection
        {
            Date = createdInspection.Date,
            Anamnesis = createdInspection.Anamnesis,
            Complaints = createdInspection.Complaints,
            Treatment = createdInspection.Treatment,
            Conclusion = createdInspection.Conclusion,
            NextVisitDate = createdInspection.NextVisitDate,
            DeathDate = createdInspection.DeathDate,
            PreviousInspectionId = createdInspection.PreviousInspectionId,
            PatientId = patient.Id,
            DoctorId = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value)
        };

        foreach (var diagnosis in createdInspection.Diagnoses)
        {
            var record = _context.Icd10Records.Where(x => x.Id == diagnosis.IcdDiagnosisId).FirstOrDefault();

            inspection.Diagnoses.Add(new Diagnosis
            {
                Code = record.Code,
                Name = record.Name,
                Description = diagnosis.Description,
                Type = diagnosis.Type
            });
        };

        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync();

        List<InspectionConsultation> resultConsultations = [];
        if (createdInspection.Consultations != null)
        {
            var consultations = createdInspection.Consultations;
           
            foreach (var consultation in consultations)
            {
                var speciality = await _context.Specialities.FindAsync(id);
                var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var consultationToAdd = new InspectionConsultation
                {
                    InspectionId = inspection.Id.ToString(),
                    Speciality = speciality,
                    RootComment = new InspectionComment
                    { 
                        ParentId = inspection.Id.ToString(),
                        Content = createdInspection.Consultations[0].Comment.Content,
                        Author = await _context.Doctors.FindAsync(doctorId)
                    }       
                };
                resultConsultations.Add(consultationToAdd);
                _context.Consultations.Add(new Consultation
                {
                    InspectionId = inspection.Id.ToString(),
                    Speciality = speciality,
                });

            }
        }
        inspection.Consultations = resultConsultations;
        await _context.SaveChangesAsync();
        return Ok(inspection);
    }


    [HttpGet("{id}/inspections")]
    [Authorize]
    public async Task<IActionResult> GetPatientInspections(Guid id)
    {
        var inspections = await _context.Inspections.Where(inspection => inspection.PatientId==id).ToListAsync();
        return Ok(inspections);
    }


    [HttpGet("{id}/inspections/search")]
    public async Task<IActionResult> GetPatientInspectionsWithoutChildren(
    Guid id,
    string? request = null
    )
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        var query = _context.Inspections
            .Where(i => i.PatientId == id && i.PreviousInspectionId == null)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request))
        {
            query = query.Where(i => i.Diagnoses.Any(d => d.Code.Contains(request)) || i.Diagnoses.Any(d => d.Name.Contains(request)));
        }

        var inspections = await query.ToListAsync();

        if (!inspections.Any())
        {
            return NotFound("Нет осмотров без дочерних с такими диагнозами.");
        }

        return Ok(inspections);
    }

}
