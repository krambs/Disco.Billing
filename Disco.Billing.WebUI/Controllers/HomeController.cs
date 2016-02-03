using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions;
using Microsoft.Extensions.OptionsModel;

namespace Disco.Billing.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private Options SalesforceOptions { get; }

        public HomeController(IOptions<Options> salesforceOptions)
        {
            SalesforceOptions = salesforceOptions.Value;
        }

        public IActionResult Index()
        {
            ViewData["salesforce-username"] = SalesforceOptions.SalesforceUsername;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
