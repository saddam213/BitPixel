using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using DotMatrix.Enums;

namespace DotMatrix.Core.Pixel
{
	public class PixelReader : IPixelReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<PixelModel> GetPixel(int x, int y)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Pixel
					.Where(p => p.X == x && p.Y == y)
					.Select(MapPixel())
					.FirstOrDefaultAsync() ?? MapDefaultPixel(x, y);
			}
		}

		public async Task<List<PixelModel>> GetPixels()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var lastdate = DateTime.UtcNow.AddMinutes(-5);
				return await context.Pixel
					.Where(x => x.LastUpdate > lastdate)
					.Select(MapPixel())
					.ToListAsync();
			}
		}

		private async Task<IEnumerable<Entity.Pixel>> GetPixelRectangle(int x, int y, int width, int height)
		{
			using (var context = DataContextFactory.CreateConnection())
			{
				return await context.QueryAsync<Entity.Pixel>(StoredProcedure.GetPixelRectangle, new
				{
					X = x,
					Y = y,
					Width = width,
					Height = height
				}, commandType: CommandType.StoredProcedure);
			}
		}

		private static Expression<Func<Entity.Pixel, PixelModel>> MapPixel()
		{
			return p => new PixelModel
			{
				X = p.X,
				Y = p.Y,
				Type = p.Type,
				Color = p.Color,
				Owner = p.User.UserName,
				Team = p.User.Team.Name,
				Points = p.Points
			};
		}

		private static PixelModel MapDefaultPixel(int x, int y)
		{
			return new PixelModel
			{
				X = x,
				Y = y,
				Type = PixelType.Empty,
				Color = "#FFFFFF",
				Points = Constant.PixelPoints,
				Owner = "Empty Pixel",
				Team = "Empty Pixel"
			};
		}
	}
}
