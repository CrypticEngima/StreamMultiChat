using Microsoft.Extensions.Options;
using StreamMultiChat.Blazor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamMultiChat.Blazor.Services
{
	public class AuthenticationService
	{
		private TwitchSettings _twitchSettings;
		public AuthenticationService(IOptions<TwitchSettings> twitchSettings)
		{
			_twitchSettings = twitchSettings.Value;
		}
	}
}
