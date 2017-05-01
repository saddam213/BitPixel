using DotMatrix.Common.DataContext;
using DotMatrix.Common.Deposits;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Deposits
{
	public class DepositReader : IDepositReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<DepositModel>> GetDeposits(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Deposit
					.Where(x => x.UserId == userId)
					.Select(x => new DepositModel
					{
						Id = x.Id,
						Amount = x.Amount,
						Status = x.Status,
						Confirmations = x.Confirmations,
						TxId = x.TxId,
						Timestamp = x.Timestamp
					}).ToListAsync();
			}
		}
	}
}
