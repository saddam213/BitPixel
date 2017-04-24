using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
