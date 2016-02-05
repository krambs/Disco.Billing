using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace Disco.Billing.WebUI.Data
{
    public class Salesforce
    {
        public static async Task<List<Contract>> GetData(string consumerKey, string consumerSecret, string username,
            string password)
        {
            var client = await GetSalesforceAPIClient(consumerKey, consumerSecret, username, password);
            var contracts = await client.QueryAsync<Contract>(CreateSQLQueryFromSalesforceObject(new Contract()));
            return contracts.Records;
        }

        private static async Task<ForceClient> GetSalesforceAPIClient(string consumerKey, string consumerSecret,
            string username, string password)
        {
            var authenticationClient = new AuthenticationClient();
            await authenticationClient.UsernamePasswordAsync(consumerKey, consumerSecret, username, password);

            var instanceUrl = authenticationClient.InstanceUrl;
            var accessToken = authenticationClient.AccessToken;
            var apiVersion = authenticationClient.ApiVersion;

            return new ForceClient(instanceUrl, accessToken, apiVersion);
        }

        private static string CreateSQLQueryFromSalesforceObject(object salesforceObjectToCreateSQLQueryFor)
        {
            return
                $"SELECT {string.Join(", ", GetPropertyNamesFromObject(salesforceObjectToCreateSQLQueryFor))} FROM {salesforceObjectToCreateSQLQueryFor.GetType().Name}";
        }

        private static IEnumerable<string> GetPropertyNamesFromObject(object objectToGetPropertyNamesFrom)
        {
            return objectToGetPropertyNamesFrom.GetType().GetProperties().Select(property => property.Name);
        }
    }

    public class Contract
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string BillingAccount__c { get; set; }
    }
}