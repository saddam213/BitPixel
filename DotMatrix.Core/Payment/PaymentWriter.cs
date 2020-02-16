using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.DataContext;
using DotMatrix.Common.Payment;
using DotMatrix.Common.Results;
using DotMatrix.Enums;

namespace DotMatrix.Core.Payment
{
	public class PaymentWriter : IPaymentWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateMethod(int userId, int paymentMethodId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users
					.Where(x => x.Id == userId)
					.FirstOrDefaultAsync();
				if (user == null)
					return new WriterResult(false, "User not found");

				var paymentMethod = await context.PaymentMethod
					.Where(x => x.Id == paymentMethodId)
					.FirstOrDefaultAsync();
				if (paymentMethod == null)
					return new WriterResult(false, "Payment method not found");

				var paymentUserMethod = await context.PaymentUserMethod
					.Where(x => x.UserId == user.Id && x.PaymentMethodId == paymentMethod.Id && x.Status != PaymentUserMethodStatus.Deleted)
					.FirstOrDefaultAsync();
				if (paymentUserMethod != null)
					return new WriterResult(false, "User payment method not found");

				var paymentAddress = await context.Database.Connection.QueryFirstOrDefaultAsync<Entity.PaymentAddress>(StoredProcedure.Payment_AssignAddress, new
				{
					UserId = user.Id,
					PaymentMethodId = paymentMethod.Id
				}, commandType: CommandType.StoredProcedure);
				if (paymentAddress == null)
					return new WriterResult(false, "Failed to assign address");

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
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> UpdatePayment(UpdatePaymentModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var payment = await context.PaymentReceipt.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (payment == null)
					return new WriterResult(false, "Payment not found");

				payment.Points = model.Points;
				payment.Status = model.Status;
				payment.Amount = model.Amount;
				payment.Rate = model.Rate;
				payment.Data2 = model.Data2;
				payment.Updated = DateTime.UtcNow;

				await context.SaveChangesAsync();
				await context.Database.Connection.ExecuteAsync(StoredProcedure.User_AuditPoints, new { UserId = payment.UserId }, commandType: CommandType.StoredProcedure);
				return new WriterResult(true);
			}
		}
	}
}
