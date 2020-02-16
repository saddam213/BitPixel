using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class ClickGameAwards
	{
		#region VerticalLine

		public static async Task VerticalLines(int userId, GameModel game, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			// VerticalLines
			var lineGroups = clicks
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
				if (awards.Any(x => x.AwardType == AwardType.ClickVerticalLine && x.Version == version))
					continue;

				await AwardEngine.InsertAward(AwardType.ClickVerticalLine, userId, game.Id, version, key);
			}
		}

		#endregion

		#region HorizontalLine

		public static async Task HorizontalLines(int userId, GameModel game, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			// HorizontalLines
			var lineGroups = clicks
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
				if (awards.Any(x => x.AwardType == AwardType.ClickHorizontalLine && x.Version == version))
					continue;

				await AwardEngine.InsertAward(AwardType.ClickHorizontalLine, userId, game.Id, version, key);
			}
		}

		#endregion

		#region FourCorners

		public static async Task FourCorners(int userId, GameModel game, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			// Four Corners
			if (awards.Any(x => x.AwardType == AwardType.ClickFourCorners))
				return;

			var maxX = game.Width - 1;
			var maxY = game.Height - 1;
			var version = $"Game:{game.Id}";

			// top left
			if (!clicks.Any(x => x.X == 0 && x.Y == 0))
				return;
			// top right
			if (!clicks.Any(x => x.X == maxX && x.Y == 0))
				return;
			// bottom left
			if (!clicks.Any(x => x.X == 0 && x.Y == maxY))
				return;
			// bottom right
			if (!clicks.Any(x => x.X == maxX && x.Y == maxY))
				return;

			await AwardEngine.InsertAward(AwardType.ClickFourCorners, userId, game.Id, version, null);
		}

		#endregion

		#region BorderLines

		public static async Task BorderLines(int userId, GameModel game, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			if (awards.Any(x => x.AwardType == AwardType.ClickBorderLines))
				return;

			var version = $"Game:{game.Id}";

			// horizontal rows
			if (!awards.Any(x => x.AwardType == AwardType.ClickHorizontalLine && x.Version == $"{version}|Y:0")
			 || !awards.Any(x => x.AwardType == AwardType.ClickHorizontalLine && x.Version == $"{version}|Y:{game.Height - 1}"))
				return;

			// vertical rows
			if (!awards.Any(x => x.AwardType == AwardType.ClickVerticalLine && x.Version == $"{version}|X:{0}")
			 || !awards.Any(x => x.AwardType == AwardType.ClickVerticalLine && x.Version == $"{version}|X:{game.Width - 1}"))
				return;

			await AwardEngine.InsertAward(AwardType.ClickBorderLines, userId, game.Id, version, null);
		}

		#endregion
	}
}
