using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamMultiChat.Blazor.Services;
using StreamMultiChat.Blazor.Settings;

namespace StreamMultiChat.Blazor
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//var section = Configuration.GetSection("TwitchSettings");

			var ts = new TwitchSettings();
			ts.Username = (string)Configuration.GetValue(typeof(string), "TwitchSettings:Username");
			ts.Token = (string)Configuration.GetValue(typeof(string), "TwitchSettings:Token");

			services.AddLogging();
			//services.Configure<TwitchSettings>(section);
			services.AddSingleton(ts);
			services.AddSingleton<TwitchService>();
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddSignalR();
			services.AddBlazoredModal();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
