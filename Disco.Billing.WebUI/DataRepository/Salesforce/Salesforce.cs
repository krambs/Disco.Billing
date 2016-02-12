using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace Disco.Billing.WebUI.DataRepository.Salesforce
{
    public class Data
    {
        public static readonly string ContractsSOQLQuery =
            "SELECT Id, AccountId, BillingAccount__c, Type__c, EndDate, SubscriptionPricePerMonth__c, SubscriptionPerGbOveragePrice__c, SubscriptionDataLimit__c, Price_Per_GB__c, Price_per_GB_Cold_Storage__c, MinimumContractCharge__c, MinimumMatterCharge__c, MaximumMatterCharge__c, InvoicePerMatter__c, InvoiceFrequency__c, InvoiceByAccount__c, ForwardBilling__c, StartDate, (SELECT Id FROM Matters__r) FROM Contract";

        public static readonly string AccountsSOQLQuery = "SELECT Id, Name FROM Account";

        public static readonly string MattersSOQLQuery =
            "SELECT DeactivationDate__c, Name, Id, (SELECT Data_Size__c,CreatedDate FROM Datasets__r) FROM Matter__c";

        public static SalesforceData GetData(Options salesforceOptions)
        {
            var salesforceData = new SalesforceData();

            var forceClient = GetSalesforceAPIClient(salesforceOptions).Result;

            var getContractsTask = GetSalesforceObjects<Contract>(forceClient, ContractsSOQLQuery);
            var getAccountsTask = GetSalesforceObjects<Account>(forceClient, AccountsSOQLQuery);
            var getMattersTask = GetSalesforceObjects<Matter__c>(forceClient, MattersSOQLQuery);

            salesforceData.Contracts = getContractsTask.Result;
            salesforceData.Accounts = getAccountsTask.Result;
            salesforceData.Matters = getMattersTask.Result;

            return salesforceData;
        }

        private static async Task<List<T>> GetSalesforceObjects<T>(ForceClient forceClient, string soqlQuery)
        {
            var salesforceObjectsToReturn = new List<T>();

            var queryResult = await forceClient.QueryAsync<T>(soqlQuery);
            salesforceObjectsToReturn.AddRange(queryResult.Records);

            while (!string.IsNullOrEmpty(queryResult.NextRecordsUrl))
            {
                queryResult = await forceClient.QueryContinuationAsync<T>(queryResult.NextRecordsUrl);
                salesforceObjectsToReturn.AddRange(queryResult.Records);
            }

            return salesforceObjectsToReturn;
        }

        private static async Task<ForceClient> GetSalesforceAPIClient(Options salesforceOptions)
        {
            var authenticationClient = new AuthenticationClient();
            await authenticationClient.UsernamePasswordAsync(salesforceOptions.SalesforceConsumerKey,
                salesforceOptions.SalesforceConsumerSecret,
                salesforceOptions.SalesforceUsername, salesforceOptions.SalesforcePassword);

            var instanceUrl = authenticationClient.InstanceUrl;
            var accessToken = authenticationClient.AccessToken;
            var apiVersion = authenticationClient.ApiVersion;

            return new ForceClient(instanceUrl, accessToken, apiVersion);
        }
    }

    public class SalesforceData
    {
        public List<Contract> Contracts { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Matter__c> Matters { get; set; }
    }

    public class Contract
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string BillingAccount__c { get; set; }
        public Matters__r Matters__r { get; set; }
        public string Type__c { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? SubscriptionPricePerMonth__c { get; set; }
        public decimal? SubscriptionPerGbOveragePrice__c { get; set; }
        public decimal? SubscriptionDataLimit__c { get; set; }
        public decimal? Price_Per_GB__c { get; set; }
        public decimal? Price_per_GB_Cold_Storage__c { get; set; }
        public decimal? MinimumContractCharge__c { get; set; }
        public decimal? MinimumMatterCharge__c { get; set; }
        public decimal? MaximumMatterCharge__c { get; set; }
        public bool InvoicePerMatter__c { get; set; }
        public decimal? InvoiceFrequency__c { get; set; }
        public bool InvoiceByAccount__c { get; set; }
        public bool ForwardBilling__c { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Matters__r
    {
        public List<Matter__c> Records { get; set; }
    }

    public class Matter__c
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DeactivationDate__c { get; set; }
        public Datasets__r Datasets__r { get; set; }
    }

    public class Datasets__r
    {
        public List<Dataset__c> Records { get; set; }
    }

    public class Dataset__c
    {
        public decimal Data_Size__c { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}