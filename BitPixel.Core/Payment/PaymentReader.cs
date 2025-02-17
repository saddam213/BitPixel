﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using BitPixel.Common.DataContext;
using BitPixel.Common.Payment;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;
using BitPixel.Enums;

namespace BitPixel.Core.Payment
{
	public class PaymentReader : IPaymentReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<PaymentMethodModel>> GetMethods()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentMethod
					.Where(x => x.Status != Enums.PaymentMethodStatus.Deleted)
					.Select(x => new PaymentMethodModel
					{
						Id = x.Id,
						Name = x.Name,
						Symbol = x.Symbol,
						Description = x.Description,
						Type = x.Type,
						Status = x.Status,
						Rate = x.Rate,
						Note = x.Note,
						Updated = x.Updated,
						Created = x.Timestamp
					}).ToListAsync();
			}
		}

		public async Task<PaymentMethodModel> GetMethod(string symbol)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentMethod
					.Where(x => x.Symbol == symbol)
					.Select(x => new PaymentMethodModel
					{
						Id = x.Id,
						Name = x.Name,
						Symbol = x.Symbol,
						Description = x.Description,
						Type = x.Type,
						Status = x.Status,
						Rate = x.Rate,
						Note = x.Note,
						Updated = x.Updated,
						Created = x.Timestamp
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<PaymentUserMethodModel> GetUserMethod(int userId, int paymentMethodId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentUserMethod
					.Where(x => x.UserId == userId && x.PaymentMethodId == paymentMethodId && x.Status != PaymentUserMethodStatus.Deleted)
					.Select(x => new PaymentUserMethodModel
					{
						Id = x.Id,
						Name = x.PaymentMethod.Name,
						Description = x.PaymentMethod.Description,
						Note = x.PaymentMethod.Note,
						MethodType = x.PaymentMethod.Type,
						Status = x.Status,
						Data = x.Data,
						Data2 = x.Data2,
						Data3 = x.Data3,
						Data4 = x.Data4,
						Data5 = x.Data5,
						Updated = x.Updated,
						Created = x.Timestamp
					}).FirstOrDefaultAsync();
			}
		}

		public async Task<List<PaymentReceiptModel>> GetReceipts()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentReceipt
					.Select(MapReceipt())
					.OrderByDescending(x => x.Created)
					.ToListAsync();
			}
		}


		public async Task<DataTablesResponseData> GetReceipts(DataTablesParam model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.PaymentReceipt
					.Select(x => new
					{
						Id = x.Id,
						Name = x.User.UserName,
						Symbol = x.PaymentMethod.Symbol,
						Transaction = x.Data2,
						Amount = x.Amount,
						Rate = x.Rate,
						Points = x.Points,
						Status = x.Status,
						Timestamp = x.Timestamp
					}).GetDataTableResponseAsync(model);
			}
		}

		public async Task<List<PaymentReceiptModel>> GetReceipts(int userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentReceipt
					.Where(x => x.UserId == userId)
					.Select(MapReceipt())
					.OrderByDescending(x => x.Created)
					.ToListAsync();
			}
		}

		public async Task<PaymentReceiptModel> GetReceipt(int paymentReceiptId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentReceipt
					.Where(x => x.Id == paymentReceiptId)
					.Select(MapReceipt())
					.FirstOrDefaultAsync();
			}
		}

		public async Task<PaymentReceiptModel> GetReceipt(int userId, int paymentReceiptId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.PaymentReceipt
					.Where(x => x.UserId == userId && x.Id == paymentReceiptId)
					.Select(MapReceipt())
					.FirstOrDefaultAsync();
			}
		}


		private static Expression<Func<Entity.PaymentReceipt, PaymentReceiptModel>> MapReceipt()
		{
			return x => new PaymentReceiptModel
			{
				Id = x.Id,
				UserName = x.User.UserName,
				Points = x.Points,
				Rate = x.Rate,
				Amount = x.Amount,
				Status = x.Status,
				Name = x.PaymentMethod.Name,
				Description = x.Description,
				Data = x.Data,
				Data2 = x.Data2,
				Data3 = x.Data3,
				Data4 = x.Data4,
				Data5 = x.Data5,
				Updated = x.Updated,
				Created = x.Timestamp
			};
		}

	}
}
