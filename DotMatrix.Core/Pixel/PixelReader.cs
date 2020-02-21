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
using DotMatrix.Datatables;
using DotMatrix.Datatables.Models;
using DotMatrix.Enums;

namespace DotMatrix.Core.Pixel
{
	public class PixelReader : IPixelReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<PixelModel> GetPixel(int gameId, int x, int y)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Pixel
					.Where(p => p.GameId == gameId && p.X == x && p.Y == y)
					.Select(MapPixel())
					.FirstOrDefaultAsync() ?? MapDefaultPixel(x, y);
			}
		}

		public async Task<List<PixelModel>> GetPixels(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var lastdate = DateTime.UtcNow.AddMinutes(-5);
				return await context.Pixel
					.Where(x => x.GameId == gameId && x.LastUpdate > lastdate)
					.Select(MapPixel())
					.ToListAsync();
			}
		}

		private async Task<IEnumerable<Entity.Pixel>> GetPixelRectangle(int x, int y, int width, int height)
		{
			using (var context = DataContextFactory.CreateConnection())
			{
				return await context.QueryAsync<Entity.Pixel>(StoredProcedure.Game_GetPixelRectangle, new
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
				Player = p.User.UserName,
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
				Player = "Empty Pixel"
			};
		}

		public async Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.PixelHistory
					.Where(x => x.UserId == userId)
					.Select(x => new
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						X = x.Pixel.X,
						Y = x.Pixel.Y,
						Color = x.Color,
						Points = x.Points,
						Timestamp = x.Timestamp
					});
				if (maxCount.HasValue)
				{
					query = query
						.OrderByDescending(x => x.Timestamp)
						.Take(maxCount.Value);
				}
				return await query.GetDataTableResponseAsync(model);
			}
		}
	}
}
