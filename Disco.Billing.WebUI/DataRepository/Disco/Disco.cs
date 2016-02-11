using System;
using System.Collections.Generic;
using System.Linq;
using Disco.Billing.WebUI.DataRepository.Salesforce;

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
                var discoContract = new Contract
                {
                    Type = salesforceContract.Type__c,
                    BillingAccount = CreateDiscoAccountFromAccountId(salesforceData,
                        salesforceContract.BillingAccount__c),
                    UserAccount = CreateDiscoAccountFromAccountId(salesforceData,
                        salesforceContract.AccountId)
                };

                if (salesforceContract.Matters__r != null)
                {
                    discoContract.Matters.AddRange(
                        salesforceContract.Matters__r.Records.Select(
                            salesforceMatter => CreateDiscoMatterFromMatterId(salesforceData, salesforceMatter.Id)));
                }

                discoContractsToReturn.Add(discoContract);
            }

            return discoContractsToReturn;
        }

        private static Account CreateDiscoAccountFromAccountId(SalesforceData salesforceData, string accountId)
        {
            var salesforceAccount = salesforceData.Accounts.Single(account => account.Id.Equals(accountId));
            return new Account
            {
                Id = salesforceAccount.Id,
                Name = salesforceAccount.Name
            };
        }

        private static Matter CreateDiscoMatterFromMatterId(SalesforceData salesforceData, string matterId)
        {
            var salesforceMatter = salesforceData.Matters.Single(matter => matter.Id.Equals(matterId));
            return new Matter
            {
                Id = salesforceMatter.Id,
                Name = salesforceMatter.Name,
                DeactivationDate = salesforceMatter.DeactivationDate__c,
                DataSets =
                    salesforceMatter.Datasets__r?.Records.Select(
                        dataSet => new DataSet {CreatedDate = dataSet.CreatedDate, DataSize = dataSet.Data_Size__c})
                        .ToList() ?? new List<DataSet>()
            };
        }
    }

    public class Contract
    {
        public Account BillingAccount { get; set; }
        public Account UserAccount { get; set; }
        public List<Matter> Matters { get; set; } = new List<Matter>();
        public string Type { get; set; }
    }

    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Matter
    {
        public string Id { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public string Name { get; set; }
        public List<DataSet> DataSets { get; set; }
    }

    public class DataSet
    {
        public decimal DataSize { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}