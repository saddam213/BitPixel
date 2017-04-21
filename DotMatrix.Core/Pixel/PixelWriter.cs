using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
						Price = 0.000000005m
					};
					context.Pixel.Add(existingPixel);
					await context.SaveChangesAsync();
				}

				existingPixel.R = model.R;
				existingPixel.G = model.G;
				existingPixel.B = model.B;
				existingPixel.Price *= 2;
				existingPixel.LastUpdate = DateTime.UtcNow;

				var history = new Entity.PixelHistory
				{
					UserId = userId,
					PixelId = existingPixel.Id,
					R = existingPixel.R,
					G = existingPixel.G,
					B = existingPixel.B,
					Price = existingPixel.Price
				};
				context.PixelHistory.Add(history);
				await context.SaveChangesAsync();

				return true;
			}
		}
	}
}
