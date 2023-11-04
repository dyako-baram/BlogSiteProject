using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace API.Services;
public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Joey", "joey@friends.com"));
        message.To.Add(new MailboxAddress(email,email));
        
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = htmlMessage;
        
        message.Body = bodyBuilder.ToMessageBody();
        
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, false); 
            await client.AuthenticateAsync("5576162b2e3a78", "b351db409d18db"); 
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
    }

}