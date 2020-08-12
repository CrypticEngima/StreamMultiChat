using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StreamMultiChat.Blazor.Settings;



namespace StreamMultiChat.Blazor.Extensions
{
	public static class StartupExtensions
	{
		public static IServiceCollection AddTwitchSettings(this IServiceCollection services, IConfiguration Configuration)
		{
			var ts = new TwitchSettings();
			//ts.Username = (string)Configuration.GetValue(typeof(string), "TwitchSettings:Username");
			//ts.Token = (string)Configuration.GetValue(typeof(string), "TwitchSettings:Token");
			ts.ClientId = (string)Configuration.GetValue(typeof(string), "TwitchSettings:ClientId");
			ts.ClientSecret = (string)Configuration.GetValue(typeof(string), "TwitchSettings:ClientSecret");

			if (string.IsNullOrEmpty(ts.ClientId) || string.IsNullOrEmpty(ts.ClientSecret))                //|| string.IsNullOrEmpty(ts.ClientId)||string.IsNullOrEmpty(ts.ClientSecret) string.IsNullOrEmpty(ts.Username) || string.IsNullOrEmpty(ts.Token)
			{
				throw new ArgumentNullException("Check configuration settings for Twitch ClientId or ClientSecret");
			}

			services.AddSingleton(ts);

			return services;
		}
	}
}
