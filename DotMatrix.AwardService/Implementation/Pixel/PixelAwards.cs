using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class PixelAwards
	{
		public static async Task LastPixel(List<GameModel> games, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			foreach (var game in games)
			{
				if (game.EndType != GameEndType.LastPixel)
					continue;

				if (awards.Any(x => x.AwardType == AwardType.PixelLastPixel && x.GameId == game.Id))
					continue;

				// check if game is complete
				var gameSize = game.Width * game.Height;
				var gamePixels = pixels
					.Where(x => x.GameId == game.Id)
					.GroupBy(x => new { x.X, x.Y })
					.Select(x => x.OrderBy(y => y.Timestamp).First())
					.OrderBy(x => x.Timestamp)
					.ToList();
				if(gamePixels.Count == gameSize)
				{
					var lastPixel = gamePixels.LastOrDefault();
					var lastPixelVersion = $"X:{lastPixel.X}-Y:{lastPixel.Y}";

					await AwardEngine.InsertAward(AwardType.PixelLastPixel, lastPixel.UserId, game.Id, lastPixelVersion, lastPixelVersion);
					await AwardEngine.UpdateGameStatus(game.Id, GameStatus.Finished);
				}
			}
		}
	}
}
