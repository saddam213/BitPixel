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

		//public class PixelData
		//{
		//	public int X { get; set; }
		//	public int Y { get; set; }
		//}

		static async Task MainAsync()
		{
			using (var apiClient = new ApiPrivate("ee652448256d497e8256bdd9068297b2", "WTAzqs48uTBtBI3MFr9G7/NSVJxFRxe68JcpAckixDQ=", "http://www.dotmatrix.co.nz"))
			{
				var pixelMap = new List<AddPixelRequest>();
				pixelMap.Add(new AddPixelRequest { X = 10, Y = 800, R = 255 });
				pixelMap.Add(new AddPixelRequest { X = 10, Y = 801, G = 255 });
				pixelMap.Add(new AddPixelRequest { X = 10, Y = 802, B = 255 });


				for (int i = 0; i < 100000; i++)
				{
					foreach (var pixel in pixelMap)
					{
						var existingPixel = await apiClient.GetPixel(new GetPixelRequest { X = pixel.X, Y = pixel.Y });
						if (existingPixel.Success)
						{
							if (existingPixel.Data.R == pixel.R && existingPixel.Data.G == pixel.G && existingPixel.Data.B == pixel.B)
								continue;

							await apiClient.AddPixel(pixel);
						}
					}
					await Task.Delay(1000);
				}
			}

			//await DrawPicture(@"C:\Users\Deven\Pictures\etjgJ2D.jpg", 500, 100);
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
						if (pixel.R == 0 && pixel.B == 0 && pixel.G == 0)
							continue;

						if (pixel.R == 255 && pixel.B == 255 && pixel.G == 255)
							continue;

						try
						{
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
						catch (Exception e)
						{

						}
					}
				}
			}
		}
	}
}
