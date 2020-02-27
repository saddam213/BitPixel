using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.AwardService.Implementation
{
	public class PixelAwardProcessor
	{
		public static async Task ProcessPixels(List<GameModel> games, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			await ProcessPixelAwards(games, pixels, awards);

			var fixedPixels = pixels
				.Where(x => x.Type == PixelType.Fixed)
				.ToList();

			var userPixelGroups = pixels
								.Where(x => x.Type == PixelType.User)
								.GroupBy(x => x.UserId)
								.ToDictionary(k => k.Key, v => v.ToList());

			var userAwardGroups = awards
				.Where(x => x.ClickType == ClickType.Pixel)
				.GroupBy(x => x.UserId)
				.ToDictionary(k => k.Key, v => v.ToList());

			foreach (var userPixels in userPixelGroups)
			{
				var userAwards = new List<AwardHistoryModel>();
				if (userAwardGroups.ContainsKey(userPixels.Key))
					userAwards = userAwardGroups[userPixels.Key];

				userPixels.Value.AddRange(fixedPixels);
				await ProcessUserPixels(userPixels.Key, userPixels.Value, userAwards, games);
			}
		}


		private static async Task ProcessUserPixels(int userId, List<PixelModel> pixels, List<AwardHistoryModel> awards, List<GameModel> games)
		{
			// User Awards
			await ProcessUserPixelAwards(userId, pixels, awards);

			// Game Awards
			var gamePixelGroups = pixels
				.GroupBy(x => x.GameId)
				.ToDictionary(k => k.Key, v => v.ToList());

			var gameAwardGroups = awards
				.Where(x => x.GameId.HasValue)
				.GroupBy(x => x.GameId)
				.ToDictionary(k => k.Key, v => v.ToList());

			foreach (var gamePixelGroup in gamePixelGroups)
			{
				var game = games.FirstOrDefault(x => x.Id == gamePixelGroup.Key);
				if (game == null)
					continue;

				var gameAwards = new List<AwardHistoryModel>();
				if (gameAwardGroups.ContainsKey(game.Id))
					gameAwards = gameAwardGroups[game.Id];

				await ProcessUserGamePixelAwards(userId, game, gamePixelGroup.Value, gameAwards);
			}
		}

		private static async Task ProcessPixelAwards(List<GameModel> games, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			await PixelAwards.LastPixel(games, pixels, awards);
		}

		private static async Task ProcessUserPixelAwards(int userId, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			await PixelUserAwards.PixelCount(userId, pixels, awards);
			await PixelUserAwards.DailyPixel(userId, pixels, awards);
			await PixelUserAwards.OverwriteCount(userId, pixels, awards);
		}

		private static async Task ProcessUserGamePixelAwards(int userId, GameModel game, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			await PixelUserGameAwards.FourCorners(userId, game, pixels, awards);
			await PixelUserGameAwards.VerticalLines(userId, game, pixels, awards);
			await PixelUserGameAwards.HorizontalLines(userId, game, pixels, awards);
			await PixelUserGameAwards.BorderLines(userId, game, pixels, awards);
			await PixelUserGameAwards.ColorRGB(userId, game, pixels, awards);
			await PixelUserGameAwards.ColorBlind(userId, game, pixels, awards);
			await PixelUserGameAwards.ColorRainbow(userId, game, pixels, awards);
			await PixelUserGameAwards.ColorCount(userId, game, pixels, awards);
		}
	}
}
