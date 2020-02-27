using BitPixel.Enums;

namespace BitPixel.Common.Prize
{
	public class UpdatePrizePaymenModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public PrizeStatus Status { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Data3 { get; set; }
		public string Data4 { get; set; }
	}
}
