using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerConsoleClient
{
    public class ISHelper
    {
        public static async Task RequestApi() 
        {
			try
			{
                // discovery endpoint
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }
                //新版抛弃tokenclient
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "IdentityServerConsoleClient",
                    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                    Scope = "IdentityServerApi"
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }
                Console.WriteLine(tokenResponse.Json);

                // call Identity Resource API
                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse.AccessToken);
                var response = await client.GetAsync("http://localhost:5000/api/Client/");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
				throw;
			}
        }
    }
}
