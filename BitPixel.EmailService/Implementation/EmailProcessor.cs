using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BitPixel.Base;
using BitPixel.Base.Logging;
using BitPixel.Base.Processor;
using BitPixel.Data;
using BitPixel.Entity;
using BitPixel.Enums;
using SendGrid;
using SendGrid.Helpers.Mail;
using BitPixel.Common.DataContext;
using BitPixel.Data.DataContext;
using System.Data.Entity;

namespace BitPixel.EmailService.Implementation
{
	public class EmailProcessor : ProcessorBase
	{
		private TimeSpan _pollPeriod = TimeSpan.FromSeconds(10);
		private IDataContextFactory _dataContextFactory = new DataContextFactory();
		private static readonly Log Log = LoggingManager.GetLog(typeof(EmailProcessor));
		private readonly string _sendGrid_Api_Key = ConfigurationManager.AppSettings["SendGrid_Api_Key"];

		protected override TimeSpan ProcessPeriod
		{
			get { return _pollPeriod; }
		}

		protected override async Task Process()
		{
			Log.Message(LogLevel.Debug, "[Process] - Process Start...");
			var processStart = DateTime.UtcNow;
			await ProcessEmails().ConfigureAwait(false);
			var processTime = DateTime.UtcNow - processStart;
			Log.Message(LogLevel.Debug, "[Process] - Process End, Time: {0}", processTime);
		}

		private async Task ProcessEmails()
		{
			Log.Message(LogLevel.Info, "[ProcessEmails] - ProcessEmails started...");
			using (var context = _dataContextFactory.CreateContext())
			{
				var templates = await context.EmailTemplate
					.Where(x => x.IsEnabled)
					.ToListAsync();
				var emails = await context.EmailOutbox
					.Where(x => x.Status == EmailStatus.Pending)
					.ToListAsync();
				if (emails.IsNullOrEmpty())
				{
					Log.Message(LogLevel.Info, "[ProcessEmails] - No pending emails found.");
					return;
				}

				Log.Message(LogLevel.Info, "[ProcessEmails] - {0} pending emails found.", emails.Count);
				foreach (var email in emails)
				{
					Log.Message(LogLevel.Info, "[ProcessEmails] - Processing email, Id: {0}.", email.Id);
					var template = templates.FirstOrDefault(x => x.Type == email.Type && x.Culture.Equals(email.UserCulture, StringComparison.OrdinalIgnoreCase))
											?? templates.FirstOrDefault(x => x.Type == email.Type && x.Culture.Equals("en-US", StringComparison.OrdinalIgnoreCase));
					if (template == null)
					{
						email.Status = EmailStatus.Failed;
						email.Error = "Template missing.";
					}
					else
					{
						email.Status = await SendEmail(template, email);
					}
					email.Updated = DateTime.UtcNow;
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, "[ProcessEmails] - Processed email, Id: {0}, Status: {1}", email.Id, email.Status);
				}
				Log.Message(LogLevel.Info, "[ProcessEmails] - ProcessEmails complete.");
			}
		}

		private async Task<EmailStatus> SendEmail(EmailTemplate template, EmailOutbox email)
		{
			try
			{
				var parameters = JsonConvert.DeserializeObject<object[]>(email.Parameters);
				var body = string.Format(template.Template ?? "", parameters);
				var bodyHtml = string.Format(template.TemplateHtml, parameters).Replace("{{", "{").Replace("}}", "}");
				var subject = string.Format(template.Subject, parameters);
				var message = MailHelper.CreateSingleEmail(MailHelper.StringToEmailAddress(template.FromAddress), MailHelper.StringToEmailAddress(email.Destination), subject, body, bodyHtml);

				var client = new SendGridClient(_sendGrid_Api_Key);
				var response = await client.SendEmailAsync(message, CancellationToken);
				if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
					return EmailStatus.Processed;

				email.Error = response.StatusCode.ToString();
			}
			catch (Exception ex)
			{
				email.Error = ex.Message;
				Log.Exception("[SendEmail]", ex);
			}
			return EmailStatus.Failed;
		}
	}
}
