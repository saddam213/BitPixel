using Cryptopia.Base.Logging;
using DotMatrix.Data.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Configuration;
using Cryptopia.WalletAPI.Base;
using Cryptopia.Base;
using DotMatrix.Enums;

namespace DotMatrix.DepositService.Implementation
{
	public class DepositEngine
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(DepositEngine));


		private bool _isRunning = false;
		private CancellationToken _cancelToken;
		private WalletConnector _walletConnector;
		private int _minConfirmations = 6;

		public DataContextFactory DataContextFactory { get; set; }

		public DepositEngine(CancellationToken cancelToken)
		{
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
			_walletConnector = new WalletConnector
			(
				ConfigurationManager.AppSettings["WalletHost"],
				int.Parse(ConfigurationManager.AppSettings["WalletPort"]),
				ConfigurationManager.AppSettings["WalletUser"],
				ConfigurationManager.AppSettings["WalletPass"]
			);
		}

		public void Start()
		{
			if (_isRunning)
				return;

			_isRunning = true;
			Task.Factory.StartNew(async () => await ProcessLoop().ConfigureAwait(false), _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
		}


		public void Stop()
		{
			_isRunning = false;
		}

		private async Task ProcessLoop()
		{
			while (_isRunning)
			{
				await ProcessDeposits();
				await Task.Delay(TimeSpan.FromMinutes(5));
			}
		}

		private async Task ProcessDeposits()
		{
			Log.Message(LogLevel.Info, "Processing Deposits..");
			using (var context = DataContextFactory.CreateContext())
			{
				var lastConfirmedDeposit = await context.Deposit
					.Where(x => x.Status == DepositStatus.Confirmed)
					.OrderByDescending(x => x.Timestamp)
					.FirstOrDefaultAsync();

				var walletDeposits = await _walletConnector.GetDepositsAsync(lastConfirmedDeposit?.BlockHash);
				if (!walletDeposits.Any())
				{
					Log.Message(LogLevel.Info, "No new deposits found.");
					return;
				}

				Log.Message(LogLevel.Info, $"{walletDeposits.Count()} new deposits found.");
				var unconfirmedDeposits = await context.Deposit
					.Include(x => x.User)
					.Where(x => x.Status == DepositStatus.Unconfirmed)
					.ToListAsync();
				foreach (var walletDeposit in walletDeposits.OrderBy(x => x.Time))
				{
					var exists = unconfirmedDeposits.FirstOrDefault(x => x.TxId == walletDeposit.Txid && x.UserId == walletDeposit.Account && x.Status == DepositStatus.Unconfirmed);
					if (exists != null)
					{
						if (exists.Confirmations == walletDeposit.Confirmations)
							continue;

						Log.Message(LogLevel.Info, $"Updating deposit #{exists.Id} confirmations.");
						exists.Confirmations = walletDeposit.Confirmations;
						exists.Status = exists.Confirmations >= _minConfirmations
							? DepositStatus.Confirmed
							: DepositStatus.Unconfirmed;
						if (exists.Status == DepositStatus.Confirmed)
						{
							var newBalance = exists.User.Balance + exists.Amount;
							Log.Message(LogLevel.Info, $"Deposit #{exists.Id} confirmed, updating user balance from {exists.User.Balance} to {newBalance}.");
							exists.User.Balance = newBalance;
						}
					}
					else
					{
						if (!await context.Users.AnyAsync(x => x.Id == walletDeposit.Account))
							continue;

						if (await context.Deposit.AnyAsync(x => x.TxId == walletDeposit.Txid && x.UserId == walletDeposit.Account))
							continue;

						var newDeposit = new Entity.Deposit
						{
							Amount = walletDeposit.Amount,
							BlockHash = walletDeposit.Blockhash,
							Confirmations = walletDeposit.Confirmations,
							Status = DepositStatus.Unconfirmed,
							Timestamp = walletDeposit.Time.ToDateTime(),
							TxId = walletDeposit.Txid,
							UserId = walletDeposit.Account
						};
						context.Deposit.Add(newDeposit);
						Log.Message(LogLevel.Info, $"Added new deposit, TxId: {walletDeposit.Txid}.");
					}
				}

				Log.Message(LogLevel.Info, $"Saving database changes...");
				await context.SaveChangesAsync();
				Log.Message(LogLevel.Info, $"Changes saved.");
			}
			Log.Message(LogLevel.Info, "Processing Deposits complete.");
		}

	}
}
