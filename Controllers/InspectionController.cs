using backend_email.Data.Models;
using backend_email.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_email.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionController : ControllerBase
{
    private readonly AppDbContext _context;

    public InspectionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInspectionById(Guid id)
    {
        var inspection = await _context.Inspections.FindAsync(id);
        if (inspection == null)
        {
            return NotFound();
        }

        return Ok(inspection);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInspection(Guid id, [FromBody] InspectionEdit inspectionEdit)
    {
        var inspection = await _context.Inspections.FindAsync(id);

        if (inspection == null)
        {
            return NotFound();
        }
        var inspectionAuthorId = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        if (inspection.DoctorId != inspectionAuthorId)
        {
            return BadRequest("Вы не являетесь автором этого осмотра");
        }

        inspection.Anamnesis = inspectionEdit.Anamnesis;
        inspection.Complaints = inspectionEdit.Complaints;
        inspection.Treatment = inspectionEdit.Treatment;
        inspection.Conclusion = inspectionEdit.Conclusion;
        inspection.NextVisitDate = inspectionEdit.NextVisitDate;
        inspection.DeathDate = inspectionEdit.DeathDate;

        inspection.Diagnoses.Clear();
        foreach (var diagnosis in inspectionEdit.Diagnoses)
        {
            var record = await _context.Icd10Records.FindAsync(id);

            inspection.Diagnoses.Add(new Diagnosis
            {
                Code = record.Code,
                Name = record.Name,
                Description = diagnosis.Description,
                Type = diagnosis.Type
            });
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
