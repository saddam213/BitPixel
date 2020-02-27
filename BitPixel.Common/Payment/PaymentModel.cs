using System.Collections.Generic;

namespace BitPixel.Common.Payment
{
	public class PaymentModel
	{
		public List<PaymentMethodModel> Methods { get; set; } = new List<PaymentMethodModel>();
	}
}
