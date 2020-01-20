namespace DotMatrix.Common.Pixel
{
	public class AddPixelResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public int PixelId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int TeamId { get; set; }
		public string TeamName { get; set; }
		public int NewPoints { get; set; }
	}
}
