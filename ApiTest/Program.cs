using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().Wait();
			Console.ReadKey();
		}

		private static Random _rand = new Random();

		static async Task MainAsync()
		{
			await DrawPicture(@"C:\Users\Deven\Downloads\Cryptopia\Cryptopia\LogoArtboard 4 copy 12100.png", 300, 300);
		}


		private static async Task DrawPicture(string filename, int offsetX, int offsetY)
		{
			//using (var apiClient = new ApiPrivate("4b9adfb45af94af78d94ce75d5881c7b", "Pw8B2Jj5P7p/7DO5e/CYwKAvelQQH2knXztQ5djaiF0=", "http://localhost:88"))
		using (var apiClient = new ApiPrivate("ee652448256d497e8256bdd9068297b2", "WTAzqs48uTBtBI3MFr9G7/NSVJxFRxe68JcpAckixDQ=", "http://www.dotmatrix.co.nz"))
			using (var bitmap = new Bitmap(filename))
			{
				var width = bitmap.Width;
				var height = bitmap.Height;
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						var pixel = bitmap.GetPixel(x, y);
						var result = await apiClient.AddPixel(new AddPixelRequest
						{
							X = offsetX + x,
							Y = offsetY + y,
							R = pixel.R,
							G = pixel.G,
							B = pixel.B
						});
						Console.WriteLine(result.Message);
					}
				}
			}
		}
	}
}
