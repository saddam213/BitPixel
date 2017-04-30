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
			using (var apiClient = new ApiPrivate("ee652448256d497e8256bdd9068297b2", "WTAzqs48uTBtBI3MFr9G7/NSVJxFRxe68JcpAckixDQ=", "http://www.dotmatrix.co.nz"))
			{
				for (int i = 0; i < 600; i++)
				{
					await apiClient.AddPixel(new AddPixelRequest { X = i, Y = i + 100, R = 255, G = 0, B = 0 }); //R
					await apiClient.AddPixel(new AddPixelRequest { X = i + 1, Y = i + 100, R = 255, G = 0, B = 0 }); //R
					await apiClient.AddPixel(new AddPixelRequest { X = i + 2, Y = i + 100, R = 255, G = 0, B = 0 }); //R
					await apiClient.AddPixel(new AddPixelRequest { X = i + 3, Y = i + 100, R = 255, G = 0, B = 0 }); //R

					await apiClient.AddPixel(new AddPixelRequest { X = i + 4, Y = i + 100, R = 255, G = 165, B = 0 }); //O
					await apiClient.AddPixel(new AddPixelRequest { X = i + 5, Y = i + 100, R = 255, G = 165, B = 0 }); //O
					await apiClient.AddPixel(new AddPixelRequest { X = i + 6, Y = i + 100, R = 255, G = 165, B = 0 }); //O
					await apiClient.AddPixel(new AddPixelRequest { X = i + 7, Y = i + 100, R = 255, G = 165, B = 0 }); //O

					await apiClient.AddPixel(new AddPixelRequest { X = i + 8, Y = i + 100, R = 255, G = 255, B = 0 }); //Y
					await apiClient.AddPixel(new AddPixelRequest { X = i + 9, Y = i + 100, R = 255, G = 255, B = 0 }); //Y
					await apiClient.AddPixel(new AddPixelRequest { X = i + 10, Y = i + 100, R = 255, G = 255, B = 0 }); //Y
					await apiClient.AddPixel(new AddPixelRequest { X = i + 11, Y = i + 100, R = 255, G = 255, B = 0 }); //Y

					await apiClient.AddPixel(new AddPixelRequest { X = i + 12, Y = i + 100, R = 0, G = 255, B = 0 }); //G
					await apiClient.AddPixel(new AddPixelRequest { X = i + 13, Y = i + 100, R = 0, G = 255, B = 0 }); //G
					await apiClient.AddPixel(new AddPixelRequest { X = i + 14, Y = i + 100, R = 0, G = 255, B = 0 }); //G
					await apiClient.AddPixel(new AddPixelRequest { X = i + 15, Y = i + 100, R = 0, G = 255, B = 0 }); //G

					await apiClient.AddPixel(new AddPixelRequest { X = i + 16, Y = i + 100, R = 0, G = 0, B = 255 }); //B
					await apiClient.AddPixel(new AddPixelRequest { X = i + 17, Y = i + 100, R = 0, G = 0, B = 255 }); //B
					await apiClient.AddPixel(new AddPixelRequest { X = i + 18, Y = i + 100, R = 0, G = 0, B = 255 }); //B
					await apiClient.AddPixel(new AddPixelRequest { X = i + 19, Y = i + 100, R = 0, G = 0, B = 255 }); //B


					await apiClient.AddPixel(new AddPixelRequest { X = i + 20, Y = i + 100, R = 9, G = 0, B = 9 }); //I
					await apiClient.AddPixel(new AddPixelRequest { X = i + 21, Y = i + 100, R = 9, G = 0, B = 9 }); //I
					await apiClient.AddPixel(new AddPixelRequest { X = i + 22, Y = i + 100, R = 9, G = 0, B = 9 }); //I
					await apiClient.AddPixel(new AddPixelRequest { X = i + 23, Y = i + 100, R = 9, G = 0, B = 9 }); //I

					await apiClient.AddPixel(new AddPixelRequest { X = i + 24, Y = i + 100, R = 128, G = 0, B = 128 }); //V
					await apiClient.AddPixel(new AddPixelRequest { X = i + 25, Y = i + 100, R = 128, G = 0, B = 128 }); //V
					await apiClient.AddPixel(new AddPixelRequest { X = i + 26, Y = i + 100, R = 128, G = 0, B = 128 }); //V
					await apiClient.AddPixel(new AddPixelRequest { X = i + 27, Y = i + 100, R = 128, G = 0, B = 128 }); //V
																																																							//Console.WriteLine(result.Message);
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
