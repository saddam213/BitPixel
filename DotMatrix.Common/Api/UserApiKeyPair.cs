using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Api
{
	public class UserApiKeyPair
	{
		public string Secret { get; set; }
		public string Key { get; set; }

		public bool IsValid
		{
			get { return !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Key); }
		}
	}
}
