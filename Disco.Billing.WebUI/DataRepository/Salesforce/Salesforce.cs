using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force;

namespace Disco.Billing.WebUI.DataRepository.Salesforce
{
    public class Data
    {
        public static SalesforceData GetData(Options salesforceOptions)
        {
            var salesforceData = new SalesforceData();

            var forceClient = GetSalesforceAPIClient(salesforceOptions).Result;

            var getContractsTask = GetSalesforceObjects<Contract>(forceClient);
            var getAccountsTask = GetSalesforceObjects<Account>(forceClient);

            salesforceData.Contracts = getContractsTask.Result;
            salesforceData.Accounts = getAccountsTask.Result;

            return salesforceData;
        }

        private static async Task<List<T>> GetSalesforceObjects<T>(ForceClient forceClient)
        {
            List<T> salesforceObjectsToReturn = new List<T>();

            QueryResult<T> queryResult = await forceClient.QueryAsync<T>(CreateSQLQueryFromSalesforceObject<T>());
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

        private static string CreateSQLQueryFromSalesforceObject<T>()
        {
            return
                $"SELECT {string.Join(", ", GetPropertyNamesFromObject<T>())} FROM {typeof (T).Name}";
        }

        private static IEnumerable<string> GetPropertyNamesFromObject<T>()
        {
            return typeof(T).GetProperties().Select(property => property.Name);
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
    }

    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}