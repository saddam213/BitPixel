using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Enums;

namespace BitPixel.AwardService.Implementation
{
	public class ClickAwards
	{
		#region ClickCount

		public static async Task ClickCount(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			var version = $"User:{userId}";
			if (clicks.Count >= 100 && !awards.Any(x => x.AwardType == AwardType.Click100))
				await AwardEngine.InsertAward(AwardType.Click100, userId, null, version, null);

			if (clicks.Count >= 1000 && !awards.Any(x => x.AwardType == AwardType.Click1000))
				await AwardEngine.InsertAward(AwardType.Click1000, userId, null, version, null);

			if (clicks.Count >= 10000 && !awards.Any(x => x.AwardType == AwardType.Click10000))
				await AwardEngine.InsertAward(AwardType.Click10000, userId, null, version, null);

			if (clicks.Count >= 100000 && !awards.Any(x => x.AwardType == AwardType.Click100000))
				await AwardEngine.InsertAward(AwardType.Click100000, userId, null, version, null);

			if (clicks.Count >= 1000000 && !awards.Any(x => x.AwardType == AwardType.Click1000000))
				await AwardEngine.InsertAward(AwardType.Click1000000, userId, null, version, null);
		}

		#endregion

		#region DailyClick

		public static async Task DailyClick(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			// daily pixel
			var dailyClicks = clicks
				.Select(x => x.Timestamp.ToString("dd/MM/yyyy"))
				.Distinct();
			foreach (var dailyClick in dailyClicks)
			{
				if (awards.Any(x => x.AwardType == AwardType.ClickDaily && x.Version == dailyClick))
					continue;

				// new daily pixel award
				await AwardEngine.InsertAward(AwardType.ClickDaily, userId, null, dailyClick, dailyClick);
			}
		}

		#endregion

		#region Number420

		public static async Task Number420(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			var version1 = $"X:{420} Y:{0}";
			if (clicks.Any(a => a.X == 420 && a.Y == 0) && !awards.Any(x => x.AwardType == AwardType.Number420 && x.Version == version1))
				await AwardEngine.InsertAward(AwardType.Number420, userId, null, version1, null);

			var version2 = $"X:{0} Y:{420}";
			if (clicks.Any(a => a.X == 0 && a.Y == 420) && !awards.Any(x => x.AwardType == AwardType.Number420 && x.Version == version2))
				await AwardEngine.InsertAward(AwardType.Number420, userId, null, version2, null);

			var version3 = $"X:{420} Y:{420}";
			if (clicks.Any(a => a.X == 420 && a.Y == 420) && !awards.Any(x => x.AwardType == AwardType.Number420 && x.Version == version3))
				await AwardEngine.InsertAward(AwardType.Number420, userId, null, version3, null);

			var version4 = $"X:{4} Y:{20}";
			if (clicks.Any(a => a.X == 4 && a.Y == 20) && !awards.Any(x => x.AwardType == AwardType.Number420 && x.Version == version4))
				await AwardEngine.InsertAward(AwardType.Number420, userId, null, version4, null);

			var version5 = $"X:{20} Y:{4}";
			if (clicks.Any(a => a.X == 20 && a.Y == 4) && !awards.Any(x => x.AwardType == AwardType.Number420 && x.Version == version5))
				await AwardEngine.InsertAward(AwardType.Number420, userId, null, version5, null);
		}

		#endregion

		#region Number69

		public static async Task Number69(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			var version1 = $"X:{69} Y:{0}";
			if (clicks.Any(a => a.X == 69 && a.Y == 0) && !awards.Any(x => x.AwardType == AwardType.Number69 && x.Version == version1))
				await AwardEngine.InsertAward(AwardType.Number69, userId, null, version1, null);

			var version2 = $"X:{0} Y:{69}";
			if (clicks.Any(a => a.X == 0 && a.Y == 69) && !awards.Any(x => x.AwardType == AwardType.Number69 && x.Version == version2))
				await AwardEngine.InsertAward(AwardType.Number69, userId, null, version2, null);

			var version3 = $"X:{69} Y:{69}";
			if (clicks.Any(a => a.X == 69 && a.Y == 69) && !awards.Any(x => x.AwardType == AwardType.Number69 && x.Version == version3))
				await AwardEngine.InsertAward(AwardType.Number69, userId, null, version3, null);

			var version4 = $"X:{6} Y:{9}";
			if (clicks.Any(a => a.X == 4 && a.Y == 9) && !awards.Any(x => x.AwardType == AwardType.Number69 && x.Version == version4))
				await AwardEngine.InsertAward(AwardType.Number69, userId, null, version4, null);

			var version5 = $"X:{9} Y:{6}";
			if (clicks.Any(a => a.X == 9 && a.Y == 6) && !awards.Any(x => x.AwardType == AwardType.Number69 && x.Version == version5))
				await AwardEngine.InsertAward(AwardType.Number69, userId, null, version5, null);
		}

		#endregion

		#region Number911

		public static async Task Number911(int userId, List<ClickModel> clicks, List<AwardHistoryModel> awards)
		{
			var version1 = $"X:{9} Y:{11}";
			if (clicks.Any(a => a.X == 9 && a.Y == 11) && !awards.Any(x => x.AwardType == AwardType.Number911 && x.Version == version1))
				await AwardEngine.InsertAward(AwardType.Number911, userId, null, version1, null);

			var version2 = $"X:{11} Y:{9}";
			if (clicks.Any(a => a.X == 11 && a.Y == 9) && !awards.Any(x => x.AwardType == AwardType.Number911 && x.Version == version2))
				await AwardEngine.InsertAward(AwardType.Number911, userId, null, version2, null);
		}

		#endregion
	}
}
