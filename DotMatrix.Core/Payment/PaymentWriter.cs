using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Payment;
using DotMatrix.Enums;

namespace DotMatrix.Core.Payment
{
	public class PaymentWriter : IPaymentWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> CreateMethod(int userId, int paymentMethodId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users
					.Where(x => x.Id == userId)
					.FirstOrDefaultAsync();
				if (user == null)
					return false;

				var paymentMethod = await context.PaymentMethod
					.Where(x => x.Id == paymentMethodId)
					.FirstOrDefaultAsync();
				if (paymentMethod == null)
					return false;

				var paymentUserMethod = await context.PaymentUserMethod
					.Where(x => x.UserId == user.Id && x.PaymentMethodId == paymentMethod.Id && x.Status != PaymentUserMethodStatus.Deleted)
					.FirstOrDefaultAsync();
				if (paymentUserMethod != null)
					return false;

				var paymentAddress = await context.Database.Connection.QueryFirstOrDefaultAsync<Entity.PaymentAddress>(StoredProcedure.PaymentAssignAddress, new
				{
					UserId = user.Id,
					PaymentMethodId = paymentMethod.Id
				}, commandType: CommandType.StoredProcedure);
				if (paymentAddress == null)
					return false;

				context.PaymentUserMethod.Add(new Entity.PaymentUserMethod
				{
					PaymentMethodId = paymentMethodId,
					Status = PaymentUserMethodStatus.Active,
					Updated = DateTime.UtcNow,
					Timestamp = DateTime.UtcNow,
					UserId = user.Id,
					Data = paymentAddress.Address
				});

				await context.SaveChangesAsync();
				return true;
			}
		}
	}
}
