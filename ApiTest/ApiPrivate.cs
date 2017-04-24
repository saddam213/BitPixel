﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
	public class ApiPrivate : IDisposable
	{
		private HttpClient _client;
		private string _apiBaseAddress;

		public ApiPrivate(string key, string secret, string apiBaseAddress)
		{
			_apiBaseAddress = apiBaseAddress;
			_client = HttpClientFactory.Create(new AuthDelegatingHandler(key, secret));
		}

		public async Task<AddPixelResponse> AddPixel(AddPixelRequest requestData)
		{
			var response = await _client.PostAsJsonAsync<AddPixelRequest>($"{_apiBaseAddress.TrimEnd('/')}/Api/ApiPrivate/AddPixel", requestData);
			return await response.Content.ReadAsAsync<AddPixelResponse>();
		}

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}