using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Base.Logging;
using DotMatrix.Base.Processor;
using DotMatrix.Common.DataContext;
using DotMatrix.Data.DataContext;
using DotMatrix.Enums;

namespace DotMatrix.AwardService.Implementation
{
	public partial class AwardEngine : ProcessorBase
	{
		private TimeSpan _pollPeriod = TimeSpan.FromSeconds(5);

		private static readonly Log Log = LoggingManager.GetLog(typeof(AwardEngine));

		private static IDataContextFactory DataContextFactory = new DataContextFactory();
		private static NotificationProxy NotificationProxy = new NotificationProxy();

		protected override TimeSpan ProcessPeriod
		{
			get { return _pollPeriod; }
		}

		protected override async Task Process()
		{
			Log.Message(LogLevel.Debug, "[Process] - Process Start...");
			var processStart = DateTime.UtcNow;
			await ProcessAwards();
			var processTime = DateTime.UtcNow - processStart;
			Log.Message(LogLevel.Debug, "[Process] - Process End, Time: {0}", processTime);
		}

		private async Task ProcessAwards()
		{
			Log.Message(LogLevel.Info, "[ProcessAwards] - ProcessAwards started...");

			var games = new List<GameModel>();
			var clicks = new List<ClickModel>();
			var pixels = new List<PixelModel>();
			var awards = new List<AwardHistoryModel>();
			Log.Message(LogLevel.Info, "[ProcessAwards] - Get process data started...");
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				pixels = await context.PixelHistory
					.Where(x => x.Game.Status == GameStatus.Started)
					.Select(x => new PixelModel
					{
						Id = x.Id,
						Type = PixelType.User,
						UserId = x.UserId,
						GameId = x.GameId,
						X = x.Pixel.X,
						Y = x.Pixel.Y,
						Color = x.Color,
						Points = x.Points,
						Timestamp = x.Timestamp
					}).ToListAsync();

				pixels.AddRange
				(
					await context.Pixel
						.Where(x => x.Type == PixelType.Fixed)
						.Select(x => new PixelModel
						{
							Id = x.Id,
							Type = PixelType.Fixed,
							UserId = 0,
							GameId = x.GameId,
							X = x.X,
							Y = x.Y,
							Color = x.Color,
							Points = x.Points,
							Timestamp = x.LastUpdate
						}).ToListAsync()
				);

				clicks = await context.Click
					.Where(x => x.Game.Status == GameStatus.Started)
					.Select(x => new ClickModel
					{
						Id = x.Id,
						UserId = x.UserId,
						GameId = x.GameId,
						ClickType = x.Type,
						X = x.X,
						Y = x.Y,
						Timestamp = x.Timestamp
					}).ToListAsync();

				awards = await context.AwardHistory
					.Where(x => x.Award.Type.HasValue)
					.Select(x => new AwardHistoryModel
					{
						Id = x.Id,
						AwardId = x.AwardId,
						UserId = x.UserId,
						GameId = x.GameId,
						Version = x.Version,
						VersionData = x.VersionData,
						AwardType = x.Award.Type.Value,
						ClickType = x.Award.ClickType
					}).ToListAsync();

				games = await context.Games
					.Where(x => x.Status == GameStatus.Started)
					.Select(x => new GameModel
					{
						Id = x.Id,
						Width = x.Width,
						Height = x.Height
					}).ToListAsync();
			}
			Log.Message(LogLevel.Info, "[ProcessAwards] - Get process data complete.");

			await ClickAwardProcessor.ProcessClicks(games, clicks, awards);
			await PixelAwardProcessor.ProcessPixels(games, pixels, awards);

			Log.Message(LogLevel.Info, "[ProcessAwards] - ProcessAwards complete.");
		}

		public static async Task<bool> InsertAward(AwardType type, int userId, int? gameId, string version, string versionData)
		{
			try
			{
				AddAwardResult result = null;
				Log.Message(LogLevel.Info, "[InsertAward] - Inserting award...");
				using (var connection = DataContextFactory.CreateConnection())
				{
					result = await connection.QueryFirstOrDefaultAsync<AddAwardResult>(StoredProcedure.Award_AddAwardHistory, new
					{
						GameId = gameId,
						UserId = userId,
						Type = type,
						Version = version,
						VersionData = versionData
					}, commandType: System.Data.CommandType.StoredProcedure);
				}

				if (!string.IsNullOrEmpty(result.Error))
				{
					Log.Message(LogLevel.Error, $"[ProcessAwards] - Error inserting award, Error: {result.Error}");
					return false;
				}

				await NotificationProxy.InternalSendAward(result);
				Log.Message(LogLevel.Info, "[InsertAward] - Inserting award complete.");
				return true;
			}
			catch (Exception ex)
			{
				Log.Message(LogLevel.Error, $"[ProcessAwards] - Exception inserting award, Error: {ex.Message}");
			}
			return false;
		}

		public static async Task<bool> UpdateGameStatus(int gameId, GameStatus status)
		{
			try
			{

				Log.Message(LogLevel.Info, $"[UpdateGameStatus] - Updating game status, GameId: {gameId}, Status: {status}");
				using (var context = DataContextFactory.CreateContext())
				{
					var game = await context.Games.FirstOrDefaultAsync(x => x.Id == gameId);
					if (game == null)
						return false;

					game.Status = status;
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, "[UpdateGameStatus] - Updating game status award complete.");
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Message(LogLevel.Error, $"[UpdateGameStatus] - Exception updating game status, Error: {ex.Message}");
			}
			return false;
		}
	}
}
