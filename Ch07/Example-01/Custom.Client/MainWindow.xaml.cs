using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Identity.Client;

namespace Custom.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PublicClientApplication _publicClientApp = null;

        //sample API fccaa2fa-fe79-42fa-b8d5-e0ac6061ae99/allowaccess
        private string _apiScope = "YOUR API SCOPE";

        private string _apiGatewayEmployeeUrl = "http://localhost:<<gateway port>>/employee";

        private string _apiGatewayValuesUrl = "http://localhost:<<gateway port>>/value";

        public MainWindow()
        {
            InitializeComponent();

            string clientId = "<<HRClient Client Id>>";

            string tenantId = "<<YOUR TENANT ID>>";

            //sample redirect uri : hrclient://auth

            string redirectUri = "<<HRClient Redirect URI>>";

            _publicClientApp = new PublicClientApplication(clientId, $"https://login.microsoftonline.com/{tenantId}");

            _publicClientApp.RedirectUri = redirectUri;

        }

        /// <summary>
        /// Invokes Employees API Via API Gateway
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnEmployeeClick(object sender, RoutedEventArgs e)
        {
            var authResult = await _publicClientApp.AcquireTokenAsync(new string[] { _apiScope }).ConfigureAwait(false);

            var employeResult = await GetHttpContentWithToken(_apiGatewayEmployeeUrl, authResult.AccessToken);

            MessageBox.Show($"Employee Result : {employeResult}");
        }

        /// <summary>
        /// Invokes Values API Via API Gateway
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnValuesClick(object sender, RoutedEventArgs e)
        {
            var valueResult = await GetHttpContentWithToken(_apiGatewayValuesUrl, string.Empty);

            MessageBox.Show($"Value Result : {valueResult}");
        }

        /// <summary>
        /// Makes an Http Call to API Gateway
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;

            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
               
                if (!string.IsNullOrEmpty(token))
                {
                    //Add the token in Authorization header
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
