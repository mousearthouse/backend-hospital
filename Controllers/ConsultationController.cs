using backend_email.Data;
using backend_email.Data.DTO;
using backend_email.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend_email.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetConsultationById(Guid id)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = consultation.Id,
            createTime = consultation.CreateTime,
            inspectionId = consultation.InspectionId,
            speciality = consultation.Speciality,
            comments = consultation.Comments,
        });
    }

    [HttpPost("{id}/comment")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CommentCreate commentCreate, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        var doctor = await _context.Doctors.FindAsync(doctorId);

        var consultation = await _context.Consultations.FindAsync(id);
        // разобраться что за что отвечает
        var comment = new Comment
        {
            Content = commentCreate.Content,
            ParentId = consultation.Id,
            AuthorId = doctorId,
            Author = doctor.Name,
        };


        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment.Id);
    }

    [HttpPut("comment/{id}")]
    [Authorize]
    public async Task<IActionResult> EditComment([FromBody] InspectionCommentCreate inspectionCommentCreate, Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        comment.Content = inspectionCommentCreate.Content;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
