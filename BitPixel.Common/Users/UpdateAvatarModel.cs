using System.Collections.Generic;

namespace BitPixel.Common.Users
{
	public class UpdateAvatarModel
	{
		public string UserName { get; set; }
		public string AvatarPath { get; set; }
		public List<AvatarPixel> Pixels { get; set; }
	}

	public class AvatarPixel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Color { get; set; }
	}
}
