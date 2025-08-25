using System.Net;
using System.Net.Mail;

namespace Web.Api.Services;

public class EmailService(IConfiguration configuration)
{
    #region Variable
    private readonly IConfiguration config = configuration; 
    #endregion

    #region Send Email
    public async Task Send(string to, string subject, string html)
    {
        // Information from the app.setting for the email server setup
        var from = config["Email:From"];
        var display = config["Email:DisplayName"];
        var host = config["Email:SmtpHost"];
        var port = int.Parse(config["Email:SmtpPort"]!);
        var user = config["Email:User"];
        var password = config["Email:Password"];
        var ssl = bool.Parse(config["Email:UseSsl"]! ?? "false");

        // Get an instance of the smpt client
        using var client = new SmtpClient(host, port) { EnableSsl = ssl };

        // Get the credentials use to authenticate sender
        if (!string.IsNullOrWhiteSpace(user))
            client.Credentials = new NetworkCredential(user, password);

        // Setup the mail message oject
        using var msg = new MailMessage
        {
            From = new MailAddress(from!, display),
            Subject = subject,
            Body = html,
            IsBodyHtml = true
        };

        // The recipient of this mesaage
        msg.To.Add(to);

        // Send email
        await client.SendMailAsync(msg);
    } 
    #endregion
}
