using System.Linq;
using Disco.Billing.WebUI.DataRepository.Disco;
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
            var contractsGroupedByBillingAccount =
                Data.GetData(SalesforceOptions).GroupBy(discoContract => discoContract.BillingAccount.Id);

            return
                Json(
                    contractsGroupedByBillingAccount.Select(
                        group =>
                            new
                            {
                                BillingAccountName = group.First().BillingAccount.Name,
                                BillingAccountId = group.First().BillingAccount.Id,
                                Contracts = group.ToList()
                            }).OrderBy(group => group.BillingAccountName));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}