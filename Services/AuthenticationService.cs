using Microsoft.Extensions.Options;
using StreamMultiChat.Blazor.Modals;
using StreamMultiChat.Blazor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class AuthenticationService
	{
		private TwitchSettings _twitchSettings;
		
		private IHttpClientFactory _clientFactory;
		public TwitchUser TwitchUser { get; set; }
		public string Token { get; set; }


		public bool IsAuthenticated { get; set; } = false;
		public AuthenticationService(TwitchSettings twitchSettings, IHttpClientFactory clientFactory)
		{
			_twitchSettings = twitchSettings;
			_clientFactory = clientFactory;
		}

		public void ResponseReceived(string fragment)
		{
			var FragmentDictionary = ParseFragmentToDictionary(fragment);

			Token = FragmentDictionary["access_token"];
			if(!(Token is null))
			{
				GetUserInfo();
			}

			if(!(TwitchUser is null))
			{
				IsAuthenticated = true;
			}
		}

		Dictionary<string,string> ParseFragmentToDictionary(string fragment)
		{
			var dict = new Dictionary<string, string>();
			fragment = fragment.Remove(0,1);
			var split = fragment.Split('&');
			foreach (var item in split)
			{
				var kvp = item.Split('=');

				dict.Add(kvp[0], kvp[1]);
			}

			return dict;
		}


		void GetUserInfo()
		{
			var client = _clientFactory.CreateClient();
			TwitchUserResponseJson jsonString = null;
			client.BaseAddress = new Uri("https://api.twitch.tv/helix/users");

			var request = new HttpRequestMessage()
			{
				Method = HttpMethod.Get
			};

			request.Headers.Add("client-id", _twitchSettings.ClientId);
			request.Headers.Add("Authorization", $"Bearer {Token}");

			var task = client.SendAsync(request)
				.ContinueWith((taskWithMsg) =>
				{
					var response = taskWithMsg.Result;

					var jsonTask = response.Content.ReadAsStringAsync();
					jsonTask.Wait();

					jsonString = JsonSerializer.Deserialize<TwitchUserResponseJson>(jsonTask.Result);

				});
			task.Wait();

			TwitchUser = jsonString.data.FirstOrDefault();
		}

	}
}
