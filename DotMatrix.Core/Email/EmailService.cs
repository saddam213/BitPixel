using DotMatrix.Common.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Email
{
	public class EmailService : IEmailService
	{
		private readonly string Email_Name = ConfigurationManager.AppSettings["Email_Name"];
		private readonly string Email_User = ConfigurationManager.AppSettings["Email_User"];
		private readonly string Email_Password = ConfigurationManager.AppSettings["Email_Pass"];
		private readonly string Email_From = ConfigurationManager.AppSettings["Email_From"];
		private readonly string Email_Server = ConfigurationManager.AppSettings["Email_Server"];
		private readonly int Email_Port = int.Parse(ConfigurationManager.AppSettings["Email_Port"]);

		public async Task SendEmail(string email, string subject, string body, bool isHtml = true)
		{
			using (var mailMessage = new MailMessage(new MailAddress(Email_From, Email_Name), new MailAddress(email)))
			{
				mailMessage.Subject = subject;
				mailMessage.Body = body;
				mailMessage.IsBodyHtml = isHtml;
				using (var mailClient = new SmtpClient(Email_Server, Email_Port))
				{
					mailClient.Credentials = new NetworkCredential(Email_User, Email_Password);
					mailClient.EnableSsl = true;
					await mailClient.SendMailAsync(mailMessage);
				}
			}
		}
	}
}
