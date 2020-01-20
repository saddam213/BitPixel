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

		public async Task<ScoreViewModel> GetScoreboard()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var pixelBoard = await context.PixelHistory
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var prizeBoard = await context.Prize
					.Where(x => x.IsClaimed && x.UserId.HasValue)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var awardBoard = await context.AwardHistory
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var clickBoard = await context.Click
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Count()
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var pointsWonBoard = await context.Prize
					.Where(x => x.IsClaimed && x.UserId.HasValue)
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Sum(p => p.Points)
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();

				var pointsLostBoard = await context.PixelHistory
					.GroupBy(x => x.User.UserName)
					.Select(x => new ScoreViewItemModel
					{
						UserName = x.Key,
						Count = x.Sum(p => p.Points)
					})
					.OrderByDescending(x => x.Count)
					.Take(25)
					.ToListAsync();
				return new ScoreViewModel
				{
					PixelBoard = pixelBoard,
					PrizeBoard = prizeBoard,
					AwardBoard = awardBoard,
					ClickBoard = clickBoard,
					PointsWonBoard = pointsWonBoard,
					PointsLostBoard = pointsLostBoard
				};
			}
		}
	}
}
