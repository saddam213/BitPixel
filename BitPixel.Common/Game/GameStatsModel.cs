using System;
using System.Collections.Generic;

namespace BitPixel.Common.Game
{
	public class GameStatsModel
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public int FixedPixels { get; set; }
		public int UserPixels { get; set; }
		public int Prizes { get; set; }
		public int PrizesFound { get; set; }
		public List<TeamStatsModel> TeamStats { get; set; }

		public int TotalPixels
		{
			get { return Width * Height; }
		}
	
		public int GamePixels
		{
			get { return TotalPixels - FixedPixels; }
		}
	
		public int PixelsRemaining
		{
			get { return GamePixels - UserPixels; }
		}

		public double PixelsRemainingPercent
		{
			get 
			{
				if (UserPixels == 0 || GamePixels == 0)
					return 100.00;

			return Math.Max(0, 100.0 - ((double)UserPixels / (double)GamePixels) * 100.0);
			}
		}

		public int PrizesRemaining
		{
			get { return Prizes - PrizesFound; }
		}

		public double PrizesRemainingPercent
		{
			get
			{
				if (Prizes == 0 || PrizesFound == 0)
					return 100.00;

				return Math.Max(0, 100.0 - ((double)PrizesFound / (double)Prizes) * 100.0);
			}
		}
	}
}
