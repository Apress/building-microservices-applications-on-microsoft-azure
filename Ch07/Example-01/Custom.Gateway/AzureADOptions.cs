using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Custom.Gateway
{
    /// <summary>
    /// Class representing AzureAd section of Appsettings.json
    /// </summary>
    public class AzureADOptions
    {
        public string Instance { get; set; }

        public string Domain { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }
  
    }
}
