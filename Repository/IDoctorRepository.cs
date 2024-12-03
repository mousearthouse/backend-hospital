using backend_email.Data;
using backend_email.Data.DTO;
using backend_email.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_email.Repository;

public interface IDoctorRepository
{
    bool IsUniqueDoctor(string username);
    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<Doctor> Register(DoctorRegister doctorRegister);
    Task<Doctor> GetDoctorByIdAsync(Guid doctorId);
    Task<Doctor> UpdateDoctorProfileAsync(Guid doctorId, DoctorEdit model);
}
