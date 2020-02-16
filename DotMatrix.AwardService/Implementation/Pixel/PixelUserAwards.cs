using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public class PixelUserAwards
	{
		#region PixelCount

		public static async Task PixelCount(int userId, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			var version = $"User:{userId}";
			if (pixels.Count >= 100 && !awards.Any(x => x.AwardType == AwardType.Pixel100))
				await AwardEngine.InsertAward(AwardType.Pixel100, userId, null, version, null);

			if (pixels.Count >= 1000 && !awards.Any(x => x.AwardType == AwardType.Pixel1000))
				await AwardEngine.InsertAward(AwardType.Pixel1000, userId, null, version, null);

			if (pixels.Count >= 10000 && !awards.Any(x => x.AwardType == AwardType.Pixel10000))
				await AwardEngine.InsertAward(AwardType.Pixel10000, userId, null, version, null);

			if (pixels.Count >= 100000 && !awards.Any(x => x.AwardType == AwardType.Pixel100000))
				await AwardEngine.InsertAward(AwardType.Pixel100000, userId, null, version, null);

			if (pixels.Count >= 1000000 && !awards.Any(x => x.AwardType == AwardType.Pixel1000000))
				await AwardEngine.InsertAward(AwardType.Pixel1000000, userId, null, version, null);
		}

		#endregion

		#region DailyPixel

		public static async Task DailyPixel(int userId, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			// daily pixel
			var dailyPixels = pixels
				.Where(x => x.Type == PixelType.User)
				.Select(x => x.Timestamp.ToString("dd/MM/yyyy"))
				.Distinct();
			foreach (var dailyPixel in dailyPixels)
			{
				if (awards.Any(x => x.AwardType == AwardType.PixelDaily && x.Version == dailyPixel))
					continue;

				// new daily pixel award
				await AwardEngine.InsertAward(AwardType.PixelDaily, userId, null, dailyPixel, dailyPixel);
			}
		}

		#endregion

		#region OverwriteCount

		public static async Task OverwriteCount(int userId, List<PixelModel> pixels, List<AwardHistoryModel> awards)
		{
			var version = $"User:{userId}";
			var overwritePixels = pixels.Where(x => x.Points > 1).ToList();
			if (overwritePixels.Count >= 1 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite, userId, null, version, null);

			if (overwritePixels.Count >= 100 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite100))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite100, userId, null, version, null);

			if (overwritePixels.Count >= 1000 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite1000))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite1000, userId, null, version, null);

			if (overwritePixels.Count >= 10000 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite10000))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite10000, userId, null, version, null);

			if (overwritePixels.Count >= 100000 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite100000))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite100000, userId, null, version, null);

			if (overwritePixels.Count >= 1000000 && !awards.Any(x => x.AwardType == AwardType.PixelOverwrite1000000))
				await AwardEngine.InsertAward(AwardType.PixelOverwrite1000000, userId, null, version, null);
		}

		#endregion
	}
}
