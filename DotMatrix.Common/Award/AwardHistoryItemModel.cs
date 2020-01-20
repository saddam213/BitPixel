using System;
using DotMatrix.Enums;

namespace DotMatrix.Common.Award
{
	public class AwardHistoryItemModel
	{
		public string Name { get; set; }
		public string UserName { get; set; }
		public int Points { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
