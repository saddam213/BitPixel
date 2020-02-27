using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

using BitPixel.Base.Logging;
using BitPixel.Base.Objects;
using BitPixel.Base.Queueing;
using BitPixel.Common.DataContext;
using BitPixel.Enums;
using BitPixel.QueueService.Common;


namespace BitPixel.QueueService.Implementation
{
	public class QueueEngine
	{
		#region Fields

		private readonly Log Log = LoggingManager.GetLog(typeof(QueueEngine));
		private static readonly QueueEngine _queueEngine = new QueueEngine();

		public static readonly ProcessorQueue<IQueueRequest, IQueueResponse> QueueProcessor = new ProcessorQueue<IQueueRequest, IQueueResponse>(_queueEngine.ProcessQueueItem);

		#endregion

		#region Processor

		private async Task<IQueueResponse> ProcessQueueItem(IQueueRequest queueItem)
		{
			using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
			{
				var stopwatch = GetStopwatch();
				try
				{
					IQueueResponse queueResponse = null;
					using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
					{
						switch (queueItem)
						{
							case SubmitPixelRequest request:
								queueResponse = await ProcessSubmitPixelRequest(connection, request);
								break;
							case SubmitClickRequest request:
								queueResponse = await ProcessSubmitClickRequest(connection, request);
								break;
						}
						transactionScope.Complete();
					}
					return queueResponse;
				}
				catch (QueueException tex)
				{
					Log.Message(LogLevel.Error, $"[QueueException] - Exception: {tex.Message}");
					return new QueueErrorResponse(tex.Message);
				}
				catch (Exception ex)
				{
					Log.Exception($"[ProcessServiceRequest] - An unknown exception occurred during processing", ex);
					return new QueueErrorResponse("Unknown Error");
				}
			}
		}

		#endregion

		private async Task<SubmitPixelResponse> ProcessSubmitPixelRequest(IDbConnection connection, SubmitPixelRequest pixelRequest)
		{
			var stopwatch = GetStopwatch();
			var addPixelResult = await connection.QueryFirstAsync<AddPixelResult>(StoredProcedure.Game_AddPixel, new
			{
				GameId = pixelRequest.GameId,
				UserId = pixelRequest.UserId,
				X = pixelRequest.X,
				Y = pixelRequest.Y,
				Type = pixelRequest.Type,
				Color = pixelRequest.Color,
				Points = pixelRequest.Points,
				MaxPoints = pixelRequest.MaxPoints,
				GameType = pixelRequest.GameType
			}, commandType: CommandType.StoredProcedure);

			if (addPixelResult == null)
				throw new QueueException("Failed to update pixel");

			if (!string.IsNullOrEmpty(addPixelResult.Error))
			{
				Log.Message(LogLevel.Error, $"[AddPixel] - GameId: {pixelRequest.GameId.ToString().PadRight(3)}, UserId: {pixelRequest.UserId.ToString().PadRight(3)}, X: {pixelRequest.X.ToString().PadRight(3)}, Y: {pixelRequest.Y.ToString().PadRight(3)}, Error: {addPixelResult.Error}, {GetElapsedTime(stopwatch)}");
				return new SubmitPixelResponse { Success = false, Message = addPixelResult.Error };
			}

			var addClickResult = await connection.QueryFirstAsync<AddClickResult>(StoredProcedure.Game_AddClick, new
			{
				GameId = pixelRequest.GameId,
				UserId = pixelRequest.UserId,
				Type = ClickType.Pixel,
				X = pixelRequest.X,
				Y = pixelRequest.Y
			}, commandType: CommandType.StoredProcedure);

			if (addClickResult == null || addClickResult.ClickId <= 0)
				throw new QueueException("Failed to add click");

			if (!string.IsNullOrEmpty(addClickResult.Error))
			{
				Log.Message(LogLevel.Error, $"[AddPixel] - GameId: {pixelRequest.GameId.ToString().PadRight(3)}, UserId: {pixelRequest.UserId.ToString().PadRight(3)}, X: {pixelRequest.X.ToString().PadRight(3)}, Y: {pixelRequest.Y.ToString().PadRight(3)}, Error: {addClickResult.Error}, {GetElapsedTime(stopwatch)}");
				return new SubmitPixelResponse { Success = false, Message = addClickResult.Error };
			}

			var response = new SubmitPixelResponse
			{
				Success = true,
				PointsNotification = new PointsNotification
				{
					UserId = pixelRequest.UserId,
					Points = addClickResult.PrizeId.HasValue
					? addClickResult.UserPoints
					: addPixelResult.UserPoints
				},
				PixelNotification = new PixelNotification
				{
					PixelId = addPixelResult.PixelId,
					X = pixelRequest.X,
					Y = pixelRequest.Y,
					Color = pixelRequest.Color,
					Points = addPixelResult.NewPoints,
					Type = pixelRequest.Type,

					UserId = addPixelResult.UserId,
					UserName = addPixelResult.UserName,

					GameId = pixelRequest.GameId,
					GameName = addClickResult.GameName
				}
			};

			if (addClickResult.PrizeId.HasValue)
			{
				response.PrizeNotification = new PrizeNotification
				{
					PrizeId = addClickResult.PrizeId.Value,
					X = pixelRequest.X,
					Y = pixelRequest.Y,

					Name = addClickResult.PrizeName,
					Points = addClickResult.PrizePoints,
					Description = addClickResult.PrizeDescription,

					UserId = addPixelResult.UserId,
					UserName = addPixelResult.UserName,

					GameId = pixelRequest.GameId,
					GameName = addClickResult.GameName
				};
			}

			Log.Message(LogLevel.Info, $"[AddPixel] - GameId: {pixelRequest.GameId.ToString().PadRight(3)}, UserId: {pixelRequest.UserId.ToString().PadRight(3)}, X: {pixelRequest.X.ToString().PadRight(3)}, Y: {pixelRequest.Y.ToString().PadRight(3)}, Color: {pixelRequest.Color}, {GetElapsedTime(stopwatch)}");
			return response;
		}



		private async Task<SubmitClickResponse> ProcessSubmitClickRequest(SqlConnection connection, SubmitClickRequest clickRequest)
		{
			var stopwatch = GetStopwatch();
			var clickResult = await connection.QueryFirstAsync<AddClickResult>(StoredProcedure.Game_AddClick, new
			{
				GameId = clickRequest.GameId,
				UserId = clickRequest.UserId,
				Type = ClickType.Click,
				X = clickRequest.X,
				Y = clickRequest.Y
			}, commandType: CommandType.StoredProcedure);

			if (clickResult == null)
				throw new QueueException("Failed to add click");

			if (!string.IsNullOrEmpty(clickResult.Error))
			{
				Log.Message(LogLevel.Error, $"[AddClick] - GameId: {clickRequest.GameId.ToString().PadRight(3)}, UserId: {clickRequest.UserId.ToString().PadRight(3)}, X: {clickRequest.X.ToString().PadRight(3)}, Y: {clickRequest.Y.ToString().PadRight(3)}, Error: {clickResult.Error}, {GetElapsedTime(stopwatch)}");
				return new SubmitClickResponse { Success = false, Message = clickResult.Error };
			}

			Log.Message(LogLevel.Info, $"[AddClick] - GameId: {clickRequest.GameId.ToString().PadRight(3)}, UserId: {clickRequest.UserId.ToString().PadRight(3)}, X: {clickRequest.X.ToString().PadRight(3)}, Y: {clickRequest.Y.ToString().PadRight(3)}, {GetElapsedTime(stopwatch)}");
			return new SubmitClickResponse
			{
				Success = true
			};
		}

		private static MicroStopwatch GetStopwatch()
		{
			var stopwatch = new MicroStopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		private static string GetElapsedTime(MicroStopwatch stopwatch)
		{
			return $"Elapsed: {stopwatch.ElapsedMilliseconds.ToString("0ms").PadRight(6)}({stopwatch.ElapsedMicroseconds}us)";
		}
	}

	public class AddPixelResult
	{
		public int PixelId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int NewPoints { get; set; }
		public string Error { get; set; }
	}

	public class AddClickResult
	{
		public int ClickId { get; set; }
		public int? PrizeId { get; set; }
		public string PrizeName { get; set; }
		public string PrizeDescription { get; set; }
		public int PrizePoints { get; set; }
		public int UserPoints { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }
		public string Error { get; set; }
	}
}