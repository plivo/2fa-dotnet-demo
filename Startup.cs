using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _2fa
{
	public class Startup
	{

		public static IConfiguration Configuration
		{
			get;
			set;
		}

		public Startup()
		{
			var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{

			// Add functionality to inject IOptions<T>
			services.AddOptions();

			services.AddMvc();
			services.AddSingleton<IConfiguration>(Configuration);

			services.AddControllersWithViews();

			// Add our Config object so it can be injected
			//services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
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
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllerRoute(
				name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}