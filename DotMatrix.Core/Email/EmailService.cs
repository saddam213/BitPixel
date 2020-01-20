using System;
using System.Threading;
using System.Threading.Tasks;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Email;
using DotMatrix.Entity;
using DotMatrix.Enums;

using Newtonsoft.Json;

namespace DotMatrix.Core.Email
{
	public class EmailService : IEmailService
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> SendEmail(EmailTemplateType type, int userId, string destination, params object[] emailParameters)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				context.EmailOutbox.Add(new EmailOutbox
				{
					UserId = userId,
					Type = type,
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Status = EmailStatus.Pending,
					Destination = destination,
					Parameters = JsonConvert.SerializeObject(emailParameters),
					UserCulture = Thread.CurrentThread.CurrentUICulture.Name
				});
				await context.SaveChangesAsync();
				return true;
			}
		}
	}
}
