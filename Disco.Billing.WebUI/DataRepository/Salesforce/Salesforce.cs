using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace Disco.Billing.WebUI.DataRepository.Salesforce
{
    public class Data
    {
        public static readonly string ContractsSOQLQuery =
            "SELECT Id, AccountId, BillingAccount__c, (SELECT Id FROM Matters__r) FROM Contract";

        public static readonly string AccountsSOQLQuery = "SELECT Id, Name FROM Account";

        public static SalesforceData GetData(Options salesforceOptions)
        {
            var salesforceData = new SalesforceData();

            var forceClient = GetSalesforceAPIClient(salesforceOptions).Result;

            var getContractsTask = GetSalesforceObjects<Contract>(forceClient, ContractsSOQLQuery);
            var getAccountsTask = GetSalesforceObjects<Account>(forceClient, AccountsSOQLQuery);

            salesforceData.Contracts = getContractsTask.Result;
            salesforceData.Accounts = getAccountsTask.Result;

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
    }

    public class Contract
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string BillingAccount__c { get; set; }
        public Matters__r Matters__r { get; set; }
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
        public Datasets__r Datasets__r { get; set; }
    }

    public class Datasets__r
    {
        public List<Dataset__c> Records { get; set; }
    }

    public class Dataset__c
    {
        public decimal Data_Size__c { get; set; }
    }
}