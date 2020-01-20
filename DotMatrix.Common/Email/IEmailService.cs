using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.Common.Email
{
	public interface IEmailService
	{
		Task<bool> SendEmail(EmailTemplateType type, int userId, string destination, params object[] emailParameters);
	}
}
