using System;
using System.Collections.Generic;
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
            var contracts = await client.QueryAsync<Contract>("SELECT Id, AccountId FROM Contract");
            foreach (var contract in contracts.Records)
            {
                Console.WriteLine(contract.AccountId);
            }

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
    }

    public class Contract
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
    }
}