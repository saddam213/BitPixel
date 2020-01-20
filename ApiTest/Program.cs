using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Base;
using Newtonsoft.Json;

namespace ApiTest
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().Wait();
			Console.ReadKey();
		}
		static async Task MainAsync()
		{
			int threads = 100;

			//for (int i = 0; i < 14; i++)
			//{
			//await DrawPicture(threads, @"C:\Users\Deven\Pictures\Litecoin.png", 70 * i, 70 * i);
			//	}

			//for (int i = 0; i < 10000000; i++)
			//{
			//	await DrawPicture(threads, @"C:\Users\Deven\Pictures\sa_ddam213.jpg", 0, 0);
			//	await Task.Delay(TimeSpan.FromSeconds(5));
			//}

			//ait DrawPicture(threads, @"C:\Users\Deven\Pictures\doge.png", 0, 628);
			//for (int i = 0; i < 10000000; i++)
			//{
				await DrawPicture(threads, @"C:\Users\Deven\Pictures\246x0w.jpg", 20,20);
			//	await Task.Delay(TimeSpan.FromSeconds(1));
			//}
			//await DrawAI2(2000, 0, 550);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 20, 600);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 100, 600);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 200, 600);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 300, 600);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 400, 600);
			//DrawPicture(threads, @"C:\Users\Deven\Pictures\smile.png", 500, 500);
		}

		
		private static async Task DrawAI2(int threads, int offsetX, int offsetY)
		{
			string apiHost = "https://dotmatrix.chainstack.nz";
			string apiPublicKey = "1a4cfeed5aea423c942142e88d8f5033";
			string apiPrivateKey = "8thxiu2FuuFoRnfgDCL+R1EH/ZZ8V0eoPCPp8AYIndQ=";

		
			int width = 1000;
			int height = 100;
			var pixelRequests = new List<PixelRequest>();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var pixelRequest = new PixelRequest
					{

						X = offsetX + x,
						Y = offsetY + y,
						ApiKey = apiPublicKey
					};
				
					pixelRequests.Add(pixelRequest);
				}
			}

			var random = new Random();
			var tasks = new List<Task>();
			var count = pixelRequests.Count / threads;
			foreach (var batch in pixelRequests.OrderBy(x => x.Y * x.X % Math.PI).Batch(count))
			{
				var color = _colors.OrderBy(x => Guid.NewGuid()).First();
				tasks.Add(Task.Run(async () =>
				{
					using (var client = new HttpClient())
					{
						foreach (var pixelRequest in batch)
						{

							pixelRequest.R = color[0];
							pixelRequest.G = color[1];
							pixelRequest.B = color[2];
							pixelRequest.Signature = CreateSignature(apiPrivateKey, pixelRequest.GetRequestBody());
							await client.PostAsJsonAsync($"{apiHost}/Api/AddPixel", pixelRequest);
						}
					}
				}));
			}
			await Task.WhenAll(tasks);
		}

		private static List<byte[]> _colors = new List<byte[]> { new byte[] { 0, 0, 255 }, new byte[] { 255, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 }, new byte[] { 0, 0, 0 } };

		private static async Task DrawAI(int threads, int offsetX, int offsetY)
		{
			string apiHost = "https://dotmatrix.chainstack.nz";
			string apiPublicKey = "1a4cfeed5aea423c942142e88d8f5033";
			string apiPrivateKey = "8thxiu2FuuFoRnfgDCL+R1EH/ZZ8V0eoPCPp8AYIndQ=";

			var wordColors = new Queue<byte[]>();
			foreach (var line in File.ReadAllLines(@"C:\Users\Deven\Desktop\bitcoin.txt"))
			{
				foreach (var word in line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
				{
					var test = StringToByte(string.Format("{0:X4}", word.GetHashCode()));
					if (test == null || test.Length < 4)
						continue;

					wordColors.Enqueue(test);
				}
			}

			int width = 50;
			int height = 50;
			var pixelRequests = new List<PixelRequest>();
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var color = wordColors.Dequeue();
					var pixelRequest = new PixelRequest
					{

						X = offsetX + y,
						Y = offsetY + x,
						R = color[1],
						G = color[2],
						B = color[3],
						ApiKey = apiPublicKey
					};
					pixelRequest.Signature = CreateSignature(apiPrivateKey, pixelRequest.GetRequestBody());
					pixelRequests.Add(pixelRequest);
				}
			}

			var tasks = new List<Task>();
			var count = pixelRequests.Count / threads;
			foreach (var batch in pixelRequests.Batch(count))
			{
				tasks.Add(Task.Run(async () =>
				{
					using (var client = new HttpClient())
					{
						foreach (var item in batch)
						{
							await client.PostAsJsonAsync($"{apiHost}/Api/AddPixel", item);
						}
					}
				}));
			}
			await Task.WhenAll(tasks);
		}


		public static byte[] StringToByte(string hex)
		{
			if (hex.Length % 2 == 1)
				return null;

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		public static int GetHexVal(char hex)
		{
			int val = (int)hex;
			return val - (val < 58 ? 48 : 55);
		}

		private static async Task DrawPicture(int threads, string filename, int offsetX, int offsetY)
		{
			string apiHost = "https://dotmatrix.chainstack.nz";
			string apiPublicKey = "1a4cfeed5aea423c942142e88d8f5033";
			string apiPrivateKey = "8thxiu2FuuFoRnfgDCL+R1EH/ZZ8V0eoPCPp8AYIndQ=";

			using (var bitmap = new Bitmap(filename))
			{
				var width = bitmap.Width;
				var height = bitmap.Height;

				var pixelRequests = new List<PixelRequest>();
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						var pixel = bitmap.GetPixel(x, y);
						//if (pixel.R == 0 && pixel.B == 0 && pixel.G == 0)
						//	continue;

						//if (pixel.R == 255 && pixel.B == 255 && pixel.G == 255)
						//	continue;

						var pixelRequest = new PixelRequest
						{
							X = offsetX + x,
							Y = offsetY + y,
							R = pixel.R,
							G = pixel.G,
							B = pixel.B,
							//R = 255,
							//G = 255,
							//B = 255,
							ApiKey = apiPublicKey
						};

						pixelRequest.Signature = CreateSignature(apiPrivateKey, pixelRequest.GetRequestBody());
						pixelRequests.Add(pixelRequest);
					}
				}

				var tasks = new List<Task>();
				var count = pixelRequests.Count / threads;
				foreach (var batch in pixelRequests.OrderBy(x => Guid.NewGuid()).Batch(count))
				{
					tasks.Add(Task.Run(async () =>
					{
						using (var client = new HttpClient())
						{
							foreach (var item in batch)
							{
								await client.PostAsJsonAsync($"{apiHost}/Api/AddPixel", item);
							}
						}
					}));
				}
				await Task.WhenAll(tasks);
			}
		}


		public class PixelRequest
		{
			public int X { get; set; }
			public int Y { get; set; }
			public byte R { get; set; }
			public byte G { get; set; }
			public byte B { get; set; }
			public string ApiKey { get; set; }
			public string Signature { get; set; }
			public string GetRequestBody()
			{
				return $"{nameof(X)}={X}&{nameof(Y)}={Y}&{nameof(R)}={R}&{nameof(G)}={G}&{nameof(B)}={B}";
			}
		}

		public class PixelResponse
		{
			public bool Success { get; set; }
			public string Message { get; set; }
		}

		private static async Task<PixelResponse> SendPixel(PixelRequest pixel)
		{
			string apiHost = "https://dotmatrix.chainstack.nz";
			string apiPublicKey = "1a4cfeed5aea423c942142e88d8f5033";
			string apiPrivateKey = "8thxiu2FuuFoRnfgDCL+R1EH/ZZ8V0eoPCPp8AYIndQ=";

			pixel.ApiKey = apiPublicKey;
			pixel.Signature = CreateSignature(apiPrivateKey, pixel.GetRequestBody());

			using (var client = new HttpClient())
			{
				var response = await client.PostAsJsonAsync($"{apiHost}/Api/AddPixel", pixel);
				return JsonConvert.DeserializeObject<PixelResponse>(await response.Content.ReadAsStringAsync());
			}
		}


		public static string CreateSignature(string secretKey, string dataToSign)
		{
			using (var hmacSha512 = new HMACSHA512(Encoding.ASCII.GetBytes(secretKey)))
			{
				return BitConverter.ToString(hmacSha512.ComputeHash(Encoding.ASCII.GetBytes(dataToSign))).Replace("-", string.Empty);
			}
		}
	}
}
