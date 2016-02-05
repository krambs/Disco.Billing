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

                discoContractsToReturn.Add(discoContract);
            }

            return discoContractsToReturn;
        }
    }

    public class Contract
    {
        public Account BillingAccount { get; set; }
        public Account UserAccount { get; set; }
    }

    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}