using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Base.Logging;
using DotMatrix.Base.Queueing;
using DotMatrix.Common.DataContext;
using DotMatrix.QueueService.Common;


namespace DotMatrix.QueueService.Implementation
{
	public class QueueEngine
	{
		#region Fields

		private readonly Log Log = LoggingManager.GetLog(typeof(QueueEngine));
		private static readonly QueueEngine _queueEngine = new QueueEngine();
		public static readonly ProcessorQueue<SubmitPixelRequest, SubmitPixelResponse> PixelQueueProcessor = new ProcessorQueue<SubmitPixelRequest, SubmitPixelResponse>(_queueEngine.ProcessPixelQueueItem);
		public static readonly ProcessorQueue<SubmitClickRequest, SubmitClickResponse> ClickQueueProcessor = new ProcessorQueue<SubmitClickRequest, SubmitClickResponse>(_queueEngine.ProcessClickQueueItem);

		#endregion

		#region Processor

		public async Task<SubmitPixelResponse> ProcessPixelQueueItem(SubmitPixelRequest queueItem)
		{
			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
				{
					Log.Message(LogLevel.Info, $"ProcessQueueItem request received. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}");
					var queueResponse = await ProcessSubmitPixelRequest(connection, queueItem);
					Log.Message(LogLevel.Info, $"ProcessQueueItem request complete. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}, Elapsed: {stopwatch.ElapsedMilliseconds}ms");
					if (queueResponse != null)
					{
						return queueResponse;
					}
				}
			}
			catch (QueueException tex)
			{
				Log.Message(LogLevel.Error, $"A QueueException was thrown. Error: {tex.Message}");
				return new SubmitPixelResponse { Success = false, Message = tex.Message };
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred during request processing.", ex);
			}

			return new SubmitPixelResponse { Success = false, Message = "An unknown error occurred. If problems persists please contact support" };
		}

		public async Task<SubmitClickResponse> ProcessClickQueueItem(SubmitClickRequest queueItem)
		{
			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				using (var connection = new SqlConnection(ConnectionString.DefaultConnection))
				{
					Log.Message(LogLevel.Info, $"ProcessQueueItem request received. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}");
					var queueResponse = await ProcessSubmitClickRequest(connection, queueItem);
					Log.Message(LogLevel.Info, $"ProcessQueueItem request complete. Type: {queueItem.GetType().Name}, IsApi: {queueItem.IsApi}, Elapsed: {stopwatch.ElapsedMilliseconds}ms");
					if (queueResponse != null)
					{
						return queueResponse;
					}
				}
			}
			catch (QueueException tex)
			{
				Log.Message(LogLevel.Error, $"A QueueException was thrown. Error: {tex.Message}");
				return new SubmitClickResponse { Success = false, Message = tex.Message };
			}
			catch (Exception ex)
			{
				Log.Exception("An unknown exception occurred during request processing.", ex);
			}

			return new SubmitClickResponse { Success = false, Message = "An unknown error occurred. If problems persists please contact support" };
		}




		#endregion

		private async Task<SubmitPixelResponse> ProcessSubmitPixelRequest(IDbConnection connection, SubmitPixelRequest pixelRequest)
		{
			try
			{
				var result = await connection.QueryFirstAsync<UpdatePixelResult>(StoredProcedure.AddPixel, new
				{
					UserId = pixelRequest.UserId,
					X = pixelRequest.X,
					Y = pixelRequest.Y,
					Type = pixelRequest.Type,
					Color = pixelRequest.Color,
					Points = pixelRequest.Points,
					MaxPoints = pixelRequest.MaxPoints
				}, commandType: CommandType.StoredProcedure);

				if (result == null)
					throw new QueueException("Failed to update pixel");

				if (!string.IsNullOrEmpty(result.Error))
					throw new QueueException(result.Error);

				return new SubmitPixelResponse
				{
					Success = true,

					PixelId = result.PixelId,
					NewPoints = result.NewPoints,

					TeamId = result.TeamId,
					TeamName = result.TeamName,
					UserId = result.UserId,
					UserName = result.UserName,
					UserPoints = result.UserPoints
				};
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
				var result = await connection.QueryFirstAsync<AddClickResult>(StoredProcedure.AddClick, new
				{
					UserId = clickRequest.UserId,
					Type = clickRequest.Type,
					X = clickRequest.X,
					Y = clickRequest.Y
				}, commandType: CommandType.StoredProcedure);

				if (result == null)
					throw new QueueException("Failed to add click");

				if (!string.IsNullOrEmpty(result.Error))
					throw new QueueException(result.Error);

				return new SubmitClickResponse
				{
					Success = true,
					IsPrizeWinner = result.PrizeId.HasValue,
					PrizeId = result.PrizeId ?? 0,
					PrizeName = result.PrizeName,
					PrizePoints = result.PrizePoints,
					PrizeDescription = result.PrizeDescription,
					UserPoints = result.UserPoints
				};
			}
			catch (SqlException)
			{
				throw new QueueException("Quota exceeded, maximum allowed 10 clicks per second.");
			}
		}
	}

	public class UpdatePixelResult
	{
		public int PixelId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int UserPoints { get; set; }
		public int TeamId { get; set; }
		public string TeamName { get; set; }
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

		public string Error { get; set; }
	}
}