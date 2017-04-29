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
			//for (int i = 0; i < 100; i++)
			//{
			//	foreach (var file in Directory.GetFiles(@"C:\Users\Dev\Downloads\test\"))
			//	{
			//		//using (var apiClient = new ApiPrivate("4b9adfb45af94af78d94ce75d5881c7b", "Pw8B2Jj5P7p/7DO5e/CYwKAvelQQH2knXztQ5djaiF0=", "http://localhost:88"))
			//		using (var apiClient = new ApiPrivate("ee652448256d497e8256bdd9068297b2", "WTAzqs48uTBtBI3MFr9G7/NSVJxFRxe68JcpAckixDQ=", "http://www.dotmatrix.co.nz"))
			//		using (var bitmap = new Bitmap(file))
			//		{
			//			var width = bitmap.Width;
			//			var height = bitmap.Height;
			//			var data = new List<AddPixelRequest>();
			//			for (int x = 0; x < width; x++)
			//			{

			//				for (int y = 0; y < height; y++)
			//				{
			//					var pixel = bitmap.GetPixel(x, y);
			//					var hex = HexConverter2(pixel);
			//					data.Add(new AddPixelRequest
			//					{
			//						Color = hex,
			//						X = 10 + x,
			//						Y = 400 + y
			//					});
			//				}

			//			}
			//			var result = await apiClient.AddPixels(data);
			//			Console.WriteLine(result.Message);
			//		}
			//	} 
			//}


			await DrawPicture(@"C:\Program Files\DotMatrix\Debug\test.png", 0, 0);
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
					//var data = new List<AddPixelRequest>();
					//for (int y = 0; y < height; y++)
					//{
					//	var pixel = bitmap.GetPixel(x, y);
					//	var hex = HexConverter2(pixel);
					//	data.Add(new AddPixelRequest
					//	{
					//		Color = hex,
					//		X = offsetX + x,
					//		Y = offsetY + y
					//	});
					//}
					//var result = await apiClient.AddPixels(data);
					//Console.WriteLine(result.Message);

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
		}

		private static string HexConverter(System.Drawing.Color c)
		{
			return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
		}

		private static string HexConverter2(System.Drawing.Color c)
		{
			return $"rgba({c.R},{c.G},{c.B},{c.A})";
		}
	}
}
