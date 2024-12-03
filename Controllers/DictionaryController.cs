using backend_email.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace backend_email.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DictionaryController : ControllerBase
{
    private readonly AppDbContext _context;

    public DictionaryController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("speciality")]
    public async Task<IActionResult> GetSpecialities([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be greater than 0");
        }

        var totalSpecialities = await _context.Specialities.CountAsync();
        var specialities = await _context.Specialities
        .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new
        {
            TotalCount = totalSpecialities,
            Page = page,
            Size = size,
            Specialities = specialities
        });
    }


    [HttpGet("icd10")]
    public async Task<IActionResult> SearchIcd10Records(string? request = "", int page = 1, int size = 10)
    {
        var query = _context.Icd10Records.AsQueryable();

        if (!string.IsNullOrEmpty(request))
        {
            query = query.Where(record =>
                EF.Functions.ILike(record.Name, $"%{request}%") ||
                EF.Functions.ILike(record.Code, $"%{request}%"));
        }

        var totalItems = await query.CountAsync();
        var records = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = size,
            Records = records
        });
    }
    

    [HttpGet("icd10/roots")]
    public async Task<IActionResult> GetIcd10Roots()
    {
        var rootElements = await _context.Icd10Records
            .Where(record => record.IdParent == null)
            .ToListAsync();
       
        Console.Write(rootElements);

        return Ok(rootElements);
    }
}
