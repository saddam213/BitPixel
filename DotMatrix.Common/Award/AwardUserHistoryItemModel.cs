using System;
using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AwardUserHistoryItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
