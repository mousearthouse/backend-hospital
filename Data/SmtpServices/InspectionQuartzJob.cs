using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using NETCore.MailKit.Infrastructure.Internal;
using Quartz;

namespace backend_email.Data.SmtpServices;

public class MissedInspectionJob : IJob
{
    private IEmailService _emailSenderService;
    private AppDbContext _context;

    public MissedInspectionJob(IEmailService emailSenderService, AppDbContext context)
    {
        _emailSenderService = emailSenderService;
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_context.Inspections.Count() == 0)
        {
            return;
        }
        var missedInspections = _context.Inspections
            .Where(i => i.NextVisitDate.HasValue && i.NextVisitDate < DateTime.UtcNow && !i.IsNotified)
            .Include(i => i.Doctor)
            .ToList();

        foreach (var inspection in missedInspections)
        {
            var patient = await _context.Patients.FindAsync(inspection.PatientId);
            var doctorEmail = inspection.Doctor.Email;
            var subject = "Пропущенный визит";
            var body = $"Пациент {patient.Name} (ID {inspection.PatientId}) пропустил(а) визит, запланированный на {inspection.NextVisitDate}.";
            var senderInfo = new SenderInfo
            {
                SenderEmail = "no-reply-hospital@gmail.com",
                SenderName = "Медицинская система"
            };
            await _emailSenderService.Send(doctorEmail, subject, body, senderInfo);
            inspection.IsNotified = true;
        }
        await _context.SaveChangesAsync();
    }
}

