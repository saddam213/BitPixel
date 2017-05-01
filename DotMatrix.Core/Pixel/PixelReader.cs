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
	public class PixelReader : IPixelReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<PixelModel>> GetPixels()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var lastdate = DateTime.UtcNow.AddMinutes(-4);
				return await context.Pixel
				.Where(x => x.LastUpdate > lastdate)
				.Select(x => new PixelModel
				{
					R = x.R,
					G = x.G,
					B = x.B,
					X = x.X,
					Y = x.Y
				}).ToListAsync();
			}
		}

		public async Task<PixelModel> GetPixel(int x, int y)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Pixel
					.Where(p => p.X == x && p.Y == y)
					.Select(p => new PixelModel
					{
						R = p.R,
						G = p.G,
						B = p.B,
						X = p.X,
						Y = p.Y
					}).FirstOrDefaultAsync() ?? new PixelModel { X = x, Y = y, R = 255, G = 255, B = 255 };
			}
		}
	}
}
