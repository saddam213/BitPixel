using DotMatrix.Common.DataContext;
using DotMatrix.Common.Pixel;
using DotMatrix.Common.Queue;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Core.Pixel
{
	public class PixelWriter : IPixelWriter
	{
		public IQueueService QueueService { get; set; }

		public async Task<PixelResultModel> AddOrUpdate(string userId, PixelModel model)
		{
			try
			{
				var result = await QueueService.SubmitPixel(userId, model, false);
				return new PixelResultModel
				{
					Success = result.Success,
					Message = result.Message,
					Balance = result.Balance
				};
			}
			catch (Exception)
			{
				return new PixelResultModel
				{
					Success = false,
					Message = "Error: Failed to add new pixel"
				};
			}
		}
	}
}
