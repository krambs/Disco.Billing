using System.Linq;
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
            ViewData["buttsauce"] = Data.Salesforce.GetData(SalesforceOptions.SalesforceConsumerKey, SalesforceOptions.SalesforceConsumerSecret,
                SalesforceOptions.SalesforceUsername, SalesforceOptions.SalesforcePassword).Result.GroupBy(contract => contract.AccountId).Count();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}