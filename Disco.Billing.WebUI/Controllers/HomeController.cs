using System;
using System.Collections.Generic;
using System.Linq;
using Disco.Billing.WebUI.DataRepository.Disco;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.OptionsModel;

namespace Disco.Billing.WebUI.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : Controller
    {
        private const string BillingDataCacheKey = "BillingData";

        public HomeController(IOptions<Options> salesforceOptions, IMemoryCache cache)
        {
            Cache = cache;
            SalesforceOptions = salesforceOptions.Value;
        }

        private Options SalesforceOptions { get; }
        private IMemoryCache Cache { get; }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BillingData()
        {
            var contractsGroupedByBillingAccount =
                Cache.Get(BillingDataCacheKey) as IEnumerable<IGrouping<string, Contract>>;
            if (contractsGroupedByBillingAccount == null)
            {
                contractsGroupedByBillingAccount =
                    Data.GetData(SalesforceOptions).GroupBy(discoContract => discoContract.BillingAccount.Id);
                Cache.Set(BillingDataCacheKey, contractsGroupedByBillingAccount,
                    new MemoryCacheEntryOptions {SlidingExpiration = TimeSpan.FromHours(1)});
            }

            return
                Json(
                    contractsGroupedByBillingAccount.Select(
                        group =>
                            new
                            {
                                BillingAccountName = group.First().BillingAccount.Name,
                                BillingAccountId = group.First().BillingAccount.Id,
                                Contracts = group.ToList()
                            }).Where(group => group.BillingAccountName.Contains("Castle")).OrderBy(group => group.BillingAccountName));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}