using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DotMatrix.Base.Logging;
using DotMatrix.Data.DataContext;
using DotMatrix.Entity;
using DotMatrix.Enums;
using DotMatrix.WalletService.Connector.Base;
using DotMatrix.WalletService.Connector.DataObjects;
using Dapper;
using DotMatrix.Common.DataContext;

namespace DotMatrix.WalletService.Implementation
{
	public class DepositEngine
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(DepositEngine));


		private bool _isRunning = false;
		private CancellationToken _cancelToken;


		public DataContextFactory DataContextFactory { get; set; }

		public DepositEngine(CancellationToken cancelToken)
		{
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
		}

		public void Start()
		{
			if (_isRunning)
				return;

			_isRunning = true;
			Task.Factory.StartNew(ProcessLoop, _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}


		public void Stop()
		{
			_isRunning = false;
		}

		private async Task ProcessLoop()
		{
			while (_isRunning)
			{
				try
				{
					await ProcessAddresses();
				}
				catch (Exception ex)
				{
					Log.Exception("ProcessAddresses", ex);
				}

				try
				{
					await ProcessDeposits();
				}
				catch (Exception ex)
				{
					Log.Exception("ProcessDeposits", ex);
				}


				try
				{
					await ProcessWithdrawals();
				}
				catch (Exception ex)
				{
					Log.Exception("ProcessWithdrawals", ex);
				}

				await Task.Delay(TimeSpan.FromMinutes(1));
			}
		}

		private async Task ProcessDeposits()
		{
			Log.Message(LogLevel.Info, "Processing Deposits..");
			using (var context = DataContextFactory.CreateContext())
			{
				var paymentMethods = await context.PaymentMethod
					.Where(x => x.Status == PaymentMethodStatus.Ok && x.Type == PaymentMethodType.Crypto)
					.ToListAsync();
				foreach (var paymentMethod in paymentMethods)
				{
					Log.Message(LogLevel.Info, $"Processing {paymentMethod.Name}");
					var walletDeposits = await GetWalletDeposits(paymentMethod);
					if (!walletDeposits.Any())
					{
						Log.Message(LogLevel.Info, "No new deposits found.");
						Log.Message(LogLevel.Info, $"Processing {paymentMethod.Name} complete.");
						continue;
					}

					var minConfirmations = int.Parse(paymentMethod.Data4);
					Log.Message(LogLevel.Info, $"{walletDeposits.Count()} new deposits found.");
					var unconfirmedDeposits = await context.PaymentReceipt
						.Include(x => x.User)
						.Where(x => x.PaymentMethodId == paymentMethod.Id && x.Status == PaymentReceiptStatus.Pending)
						.ToListAsync();

					foreach (var walletDeposit in walletDeposits.OrderBy(x => x.Time))
					{
						var exists = unconfirmedDeposits.FirstOrDefault(x => x.Data == walletDeposit.Address && x.Data2 == walletDeposit.Txid);
						if (exists != null)
						{
							int confirmations = int.Parse(exists.Data3);
							if (confirmations == walletDeposit.Confirmations)
								continue;

							Log.Message(LogLevel.Info, $"Updating deposit #{exists.Id} confirmations.");
							exists.Data3 = Math.Min(walletDeposit.Confirmations, minConfirmations).ToString();
							exists.Status = confirmations >= minConfirmations
								? PaymentReceiptStatus.Complete
								: PaymentReceiptStatus.Pending;
							await context.SaveChangesAsync();
							if (exists.Status == PaymentReceiptStatus.Complete)
							{
								Log.Message(LogLevel.Info, $"Deposit #{exists.Id} confirmed, auditing user points...");
								await context.Database.Connection.ExecuteAsync(StoredProcedure.User_AuditPoints, new { UserId = exists.UserId }, commandType: System.Data.CommandType.StoredProcedure);
								Log.Message(LogLevel.Info, $"Deposit #{exists.Id} audit complete.");
								paymentMethod.Data5 = walletDeposit.Blockhash;
							}
						}
						else
						{
							var paymentUserMethod = await context.PaymentUserMethod.FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethod.Id && x.Data == walletDeposit.Address);
							if (paymentUserMethod == null)
								continue;

							if (await context.PaymentReceipt.AnyAsync(x => x.PaymentUserMethodId == paymentUserMethod.Id && x.Data == walletDeposit.Address && x.Data2 == walletDeposit.Txid))
								continue;

							var points = (int)(walletDeposit.Amount / paymentMethod.Rate);
							var newDeposit = new Entity.PaymentReceipt
							{
								UserId = paymentUserMethod.UserId,
								Data = paymentUserMethod.Data,
								Data2 = walletDeposit.Txid,
								Data3 = Math.Min(walletDeposit.Confirmations, minConfirmations).ToString(),
								Data4 = walletDeposit.Blockhash,
								Amount = walletDeposit.Amount,
								PaymentMethodId = paymentMethod.Id,
								PaymentUserMethodId = paymentUserMethod.Id,
								Rate = paymentMethod.Rate,
								Status = PaymentReceiptStatus.Pending,
								Updated = DateTime.UtcNow,
								Timestamp = DateTime.UtcNow,
								Points = points
							};
							context.PaymentReceipt.Add(newDeposit);
							await context.SaveChangesAsync();
							Log.Message(LogLevel.Info, $"Added new deposit, TxId: {walletDeposit.Txid}.");
						}
					}

					Log.Message(LogLevel.Info, $"Processing {paymentMethod.Name} complete");
				}


			}
			Log.Message(LogLevel.Info, "Processing Deposits complete.");
		}

		private static int _addressCreationCount = 25;
		private async Task ProcessAddresses()
		{
			Log.Message(LogLevel.Info, "Processing Addresses..");
			using (var context = DataContextFactory.CreateContext())
			{
				var paymentMethods = await context.PaymentMethod
					.Where(x => x.Status == PaymentMethodStatus.Ok && x.Type == PaymentMethodType.Crypto)
					.ToListAsync();
				foreach (var paymentMethod in paymentMethods)
				{
					Log.Message(LogLevel.Info, $"Processing {paymentMethod.Name}");
					var addressCount = await context.PaymentAddress
						.Where(x => x.PaymentMethodId == paymentMethod.Id && x.UserId == null)
						.CountAsync();
					if (addressCount < _addressCreationCount)
					{
						for (int i = 0; i < _addressCreationCount; i++)
						{
							var address = await CreateAddress(paymentMethod);
							if (string.IsNullOrEmpty(address))
								throw new Exception("Null address returnd from wallet connector");

							context.PaymentAddress.Add(new PaymentAddress
							{
								Address = address,
								PaymentMethodId = paymentMethod.Id,
								Updated = DateTime.UtcNow,
								Timestamp = DateTime.UtcNow
							});
						}
					}
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, $"Processing {paymentMethod.Name} complete");
				}
			}
			Log.Message(LogLevel.Info, "Processing Addresses complete.");
		}

		private async Task ProcessWithdrawals()
		{
			Log.Message(LogLevel.Info, "Processing Withdrawals..");
			using (var context = DataContextFactory.CreateContext())
			{
				var internalAddresses = await context.PaymentUserMethod
					.Include(x => x.PaymentMethod)
					.ToListAsync();
				var pendingPrizes = await context.Database.Connection.QueryAsync<PrizePayment>(StoredProcedure.WalletService_GetWithdrawals, commandType: System.Data.CommandType.StoredProcedure);
				Log.Message(LogLevel.Info, $"{pendingPrizes.Count()} withdrawals found to process.");
				foreach (var prize in pendingPrizes)
				{
					Log.Message(LogLevel.Info, $"Processing prize Id: {prize.PrizeId}, Symbol: {prize.Symbol}, Amount: {prize.Amount}, Destination: {prize.Destination}");

					var userPaymentMethod = internalAddresses.FirstOrDefault(x => x.Data.Equals(prize.Destination, StringComparison.OrdinalIgnoreCase));
					if (userPaymentMethod != null)
					{
						Log.Message(LogLevel.Debug, "INTERNAL CLAIM");
						var transactionId = $"Internal_Prize_Claim: #{prize.PrizeId}";
						var cryptoAmount = decimal.Parse(prize.Amount);
						var minConfirmations = int.Parse(userPaymentMethod.PaymentMethod.Data4);
						var points = (int)(cryptoAmount / userPaymentMethod.PaymentMethod.Rate);
						var newDeposit = new Entity.PaymentReceipt
						{
							UserId = userPaymentMethod.UserId,
							Data = userPaymentMethod.Data,
							Data2 = transactionId,
							Data3 = userPaymentMethod.PaymentMethod.Data4, // minconfirms
							Data4 = default(string),
							Amount = cryptoAmount,
							PaymentMethodId = userPaymentMethod.PaymentMethodId,
							PaymentUserMethodId = userPaymentMethod.Id,
							Rate = userPaymentMethod.PaymentMethod.Rate,
							Status = PaymentReceiptStatus.Complete,
							Updated = DateTime.UtcNow,
							Timestamp = DateTime.UtcNow,
							Points = points
						};
						context.PaymentReceipt.Add(newDeposit);
						await context.SaveChangesAsync();
						await context.Database.Connection.QueryAsync(StoredProcedure.WalletService_UpdateWithdraw, new
						{
							PrizeId = prize.PrizeId,
							TransactionId = transactionId
						}, commandType: System.Data.CommandType.StoredProcedure);
						await context.Database.Connection.ExecuteAsync(StoredProcedure.User_AuditPoints, new { UserId = userPaymentMethod.UserId }, commandType: System.Data.CommandType.StoredProcedure);
					}
					else
					{
						Log.Message(LogLevel.Debug, "EXTERNAL CLAIM");
						var transactionId = await SendTransaction(prize);
						if (string.IsNullOrEmpty(transactionId))
						{
							continue;
						}

						Log.Message(LogLevel.Info, $"Transaction sent, TrasnactionId: {transactionId}");
						await context.Database.Connection.QueryAsync(StoredProcedure.WalletService_UpdateWithdraw, new
						{
							PrizeId = prize.PrizeId,
							TransactionId = transactionId
						}, commandType: System.Data.CommandType.StoredProcedure);
					}

					Log.Message(LogLevel.Info, $"Processing prize complete.");
				}
			}
			Log.Message(LogLevel.Info, "Processing Withdrawals complete.");
		}

		private async Task<string> SendTransaction(PrizePayment prizePayment)
		{
			try
			{
				var connector = new WalletConnector
				(
					prizePayment.Host,
					prizePayment.UserName,
					prizePayment.Password
				);
				var result = await connector.SendToAddressAsync(prizePayment.Destination, decimal.Parse(prizePayment.Amount));
				return result.Txid;
			}
			catch (Exception ex)
			{
				Log.Exception("Failed to send transaction.", ex);
				return null;
			}
		}

		private async Task<IEnumerable<TransactionData>> GetWalletDeposits(PaymentMethod paymentMethod)
		{
			try
			{
				var connector = new WalletConnector
			(
				paymentMethod.Data,
				paymentMethod.Data2,
				paymentMethod.Data3
			);
				return await connector.GetDepositsAsync(paymentMethod.Data5);
			}
			catch (Exception ex)
			{
				Log.Exception("Failed to get transactions.", ex);
				return Enumerable.Empty<TransactionData>();
			}
		}

		private async Task<string> CreateAddress(PaymentMethod paymentMethod)
		{
			try
			{
				var connector = new WalletConnector
				(
					paymentMethod.Data,
					paymentMethod.Data2,
					paymentMethod.Data3
				);
				return await connector.GenerateAddressAsync("", true);
			}
			catch (Exception ex)
			{
				Log.Exception("Failed to create address.", ex);
				return null;
			}
		}
	}

	public class PrizePayment
	{
		public int PrizeId { get; set; }
		public string Symbol { get; set; }
		public string Amount { get; set; }
		public string Destination { get; set; }

		public string Host { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}
