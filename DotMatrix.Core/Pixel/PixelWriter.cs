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
				var key = $"{model.X}-{model.Y}";
				var existingPixel = await context.Pixel.Where(x => x.PixelKey == key).FirstOrDefaultAsync();
				if (existingPixel == null)
				{
					existingPixel = new Entity.Pixel
					{
						X = model.X,
						Y = model.Y,
						PixelKey = key,
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
				var newPixels = new List<Entity.Pixel>();
				foreach (var model in models)
				{
					var key = $"{model.X}-{model.Y}";
					var existingPixel = await context.Pixel.Where(x => x.PixelKey == key).FirstOrDefaultAsync();
					if (existingPixel == null)
					{
						existingPixel = new Entity.Pixel
						{
							X = model.X,
							Y = model.Y,
							PixelKey = key,
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
				}

				context.Pixel.AddRange(newPixels);
				await context.SaveChangesAsync();
				return true;
			}
		}
	}
}
