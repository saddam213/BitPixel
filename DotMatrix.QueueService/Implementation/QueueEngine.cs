using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

using DotMatrix.Base.Logging;
using DotMatrix.Base.Queueing;
using DotMatrix.Common.DataContext;
using DotMatrix.Enums;
using DotMatrix.QueueService.Common;


namespace DotMatrix.QueueService.Implementation
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

					if (!queueResponse.Success)
						Log.Message(LogLevel.Error, $"[QueueError] - Error: {queueResponse.Message}, {GetElapsedTime(stopwatch)}");

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
				MaxPoints = pixelRequest.MaxPoints
			}, commandType: CommandType.StoredProcedure);

			if (addPixelResult == null)
				throw new QueueException("Failed to update pixel");

			if (!string.IsNullOrEmpty(addPixelResult.Error))
				return new SubmitPixelResponse { Success = false, Message = addPixelResult.Error };

			var addClickResult = await connection.QueryFirstAsync<AddClickResult>(StoredProcedure.Game_AddClick, new
			{
				GameId = pixelRequest.GameId,
				UserId = pixelRequest.UserId,
				Type = ClickType.Pixel,
				X = pixelRequest.X,
				Y = pixelRequest.Y
			}, commandType: CommandType.StoredProcedure);

			if (addClickResult == null)
				throw new QueueException("Failed to add click");

			if (!string.IsNullOrEmpty(addClickResult.Error))
				return new SubmitPixelResponse { Success = false, Message = addClickResult.Error };

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

			Log.Message(LogLevel.Info, $"[AddPixel] - GameId: {pixelRequest.GameId}, UserId: {pixelRequest.UserId}, X: {pixelRequest.X}, Y: {pixelRequest.Y}, Color: {pixelRequest.Color}, {GetElapsedTime(stopwatch)}");
			return response;
		}



		private async Task<SubmitClickResponse> ProcessSubmitClickRequest(SqlConnection connection, SubmitClickRequest clickRequest)
		{
			var stopwatch = GetStopwatch();
			var result = await connection.QueryFirstAsync<AddClickResult>(StoredProcedure.Game_AddClick, new
			{
				GameId = clickRequest.GameId,
				UserId = clickRequest.UserId,
				Type = ClickType.Click,
				X = clickRequest.X,
				Y = clickRequest.Y
			}, commandType: CommandType.StoredProcedure);

			if (result == null)
				throw new QueueException("Failed to add click");

			if (!string.IsNullOrEmpty(result.Error))
				return new SubmitClickResponse { Success = false, Message = result.Error };

			Log.Message(LogLevel.Info, $"[AddClick] - GameId: {clickRequest.GameId}, UserId: {clickRequest.UserId}, X: {clickRequest.X}, Y: {clickRequest.Y}, {GetElapsedTime(stopwatch)}");
			return new SubmitClickResponse
			{
				Success = true
			};
		}

		private static Stopwatch GetStopwatch()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		private static string GetElapsedTime(Stopwatch stopwatch)
		{
			return $"Elapsed: {stopwatch.ElapsedMilliseconds.ToString("0ms").PadRight(6)}({stopwatch.ElapsedTicks / 10}us)";
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