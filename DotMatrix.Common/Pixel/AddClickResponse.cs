namespace DotMatrix.Common.Pixel
{

	public class AddClickResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int UserPoints { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }
	}
}
