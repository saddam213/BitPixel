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
				try
				{
					var stopwatch = new Stopwatch();
					stopwatch.Start();
					IQueueResponse queueResponse = null;
					Log.Message(LogLevel.Info, $"ProcessQueueItem request received. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}");
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
					Log.Message(LogLevel.Info, $"ProcessQueueItem request complete. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}, Elapsed: {stopwatch.ElapsedMilliseconds}ms");
					return queueResponse;
				}
				catch (QueueException tex)
				{
					Log.Message(LogLevel.Error, $"A QueueException was thrown. Error: {tex.Message}");
					return new QueueErrorResponse(tex.Message);
				}
				catch (Exception ex)
				{
					Log.Exception("[ProcessServiceRequest] - An unknown exception occurred during processing.", ex);
					return new QueueErrorResponse("Unknown Error");
				}
			}
		}

		#endregion

		private async Task<SubmitPixelResponse> ProcessSubmitPixelRequest(IDbConnection connection, SubmitPixelRequest pixelRequest)
		{
			try
			{
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
					throw new QueueException(addPixelResult.Error);


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
					throw new QueueException(addClickResult.Error);

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
						GameName = addPixelResult.GameName,
						TeamId = addPixelResult.TeamId,
						TeamName = addPixelResult.TeamName
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
						GameName = addPixelResult.GameName,
						TeamId = addPixelResult.TeamId,
						TeamName = addPixelResult.TeamName
					};
				}

				return response;
			}
			catch (SqlException)
			{
				throw new QueueException("Quota exceeded, maximum allowed 10 clicks per second.");
			}
		}


		private async Task<SubmitClickResponse> ProcessSubmitClickRequest(SqlConnection connection, SubmitClickRequest clickRequest)
		{
			try
			{
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
					throw new QueueException(result.Error);

				return new SubmitClickResponse
				{
					Success = true
				};
			}
			catch (SqlException)
			{
				throw new QueueException("Quota exceeded, maximum allowed 10 clicks per second.");
			}
		}
	}

	public class AddPixelResult
	{
		public int PixelId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int TeamId { get; set; }
		public string TeamName { get; set; }
		public int NewPoints { get; set; }
		public string GameName { get; set; }
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