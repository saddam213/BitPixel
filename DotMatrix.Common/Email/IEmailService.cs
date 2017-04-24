using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Email
{
	public interface IEmailService
	{
		Task SendEmail(string email, string subject, string body, bool isHtml = true);
	}
}
