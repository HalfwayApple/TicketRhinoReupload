using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using WebAPI.Models.Email;

namespace WebAPI.Services
{
	public interface IEmailService
	{
		Task<MimeMessage?> SendAsync(string from, string to, string subject, string html, byte[] qrCode);
	}

	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> settings)
		{
			_emailSettings = settings.Value;
		}

		public async Task<MimeMessage?> SendAsync(string from, string to, string subject, string body, byte[] qrCode)
		{
			try
			{
				// Build email
				var email = new MimeMessage();
				email.From.Add(MailboxAddress.Parse(from));
				email.To.Add(MailboxAddress.Parse(to));
				email.Subject = subject;

				var builder = new BodyBuilder();

				// Attach QR code
				var imageAttachment = builder.LinkedResources.Add("qrCode.png", qrCode, ContentType.Parse("image/png"));
				imageAttachment.ContentId = MimeUtils.GenerateMessageId();

				// Email design
				string htmlContent = $@"
					<html>
					<head>
					  <style>
						/* CSS styles for the email */
						.container {{
						  max-width: 600px;
						  margin: 0 auto;
						  padding: 20px;
						  background-color: #f0f0f0;
						}}
						h1 {{
						  color: #333333;
						  font-family: Arial, sans-serif;
						}}
						p {{
						  color: #666666;
						  font-family: Arial, sans-serif;
						}}
						img {{
						  width: 150px
						  height: 100%
						}}
					  </style>
					</head>
					<body>
					  <div class='container'>
						<h1>Welcome to My Email</h1>
						<p>{body}</p>
						<img src='cid:{imageAttachment.ContentId}' />
					  </div>
					</body>
					</html>";

				builder.HtmlBody = htmlContent;
				email.Body = builder.ToMessageBody();

				// Send email
				using (var smtp = new SmtpClient())
				{
					smtp.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
					smtp.Authenticate(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
					await smtp.SendAsync(email);
					await smtp.DisconnectAsync(true);
				}
				return email;
			}
			catch
			{
				return null;
			}
		}
	}
}
