using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Score;
using DotMatrix.Enums;

namespace DotMatrix.Core.Score
{
	public class ScoreReader : IScoreReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<ScoresModel> GetScoreboard()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var pixelBoard = await context.PixelHistory
					.Where(x => x.UserId != Constant.SystemUserId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var prizeBoard = await context.Prize
					.Where(x => x.UserId != Constant.SystemUserId)
					.Where(x => x.IsClaimed && x.UserId.HasValue)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var awardBoard = await context.AwardHistory
					.Where(x => x.UserId != Constant.SystemUserId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var clickBoard = await context.Click
					.Where(x => x.UserId != Constant.SystemUserId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				return new ScoresModel
				{
					PixelBoard = pixelBoard,
					PrizeBoard = prizeBoard,
					AwardBoard = awardBoard,
					ClickBoard = clickBoard
				};
			}
		}

		public async Task<ScoresModel> GetScoreboard(int gameId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var pixelBoard = await context.PixelHistory
					.Where(x => x.UserId != Constant.SystemUserId)
					.Where(x => x.GameId == gameId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var prizeBoard = await context.Prize
					.Where(x => x.UserId != Constant.SystemUserId)
					.Where(x => x.GameId == gameId && x.IsClaimed && x.UserId.HasValue)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var awardBoard = await context.AwardHistory
					.Where(x => x.UserId != Constant.SystemUserId)
					.Where(x => x.GameId == gameId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var clickBoard = await context.Click
					.Where(x => x.UserId != Constant.SystemUserId)
					.Where(x => x.GameId == gameId)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				return new ScoresModel
				{
					PixelBoard = pixelBoard,
					PrizeBoard = prizeBoard,
					AwardBoard = awardBoard,
					ClickBoard = clickBoard
				};
			}
		}
	}
}
