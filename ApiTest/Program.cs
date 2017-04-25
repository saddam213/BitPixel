using System;
using System.Collections.Generic;
using System.Drawing;
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
			using (var apiClient = new ApiPrivate("4b9adfb45af94af78d94ce75d5881c7b", "Pw8B2Jj5P7p/7DO5e/CYwKAvelQQH2knXztQ5djaiF0=", "http://localhost:88"))
			{
				var data = new List<AddPixelRequest>();
				//for (int i = 0; i < 100; i++)
				//{
				//	data.Add(new AddPixelRequest
				//	{
				//		Color = "#000000",
				//		X = _rand.Next(500, 520),
				//		Y = _rand.Next(200, 220)
				//	});

				//	//await Task.Delay(200);
				//}

				using (var bitmap = new Bitmap(@"E:\Shared\bigdick.bmp"))
				{
					var offsetX = 20;
					var offsetY = 20;
					var width = bitmap.Width;
					var height = bitmap.Height;
					for (int x = 0; x < width; x++)
					{
						for (int y = 0; y < height; y++)
						{
							var pixel = bitmap.GetPixel(x, y);
							var hex = HexConverter2(pixel);
		
								var result = await apiClient.AddPixel(new AddPixelRequest
								{
									Color = hex,
									X = offsetX + x,
									Y = offsetY + y
								});
								Console.WriteLine(result.Message);
			

						}
					}
				}

				//	var result = await apiClient.AddPixel(data.First());
			
			}
		}

		private static string HexConverter(System.Drawing.Color c)
		{
			return "#"  + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
		}

		private static string HexConverter2(System.Drawing.Color c)
		{
			return $"rgba({c.R},{c.G},{c.B},{c.A})";
		}
	}
}
