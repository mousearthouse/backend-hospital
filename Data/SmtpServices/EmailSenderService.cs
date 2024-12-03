using NETCore.MailKit.Infrastructure.Internal;
using MimeKit;


namespace backend_email.Data.SmtpServices;

public class EmailSenderService : IEmailService
{
    private readonly IEmailService _emailService;

    public EmailSenderService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Send(string toEmail, string subject, string body, SenderInfo senderInfo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderInfo.SenderName, senderInfo.SenderEmail));
        message.To.Add(new MailboxAddress(subject, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = body
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("localhost", 1025, false);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
}
