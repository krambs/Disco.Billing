using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force;

namespace Disco.Billing.Salesforce
{
    public class Data
    {
        public static void Main()
        {
            Task task = GetData();
            task.Wait();

        }

        private static async Task GetData()
        {
            ForceClient client = await GetSalesforceAPIClient();
            QueryResult<Contract> contracts = await client.QueryAsync<Contract>("SELECT Id, AccountId FROM Contract");
            foreach (var contract in contracts.Records)
            {
                Console.WriteLine(contract.AccountId);
            }
        }

        private static async Task<ForceClient> GetSalesforceAPIClient()
        {
            var consumerkey = "3MVG9xOCXq4ID1uH0ryjWe8f1uFUbiypZoLbUoX0V152kKakkInuJfMbZloj8Mt5LbLAbwGcv5s97PG3oV0yH";
            var consumersecret = "8899212903892392295";
            var username = "hagan@csdisco.com";
            var password = "B364277YJT8q2Qp";

            var authenticationClient = new AuthenticationClient();
            await authenticationClient.UsernamePasswordAsync(consumerkey, consumersecret, username, password);

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
