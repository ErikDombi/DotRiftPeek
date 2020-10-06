using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotRiftPeek.Models;
using Pyke;
using Newtonsoft.Json;

namespace DotRiftPeek.Controllers
{
    public class HomeController : Controller
    {
        private Startup Startup;
        private PykeAPI Pyke;

        public HomeController(Startup starup, PykeAPI pyke)
        {
            this.Pyke = pyke;
            this.Startup = starup;
        }

        [Route("/")]
        public IActionResult Index()
        {
            if (!this.Startup.hasInit)
            {
                this.Startup.LoadData();
                return View(viewName: "/Views/Home/Loading.cshtml");
            }
            else
            {
                return View("Index", Startup.vals);
            }
        }

        [Route("/Endpoints/")]
        public IActionResult Home()
        {
            Console.WriteLine("Index View");
            return View("Index", Startup.vals);
        }

        [Route("/Endpoint/")]
        public async Task<IActionResult> Endpoint(string id)
        {
            string response = JsonConvert.SerializeObject(await Pyke.RequestHandler.StandardGet<dynamic>("/Help?target=" + id), Formatting.Indented);
            var splits = string.Join("{", response.Split("{").Skip(2)).Split("}");
            response = "{" + string.Join("}", splits.Take(splits.Length - 1));
            Console.WriteLine(response);
            var obj = JsonConvert.DeserializeObject<Function>(response);
            return View(model: obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
