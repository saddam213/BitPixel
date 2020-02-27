using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using BitPixel.Common.DataContext;
using BitPixel.Common.Replay;

namespace BitPixel.Core.Replay
{
	public class ReplayReader : IReplayReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<ReplayPixelModel>> GetPixels(ReplayFilterModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.PixelHistory
					.Where(x => x.GameId == model.GameId && x.Type == Enums.PixelType.User && (string.IsNullOrEmpty(model.Player) || x.User.UserName == model.Player))
					.Select(x => new ReplayPixelModel
					{
						Id = x.Id,
						X = x.Pixel.X,
						Y = x.Pixel.Y,
						Color = x.Color
					})
					.OrderBy(x => x.Id)
					.ToListAsync();
			}
		}
	}
}
