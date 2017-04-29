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

		public async Task<bool> AddOrUpdate(string userId, PixelModel model)
		{
			return await QueueService.SubmitPixel(userId, model, false);
		}

		public async Task<bool> AddOrUpdate(string userId, List<PixelModel> models)
		{
			return await QueueService.SubmitPixels(userId, models, false);
		}
	}
}
