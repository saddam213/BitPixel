using System.Collections.Generic;

namespace DotMatrix.Common.Payment
{
	public class PaymentModel
	{
		public List<PaymentMethodModel> Methods { get; set; } = new List<PaymentMethodModel>();
	}
}
