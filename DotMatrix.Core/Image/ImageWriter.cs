using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Image;
using DotMatrix.Common.Results;
using DotMatrix.Common.Users;
using DotMatrix.Enums;

namespace DotMatrix.Core.Image
{
	public class ImageWriter : IImageWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateAvatarImage(UpdateAvatarModel model)
		{
			try
			{
				foreach (var pixel in model.Pixels)
				{
					if (!Constant.IsValidPixel(pixel.X, pixel.Y, 10, 10, pixel.Color))
						return new WriterResult(false, $"Invalid pixel, X:{pixel.X}, Y:{pixel.Y}, Color:{pixel.Color}");
				}

				using (var bitmapSmall = new Bitmap(10, 10))
				using (var bitmapMedium = new Bitmap(100, 100))
				using (var bitmapLarge = new Bitmap(200, 200))
				{
					foreach (var pixel in model.Pixels)
					{
						var color = ColorTranslator.FromHtml(pixel.Color);
						bitmapSmall.SetPixel(pixel.X, pixel.Y, color);

						var medX = pixel.X * 10;
						var medY = pixel.Y * 10;
						for (int x = 0; x < 10; x++)
							for (int y = 0; y < 10; y++)
								bitmapMedium.SetPixel(medX + x, medY + y, color);

						var lrgX = pixel.X * 20;
						var lrgY = pixel.Y * 20;
						for (int x = 0; x < 20; x++)
							for (int y = 0; y < 20; y++)
								bitmapLarge.SetPixel(lrgX + x, lrgY + y, color);
					}

					bitmapSmall.Save(Path.Combine(model.AvatarPath, $"{model.UserName}-small.png"), System.Drawing.Imaging.ImageFormat.Png);
					bitmapMedium.Save(Path.Combine(model.AvatarPath, $"{model.UserName}-medium.png"), System.Drawing.Imaging.ImageFormat.Png);
					bitmapLarge.Save(Path.Combine(model.AvatarPath, $"{model.UserName}-large.png"), System.Drawing.Imaging.ImageFormat.Png);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				return new WriterResult(false, $"Failed to build avatar, unknown error");
			}
		}

		public async Task<IWriterResult> CreateFixedImage(CreateFixedImageModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			using (var bitmap = new Bitmap(model.ImageStream))
			{
				var game = await context.Games
					.Where(x => x.Status == Enums.GameStatus.NotStarted && x.Id == model.GameId)
					.FirstOrDefaultAsync();
				if (game == null)
					return new WriterResult(false, "Game not found");

				var width = bitmap.Width;
				var height = bitmap.Height;

				if (model.X + width > game.Width)
					return new WriterResult(false, "X + Width = Out of bounds");

				if (model.Y + height > game.Height)
					return new WriterResult(false, "Y + Height = Out of bounds");

				var imagePixels = new List<Entity.Pixel>();
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						var color = bitmap.GetPixel(x, y);
						if(color.A == 0){
							continue;
						}

						var hexColor = ColorTranslator.ToHtml(color);
						imagePixels.Add(new Entity.Pixel
						{
							UserId = Constant.SystemUserId,
							GameId = game.Id,
							X = model.X + x,
							Y = model.Y + y,
							Points = 1,
							Color = hexColor,
							LastUpdate = DateTime.UtcNow,
							Type = model.IsFixed
								? PixelType.Fixed
								: PixelType.User
						});
					}
				}

				context.Pixel.AddRange(imagePixels);
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}


	}
}
