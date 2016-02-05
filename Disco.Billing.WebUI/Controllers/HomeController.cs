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
            return View();
        }

        [HttpGet]
        public IActionResult BillingData()
        {
            var data = Data.Disco.Disco.GetData(SalesforceOptions).GroupBy(discoContract => discoContract.BillingAccount);

            return Json(data);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}