using EmployeeWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EmployeeWeb.Proxy
{
    /// <summary>
    /// Proxy class to handle the complexity of dealing with reliable service
    /// </summary>
    public class EmployeeDataAPIProxy
    {
        private ServiceContext _context = null;
        private readonly HttpClient _httpClient;
        private readonly FabricClient _fabricClient;

        public EmployeeDataAPIProxy(ServiceContext context, HttpClient httpClient,FabricClient fabricClient)
        {
            _context = context;

            _httpClient = httpClient;

            _fabricClient = fabricClient;
        }

        /// <summary>
        /// Returns the list of employees from all the partitions. In our sample we have only 1 partition
        /// Also, we are making use of proxy to determine the right partition to connect to.
        /// Please refer this link for more details. https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reverseproxy
        /// </summary>
        /// <returns></returns>
        public async Task<List<Employee>> GetEmployees()
        {
            Uri serviceName = EmployeeWeb.GetEmployeeDataServiceName(_context);

            Uri proxyAddress = this.GetProxyAddress(serviceName);

            ServicePartitionList partitions = await _fabricClient.QueryManager.GetPartitionListAsync(serviceName);

            List<Employee> employees = new List<Employee>();

            foreach (Partition partition in partitions)
            {
                string proxyUrl =
                    $"{proxyAddress}/api/Employee?PartitionKey={((Int64RangePartitionInformation)partition.PartitionInformation).LowKey}&PartitionKind=Int64Range";

                using (HttpResponseMessage response = await _httpClient.GetAsync(proxyUrl))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        continue;
                    }

                    employees.AddRange(JsonConvert.DeserializeObject<List<Employee>>(await response.Content.ReadAsStringAsync()));
                }
            }

            return employees;
        }

        /// <summary>
        /// Creates an Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task CreateEmployee(Employee employee)
        {
            Uri serviceName = EmployeeWeb.GetEmployeeDataServiceName(_context);

            Uri proxyAddress = this.GetProxyAddress(serviceName);
            
            long partitionKey = employee.Id;

            string proxyUrl = $"{proxyAddress}/api/Employee?PartitionKey={partitionKey}&PartitionKind=Int64Range";

            await this._httpClient.PostAsJsonAsync<Employee>(proxyUrl, employee);
        }


        /// <summary>
        /// Deletes an Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task DeleteEmployee(long employeeId)
        {
            Uri serviceName = EmployeeWeb.GetEmployeeDataServiceName(_context);

            Uri proxyAddress = this.GetProxyAddress(serviceName);

            long partitionKey = employeeId;

            string proxyUrl = $"{proxyAddress}/api/Employee/{employeeId}?PartitionKey={partitionKey}&PartitionKind=Int64Range";

            await this._httpClient.DeleteAsync(proxyUrl);
        }


        /// <summary>
        /// Constructs a reverse proxy URL for a given service.
        /// To find the reverse proxy port used in your local development cluster, view the HttpApplicationGatewayEndpoint element in the local Service Fabric cluster manifest:
        /// Open a browser window and navigate to http://localhost:19080 to open the Service Fabric Explorer tool.
        /// Select Cluster -> Manifest.
        /// Make a note of the HttpApplicationGatewayEndpoint element port.By default this should be 19081. If it is not 19081, you will need to change the port in the GetProxyAddress method of the following VotesController.cs code.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
        }
    }
}
