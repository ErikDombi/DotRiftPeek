using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotRiftPeek.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Pyke;

namespace DotRiftPeek
{
    public class Startup
    {
        private PykeAPI Pyke;
        public Dictionary<string, List<Function>> vals = new Dictionary<string, List<Function>>();
        public bool hasInit;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton<PykeAPI>(new PykeAPI(Serilog.Events.LogEventLevel.Debug));
            services.AddSingleton<Startup>(this);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PykeAPI pyke)
        {

            Pyke = pyke;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var options = new BrowserWindowOptions
            {
                WebPreferences = new WebPreferences
                {
                    WebSecurity = false
                },
                Width = 300,
                Height = 100,
                Center = true,
                Frame = false,
                Transparent = true,
                Show = true
            };

            var window = Electron.WindowManager.CreateWindowAsync(options).GetAwaiter().GetResult();
        }

        public void LoadData()
        {
            hasInit = true;

            Pyke.Disconnected += Pyke_Disconnected;
            Pyke.PykeReady += Pyke_PykeReady;

            Pyke.ConnectAsync().ConfigureAwait(false);
        }

        BrowserWindow mainWindow => Electron.WindowManager.BrowserWindows.FirstOrDefault();
        private async void Pyke_PykeReady(object sender, PykeAPI e)
        {
            Console.WriteLine("PYKE READY STARTING GET /HELP\n");

            HelpResponse response = await Pyke.RequestHandler.StandardGet<HelpResponse>("/Help");
            foreach (var res in response.functions)
            {
                var func = await getFunction(res.Key);
                if (string.IsNullOrWhiteSpace(func.url) || string.IsNullOrWhiteSpace(func.HttpMethod))
                    continue;
                Console.WriteLine("Added: " + func.url);
                var tag = func.url.Split("/")[1];
                if (vals.ContainsKey(tag))
                {
                    vals[tag].Add(func);
                }
                else
                {
                    vals.Add(tag, new List<Function> { func });
                }
            }
            Console.WriteLine("\nDone! Redirecting!");
            Electron.IpcMain.Send(mainWindow, "redirect", "", "/");
            mainWindow.SetSize(1300, 700);
            mainWindow.Center();
        }
        public async Task<Function> getFunction(string id)
        {
            string response = JsonConvert.SerializeObject(await Pyke.RequestHandler.StandardGet<dynamic>("/Help?target=" + id), Formatting.Indented);
            var splits = string.Join("{", response.Split("{").Skip(2)).Split("}");
            response = "{" + string.Join("}", splits.Take(splits.Length - 1));
            return JsonConvert.DeserializeObject<Function>(response);
        }

        private void Pyke_Disconnected(object sender, EventArgs e)
        {

        } 
    }
}
