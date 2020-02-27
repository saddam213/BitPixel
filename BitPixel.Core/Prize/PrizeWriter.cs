using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Common.DataContext;
using BitPixel.Common.Game;
using BitPixel.Common.Prize;
using BitPixel.Common.Results;
using BitPixel.Enums;

namespace BitPixel.Core.Prize
{
	public class PrizeWriter : IPrizeWriter
	{
		public IGameReader GameReader { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> ClaimPrize(int userId, ClaimPrizeModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var prize = await context.Prize.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);
				if (prize == null)
					return new WriterResult(false, "Prize not found");

				if (prize.Status != PrizeStatus.Unclaimed)
					return new WriterResult(false, $"Unable to claim prize in {prize.Status} status");

				// Data3 = address
				prize.Data3 = model.Data3;
				prize.Status = PrizeStatus.Pending;
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> CreatePrizePool(CreatePrizePoolModel model)
		{
			var random = new Random();
			var game = await GameReader.GetGame(model.GameId.Value);
			using (var context = DataContextFactory.CreateContext())
			{
				var dateTimeNow = DateTime.UtcNow;
				var newPrizes = new List<Entity.Prize>();
				var existingPixels = await context.Pixel
					.Where(x => x.GameId == model.GameId)
					.ToListAsync();
				var existingPrizes = await context.Prize
					.Where(x => x.GameId == model.GameId && !x.IsClaimed)
					.ToListAsync();

				if (existingPrizes.Any(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
					return new WriterResult(false, "Prize pool name already exists");

				var totalPixels = game.Width * game.Height;
				var usedPixels = existingPixels.Count + existingPrizes.Count;
				var remainingPixels = totalPixels - usedPixels;
				if (model.Count > remainingPixels)
					return new WriterResult(false, "Not enough free pixels to distribute prize pool");

				while (newPrizes.Count < model.Count)
				{
					var locationX = random.Next(0, game.Width - 1);
					var locationY = random.Next(0, game.Height - 1);

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

						GameId = model.GameId.Value
					});
				}

				context.Prize.AddRange(newPrizes);
				await context.SaveChangesAsync();

				return new WriterResult(true, "Successfully created all prizes");
			}
		}



		public async Task<IWriterResult> UpdatePrizePool(UpdatePrizePoolModel model)
		{
			var game = await GameReader.GetGame(model.GameId);
			using (var context = DataContextFactory.CreateContext())
			{
				var existingPrizes = await context.Prize
					.Where(x => x.GameId == game.Id)
					.ToListAsync();

				if (!model.Name.Equals(model.NewName, StringComparison.OrdinalIgnoreCase))
				{
					if (existingPrizes.Any(x => x.Name.Equals(model.NewName, StringComparison.OrdinalIgnoreCase)))
						return new WriterResult(false, "Prize pool name already exists");
				}

				foreach (var existingPrize in existingPrizes.Where(x => x.Name == model.Name))
				{
					existingPrize.Name = model.NewName;
					existingPrize.Description = model.Description;
				}

				await context.SaveChangesAsync();
				return new WriterResult(true, "Successfully updated prizes");
			}
		}

		public async Task<IWriterResult> UpdatePrizePayment(UpdatePrizePaymenModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var prize = await context.Prize
					.Where(x => x.IsClaimed && x.Id == model.Id && x.Type == PrizeType.Crypto)
					.FirstOrDefaultAsync();
				if (prize == null)
					return new WriterResult(false, "Prize not found");

				prize.Status = model.Status;
				prize.Data3 = model.Data3;
				prize.Data4 = model.Data4;
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}
	}
}
