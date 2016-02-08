using System.Collections.Generic;
using System.Linq;

namespace Disco.Billing.WebUI.DataRepository.Disco
{
    public class Data
    {
        public static List<Contract> GetData(Options salesforceOptions)
        {
            var discoContractsToReturn = new List<Contract>();

            var salesforceData = Salesforce.Data.GetData(salesforceOptions);
            foreach (var salesforceContract in salesforceData.Contracts)
            {
                var discoContract = new Contract();

                var salesforceBillingAccount =
                    salesforceData.Accounts.Single(
                        salesforceAccount => salesforceAccount.Id.Equals(salesforceContract.BillingAccount__c));
                discoContract.BillingAccount = new Account
                {
                    Id = salesforceBillingAccount.Id,
                    Name = salesforceBillingAccount.Name
                };

                var salesforceUserAccount =
                    salesforceData.Accounts.Single(
                        salesforceAccount => salesforceAccount.Id.Equals(salesforceContract.AccountId));
                discoContract.UserAccount = new Account
                {
                    Id = salesforceUserAccount.Id,
                    Name = salesforceUserAccount.Name
                };

                if (salesforceContract.Matters__r != null)
                {
                    foreach (var matter in salesforceContract.Matters__r.Records)
                    {
                        discoContract.Matters.Add(new Matter {Id = matter.Id});
                    }
                }

                discoContractsToReturn.Add(discoContract);
            }

            return discoContractsToReturn;
        }
    }

    public class Contract
    {
        public Account BillingAccount { get; set; }
        public Account UserAccount { get; set; }
        public List<Matter> Matters { get; set; } = new List<Matter>();
    }

    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Matter
    {
        public string Id { get; set; }
    }
}