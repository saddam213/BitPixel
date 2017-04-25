using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Pixel
{
	public class PixelWriter : IPixelWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> AddOrUpdate(string userId, PixelModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existingPixel = await context.Pixel.Where(x => x.X == model.X && x.Y == model.Y).FirstOrDefaultAsync();
				if (existingPixel == null)
				{
					existingPixel = new Entity.Pixel
					{
						X = model.X,
						Y = model.Y,
					
						Price = 0.000000005m,
						Color = "#FFFFFF",
						History = new List<Entity.PixelHistory>()
					};
					context.Pixel.Add(existingPixel);
				}

				existingPixel.UserId = userId;
				existingPixel.Color = model.Color;
				existingPixel.Price *= 2;
				existingPixel.LastUpdate = DateTime.UtcNow;
				existingPixel.History.Add(new Entity.PixelHistory
				{
					UserId = userId,
					Color = existingPixel.Color,
					Price = existingPixel.Price,
					Timestamp = existingPixel.LastUpdate
				});

				await context.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> AddOrUpdate(string userId, List<PixelModel> models)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				foreach (var model in models)
				{
					var existingPixel = await context.Pixel.Where(x => x.X == model.X && x.Y == model.Y).FirstOrDefaultAsync();
					if (existingPixel == null)
					{
						existingPixel = new Entity.Pixel
						{
							X = model.X,
							Y = model.Y,
							Price = 0.000000005m,
							Color = "#FFFFFF"
						};
						context.Pixel.Add(existingPixel);
						await context.SaveChangesAsync();
					}

					existingPixel.Color = model.Color;
					existingPixel.Price *= 2;
					existingPixel.LastUpdate = DateTime.UtcNow;

					var history = new Entity.PixelHistory
					{
						UserId = userId,
						PixelId = existingPixel.Id,
						Color = existingPixel.Color,
						Price = existingPixel.Price
					};
					context.PixelHistory.Add(history);
				}

				await context.SaveChangesAsync();
				return true;
			}
		}
	}
}
