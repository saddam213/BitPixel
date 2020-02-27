using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.AwardService.Implementation
{
	public class ClickAwardProcessor
	{
		public static async Task ProcessClicks(List<GameModel> games, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			var userClickGroups = clicks
							.GroupBy(x => x.UserId)
							.ToDictionary(k => k.Key, v => v.ToList());

			var userAwardGroups = awards
				.Where(x => x.ClickType == ClickType.Click)
				.GroupBy(x => x.UserId)
				.ToDictionary(k => k.Key, v => v.ToList());

			foreach (var userClicks in userClickGroups)
			{
				var userAwards = new List<AwardHistoryModel>();
				if (userAwardGroups.ContainsKey(userClicks.Key))
					userAwards = userAwardGroups[userClicks.Key];

				await ProcessUserClicks(userClicks.Key, userClicks.Value, userAwards, games);
			}
		}

		private static async Task ProcessUserClicks(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards, List<GameModel> games)
		{
			// User Awards
			await ProcessUserClickAwards(userId, clicks, awards);

			// Game Awards
			var gameClickGroups = clicks
				.GroupBy(x => x.GameId)
				.ToDictionary(k => k.Key, v => v.ToList());

			var gameAwardGroups = awards
				.Where(x => x.GameId.HasValue)
				.GroupBy(x => x.GameId)
				.ToDictionary(k => k.Key, v => v.ToList());

			foreach (var gameClickGroup in gameClickGroups)
			{
				var game = games.FirstOrDefault(x => x.Id == gameClickGroup.Key);
				if (game == null)
					continue;

				var gameAwards = new List<AwardHistoryModel>();
				if (gameAwardGroups.ContainsKey(game.Id))
					gameAwards = gameAwardGroups[game.Id];

				await ProcessGameClickAwards(userId, game, gameClickGroup.Value, gameAwards);
			}
		}

		private static async Task ProcessUserClickAwards(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			await ClickAwards.ClickCount(userId, clicks, awards);
			await ClickAwards.DailyClick(userId, clicks, awards);
			await ClickAwards.Number420(userId, clicks, awards);
			await ClickAwards.Number69(userId, clicks, awards);
			await ClickAwards.Number911(userId, clicks, awards);
		}

		private static async Task ProcessGameClickAwards(int userId, GameModel game, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			await ClickGameAwards.FourCorners(userId, game, clicks, awards);
			await ClickGameAwards.VerticalLines(userId, game, clicks, awards);
			await ClickGameAwards.HorizontalLines(userId, game, clicks, awards);
			await ClickGameAwards.BorderLines(userId, game, clicks, awards);
		}
	}
}
