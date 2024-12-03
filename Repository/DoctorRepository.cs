using backend_email.Data;
using backend_email.Data.DTO;
using backend_email.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend_email.Repository;

public class DoctorRepository : IDoctorRepository
{
    private readonly AppDbContext _context;
    private string secretKey;
    public DoctorRepository(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }

    public bool IsUniqueDoctor(string email)
    {
        var user = _context.Doctors.FirstOrDefault(x => x.Email == email);
        if (user == null)
        {
            return true;
        }
        return false;
    }   

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        // тут должна быть проверка на пароль
        var user = _context.Doctors.FirstOrDefault();
        if (user == null)
        {
            return new LoginResponseDTO()
            {
                Token = ""
            };
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                //new Claim(ClaimTypes.Name, user.Name)

            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
        };
        return loginResponseDTO;
    }

    public async Task<Doctor> Register(DoctorRegister doctorRegister)
    {
        Doctor user = new Doctor()
        {
            Name = doctorRegister.Name,
            Email = doctorRegister.Email,
            Birthday = doctorRegister.Birthday,
            Gender = doctorRegister.Gender,
            Phone = doctorRegister.Phone,

        };

        _context.Doctors.Add(user);
        await _context.SaveChangesAsync();
        // тут сбрасывают пароль созданному доктору,
        // но у меня и так у доктора нет пароля
        return user;
    }

    public async Task<Doctor> GetDoctorByIdAsync(Guid doctorId)
    {
        return await _context.Doctors.FindAsync(doctorId);
    }

    public async Task<Doctor> UpdateDoctorProfileAsync(Guid doctorId, DoctorEdit model)
    {
        var doctor = await _context.Doctors.FindAsync(doctorId);
        if (doctor == null) return null;

        doctor.Name = model.Name ?? doctor.Name;
        doctor.Email = model.Email ?? doctor.Email;
        doctor.Birthday = model.Birthday ?? doctor.Birthday;
        doctor.Phone = model.Phone ?? doctor.Phone;

        await _context.SaveChangesAsync();
        return doctor;
    }

}
