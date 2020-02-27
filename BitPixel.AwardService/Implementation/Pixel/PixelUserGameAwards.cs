using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.AwardService.Implementation
{
	public class PixelUserGameAwards
	{
		#region VerticalLine

		public static async Task VerticalLines(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			// VerticalLines
			var lineGroups = pixels
				.Where(x => x.Type == PixelType.User)
				.Select(x => new { X = x.X, Y = x.Y })
				.GroupBy(x => x.X)
				.Select(x => new
				{
					X = x.Key,
					Count = x.Distinct().Count()
				})
				.Where(x => x.Count == game.Height)
				.ToList();

			foreach (var lineGroup in lineGroups)
			{
				var key = $"X:{lineGroup.X}";
				var version = $"Game:{game.Id}|{key}";
				if (awards.Any(x => x.AwardType == AwardType.PixelVerticalLine && x.Version == version))
					continue;

				await AwardEngine.InsertAward(AwardType.PixelVerticalLine, userId, game.Id, version, key);
			}
		}

		#endregion

		#region HorizontalLine

		public static async Task HorizontalLines(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			// HorizontalLines
			var lineGroups = pixels
				.Where(x => x.Type == PixelType.User)
				.Select(x => new { X = x.X, Y = x.Y })
				.GroupBy(x => x.Y)
				.Select(x => new
				{
					Y = x.Key,
					Count = x.Distinct()
									 .Count()
				})
				.Where(x => x.Count == game.Width)
				.ToList();

			foreach (var lineGroup in lineGroups)
			{
				var key = $"Y:{lineGroup.Y}";
				var version = $"Game:{game.Id}|{key}";
				if (awards.Any(x => x.AwardType == AwardType.PixelHorizontalLine && x.Version == version))
					continue;

				await AwardEngine.InsertAward(AwardType.PixelHorizontalLine, userId, game.Id, version, key);
			}
		}

		#endregion

		#region FourCorners

		public static async Task FourCorners(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			// Four Corners
			if (awards.Any(x => x.AwardType == AwardType.PixelFourCorners))
				return;

			var maxX = game.Width - 1;
			var maxY = game.Height - 1;
			var version = $"Game:{game.Id}";

			// top left
			if (!pixels.Any(x => x.X == 0 && x.Y == 0))
				return;
			// top right
			if (!pixels.Any(x => x.X == maxX && x.Y == 0))
				return;
			// bottom left
			if (!pixels.Any(x => x.X == 0 && x.Y == maxY))
				return;
			// bottom right
			if (!pixels.Any(x => x.X == maxX && x.Y == maxY))
				return;

			await AwardEngine.InsertAward(AwardType.PixelFourCorners, userId, game.Id, version, null);
		}

		#endregion

		#region BorderLines

		public static async Task BorderLines(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			if (awards.Any(x => x.AwardType == AwardType.PixelBorderLines))
				return;

			var version = $"Game:{game.Id}";

			// horizontal rows
			if (!awards.Any(x => x.AwardType == AwardType.PixelHorizontalLine && x.Version == $"{version}|Y:0")
			 || !awards.Any(x => x.AwardType == AwardType.PixelHorizontalLine && x.Version == $"{version}|Y:{game.Height - 1}"))
				return;

			// vertical rows
			if (!awards.Any(x => x.AwardType == AwardType.PixelVerticalLine && x.Version == $"{version}|X:{0}")
			 || !awards.Any(x => x.AwardType == AwardType.PixelVerticalLine && x.Version == $"{version}|X:{game.Width - 1}"))
				return;

			await AwardEngine.InsertAward(AwardType.PixelBorderLines, userId, game.Id, version, null);
		}

		#endregion

		#region ColorRGB

		public static async Task ColorRGB(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			if (awards.Any(x => x.AwardType == AwardType.ColorRGB))
				return;

			var userPixels = pixels.Where(x => x.Type == PixelType.User).ToList();
			// red
			if (!userPixels.Any(x => x.Color.Equals("#ff0000", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// green
			if (!userPixels.Any(x => x.Color.Equals("#00ff00", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// blue
			if (!userPixels.Any(x => x.Color.Equals("#0000ff", System.StringComparison.OrdinalIgnoreCase)))
				return;

			var version = $"Game:{game.Id}";
			await AwardEngine.InsertAward(AwardType.ColorRGB, userId, game.Id, version, null);
		}

		#endregion

		#region ColorBlind

		public static async Task ColorBlind(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			if (awards.Any(x => x.AwardType == AwardType.ColorBlind))
				return;

			var userPixels = pixels.Where(x => x.Type == PixelType.User).ToList();
			// white
			if (!userPixels.Any(x => x.Color.Equals("#ffffff", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// black
			if (!userPixels.Any(x => x.Color.Equals("#000000", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// grey
			if (!userPixels.Any(x => x.Color.Equals("#808080", System.StringComparison.OrdinalIgnoreCase)))
				return;

			var version = $"Game:{game.Id}";
			await AwardEngine.InsertAward(AwardType.ColorBlind, userId, game.Id, version, null);
		}

		#endregion

		#region ColorRainbow

		public static async Task ColorRainbow(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			if (awards.Any(x => x.AwardType == AwardType.ColorRainbow))
				return;

			var userPixels = pixels.Where(x => x.Type == PixelType.User).ToList();
			// Red
			if (!userPixels.Any(x => x.Color.Equals("#FF0000", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Orange
			if (!userPixels.Any(x => x.Color.Equals("#FF7F00", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Yellow
			if (!userPixels.Any(x => x.Color.Equals("#FFFF00", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Green
			if (!userPixels.Any(x => x.Color.Equals("#00FF00", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Blue
			if (!userPixels.Any(x => x.Color.Equals("#0000FF", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Indigo
			if (!userPixels.Any(x => x.Color.Equals("#4B0082", System.StringComparison.OrdinalIgnoreCase)))
				return;
			// Violet
			if (!userPixels.Any(x => x.Color.Equals("#9400D3", System.StringComparison.OrdinalIgnoreCase)))
				return;

			var version = $"Game:{game.Id}";
			await AwardEngine.InsertAward(AwardType.ColorRainbow, userId, game.Id, version, null);
		}

		#endregion

		#region ColorCount

		public static async Task ColorCount(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			var version = $"Game:{game.Id}";
			var userPixels = pixels.Where(x => x.Type == PixelType.User).ToList();
			var uniqueColors = userPixels.Select(x => x.Color).Distinct().Count();
			if (uniqueColors >= 10 && !awards.Any(x => x.AwardType == AwardType.Color10))
				await AwardEngine.InsertAward(AwardType.Color10, userId, game.Id, version, null);

			if (uniqueColors >= 100 && !awards.Any(x => x.AwardType == AwardType.Color100))
				await AwardEngine.InsertAward(AwardType.Color100, userId, game.Id, version, null);

			if (uniqueColors >= 1000 && !awards.Any(x => x.AwardType == AwardType.Color1000))
				await AwardEngine.InsertAward(AwardType.Color1000, userId, game.Id, version, null);
		}

		#endregion

	}
}
