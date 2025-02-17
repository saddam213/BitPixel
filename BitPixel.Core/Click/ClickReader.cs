﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Common.Click;
using BitPixel.Common.DataContext;
using BitPixel.Datatables;
using BitPixel.Datatables.Models;

namespace BitPixel.Core.Click
{
	public class ClickReader : IClickReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponseData> GetUserHistory(DataTablesParam model, int userId, int? maxCount)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Click
					.Where(x => x.UserId == userId)
					.Select(x => new
					{
						Id = x.Id,
						GameId = x.GameId,
						Game = x.Game.Name,
						X = x.X,
						Y = x.Y,
						Color = x.Type,
						Timestamp = x.Timestamp
					});
				if (maxCount.HasValue)
				{
					query = query
						.OrderByDescending(x => x.Timestamp)
						.Take(maxCount.Value);
				}
				return await query.GetDataTableResponseAsync(model);
			}
		}
	}
}
