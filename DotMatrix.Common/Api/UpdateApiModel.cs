using System.ComponentModel.DataAnnotations;

namespace DotMatrix.Common.Api
{
	public class UpdateApiModel
	{
		public bool IsApiEnabled { get; set; }

		[Required]
		public string ApiKey { get; set; }

		[Required]
		public string ApiSecret { get; set; }
	}
}
