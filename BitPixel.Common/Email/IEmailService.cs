using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.Common.Email
{
	public interface IEmailService
	{
		Task<bool> SendEmail(EmailTemplateType type, int userId, string destination, params object[] emailParameters);
	}
}
