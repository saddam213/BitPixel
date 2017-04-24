using DotMatrix.Common.Pixel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotMatrix.Controllers
{
	public class ApiPublicController : ApiController
	{
		public IPixelReader PixelReader { get; set; }

		[HttpGet]
		public async Task<List<PixelModel>> GetPixels()
		{
			return await PixelReader.GetPixels();
		}
	}
}
