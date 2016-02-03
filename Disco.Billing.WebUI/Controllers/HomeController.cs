using Disco.Billing.Salesforce;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace Disco.Billing.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IOptions<Options> salesforceOptions)
        {
            SalesforceOptions = salesforceOptions.Value;
        }

        private Options SalesforceOptions { get; }

        public IActionResult Index()
        {
            ViewData["contract-count"] =
                Data.GetData(SalesforceOptions.SalesforceConsumerKey,
                    SalesforceOptions.SalesforceConsumerSecret, SalesforceOptions.SalesforceUsername,
                    SalesforceOptions.SalesforcePassword).Result.Count;
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