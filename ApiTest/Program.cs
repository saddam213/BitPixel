using System;
using System.Collections.Generic;
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
			using (var apiClient = new ApiPrivate("4b9adfb45af94af78d94ce75d5881c7b", "Pw8B2Jj5P7p/7DO5e/CYwKAvelQQH2knXztQ5djaiF0=", "http://localhost:51641"))
			{
				for (int i = 0; i < 1000; i++)
				{
					var result = await apiClient.AddPixel(new AddPixelRequest
					{
						Color = "#0000FF",
						X = _rand.Next(0, 200),
						Y = _rand.Next(0, 200)
					});
					Console.WriteLine(result.Message);
					//await Task.Delay(200);
				}
				
			}
		}
	}
}
