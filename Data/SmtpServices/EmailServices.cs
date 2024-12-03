using MailKit.Net.Smtp;
using MimeKit;
using NETCore.MailKit.Infrastructure.Internal;
using System.Net.Mail;

namespace backend_email.Data.SmtpServices;
public class Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public Message(string name, string address, string subject, string content)
    {
        To = new List<MailboxAddress> { new MailboxAddress(name, address) };
        Subject = subject;
        Content = content;
    }
}

public interface IEmailService
{
    Task Send(string to, string subject, string content, SenderInfo senderInfo);
}

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    public EmailService(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }

    public async Task Send(string to, string subject, string content, SenderInfo senderInfo)
    {
        using (var client = new System.Net.Mail.SmtpClient("localhost", 1025))
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderInfo.SenderEmail, senderInfo.SenderName),
                Subject = subject,
                Body = content,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}

