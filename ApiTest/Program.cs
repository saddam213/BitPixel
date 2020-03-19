using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitPixel.Base;
using BitPixel.Common.DataContext;
using BitPixel.Data.DataContext;
using BitPixel.WalletService.Connector.Base;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
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

			var addreses = new List<string> { "CRDEqTgSCVRs3KJy3njryPxMLuSR8psWpR", "CGajpdKGujT8QMC9mPVCjUc9giT2vrAPDg", "CNrZC3GEsN59fV2LptwNaaesmJTDwV9Lry", "CdpkTxFhPBMrST26UshoxE7FPAZv4HN7L4" };
			var connector = new WalletConnector("http://192.168.1.166:8010", "548066c31a", "05450c152c949b8c040c9f3805fa41e6");
			var lastblock = connector.GetBlockCount();
			for (int i = 0; i < 1000000; i++)
			{
				try
				{
					await Task.Delay(TimeSpan.FromSeconds(30));
					var block = connector.GetBlockCount();
					if (lastblock >= block)
					{
						Console.WriteLine($"[Success] - Waiting for new block...");
						continue;
					}

					lastblock = block;
					foreach (var address in addreses)
					{
						var tx = await connector.SendToAddressAsync(address, 2);
						Console.WriteLine($"[Success] - {tx.Txid}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[Failed] - {ex.Message}");
				}


			}
		}

	}
}
