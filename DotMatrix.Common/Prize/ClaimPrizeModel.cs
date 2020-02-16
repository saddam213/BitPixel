using System.ComponentModel.DataAnnotations;
using DotMatrix.Enums;

namespace DotMatrix.Common.Prize
{
	public class ClaimPrizeModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Game { get; set; }
		public int Points { get; set; }
		public PrizeStatus Status { get; set; }
		public PrizeType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		[Required]
		public string Data3 { get; set; }
		public string Data4 { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
	}
}
