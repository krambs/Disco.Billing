using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;

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
            return View();
        }

        [HttpGet]
        public IActionResult BillingData()
        {
            var data = Data.Salesforce.GetData(SalesforceOptions.SalesforceConsumerKey, SalesforceOptions.SalesforceConsumerSecret,
                SalesforceOptions.SalesforceUsername, SalesforceOptions.SalesforcePassword)
                .Result.GroupBy(contract => contract.AccountId);

            return Json(data);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}