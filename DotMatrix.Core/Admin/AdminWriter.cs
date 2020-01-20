using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DotMatrix.Common.Admin;
using DotMatrix.Common.DataContext;
using DotMatrix.Enums;

namespace DotMatrix.Core.Admin
{
	public class AdminWriter : IAdminWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<CreatePrizePoolResult> CreatePrizePool(CreatePrizePoolModel model)
		{
			var random = new Random();
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var dateTimeNow = DateTime.UtcNow;
				var newPrizes = new List<Entity.Prize>();
				var existingPixels = await context.Pixel
					.ToListAsync();
				var existingPrizes = await context.Prize
					.Where(x => !x.IsClaimed)
					.ToListAsync();
				for (int i = 0; i < model.Count; i++)
				{
					var locationX = random.Next(0, Constant.Width - 1);
					var locationY = random.Next(0, Constant.Height - 1);

					// Check Pixel rules
					var existingPixel = existingPixels.FirstOrDefault(x => x.X == locationX && x.Y == locationY);
					if (existingPixel != null)
					{
						if (existingPixel.Type == PixelType.Fixed)
							continue;

						if (model.MaxPoints > 0 && existingPixel.Points > model.MaxPoints)
							continue;
					}

					var existingPrize = existingPrizes.FirstOrDefault(x => x.X == locationX && x.Y == locationY);
					if (existingPrize != null)
						continue;

					// Check for duplicates
					if (newPrizes.Any(x => x.X == locationX && x.Y == locationY))
						continue;

					newPrizes.Add(new Entity.Prize
					{
						X = locationX,
						Y = locationY,
						Type = model.Type,
						Name = model.Name,
						Description = model.Description,
						Points = model.Points,
						Data = model.Data,
						Data2 = model.Data2,

						Status = PrizeStatus.Unclaimed,
						IsClaimed = false,
						Timestamp = dateTimeNow,
					});
				}

				if (!newPrizes.Any())
					return new CreatePrizePoolResult { Success = false, Message = "Failed to create any unique prize locations" };

				context.Prize.AddRange(newPrizes);
				await context.SaveChangesAsync();

				return new CreatePrizePoolResult
				{
					Success = false,
					Created = newPrizes.Count,
					Message = newPrizes.Count == model.Count
						? "Successfully created all prizes"
						: $"Successfully created {newPrizes.Count} of {model.Count} prizes"
				};
			}
		}
	}
}
