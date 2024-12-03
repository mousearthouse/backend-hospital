using backend_email.Data;
using backend_email.Data.DTO;
using backend_email.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend_email.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDoctorRepository _repo;

    public DoctorController(AppDbContext context,
        IDoctorRepository repo)
    {
        _context = context;
        _repo = repo;

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var loginResponse = await _repo.Login(model);
        if (string.IsNullOrEmpty(loginResponse.Token))
        {
            return BadRequest(new { message = "smth not correct" });
        }
        return Ok(loginResponse.Token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DoctorRegister model)
    {
        bool ifUserNameUnique = _repo.IsUniqueDoctor(model.Email);
        if (!ifUserNameUnique)
        {
            return BadRequest(new { message = "user with this email already exists" });
        }
        var user = await _repo.Register(model);
        if (user == null)
        {
            return BadRequest(new { message = "Error while registering" });

        }
        return Ok(user);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetDoctorProfile()
    {
        var doctorIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        if (doctorIdClaim == null)
        {
            return Unauthorized(new { message = "ID wasn't found in token" });
        }

        var doctorId = Guid.Parse(doctorIdClaim);

        var doctor = await _repo.GetDoctorByIdAsync(doctorId);
        if (doctor == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        return Ok(new
        {
            doctor.Id,
            doctor.Name,
            doctor.Email,

        });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateDoctorProfile([FromBody] DoctorEdit model)
    {
        var doctorIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        if (doctorIdClaim == null)
        {
            return Unauthorized(new { message = "ID wasn't found in token" });
        }

        var doctorId = Guid.Parse(doctorIdClaim);

        var updatedDoctor = await _repo.UpdateDoctorProfileAsync(doctorId, model);
        if (updatedDoctor == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        return Ok(new
        {
            updatedDoctor.Id,
            updatedDoctor.Name,
            updatedDoctor.Email,
            updatedDoctor.Birthday,
            updatedDoctor.Gender,
            updatedDoctor.Phone,
        });
    }
}
