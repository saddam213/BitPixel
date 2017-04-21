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
				return await context.Pixel.Select(x => new PixelModel
				{
					B = x.B,
					G = x.G,
					R = x.R,
					X = x.X,
					Y = x.Y
				}).ToListAsync();
			}
		}
	}
}
