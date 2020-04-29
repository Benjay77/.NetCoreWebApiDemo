using IdentityModel.Client;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityModel;

namespace IdentityServerConsoleClient
{
    public class ISHelper
    {
        public static async Task RequestApi()
        {
            try
            {
                //.NetCore2.x版本
                //DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                //if (disco.IsError)
                //{
                //    Console.WriteLine(disco.Error);
                //    return;
                //}
                //TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "Client", "secret");

                //.NetCore3.x版本
                // discovery endpoint
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

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
                var response = await apiClient.GetAsync("http://localhost:5000/api/Client/");
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

        public static async Task PostClient()
        {
            try
            {
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

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
                var postClient = new HttpClient();
                postClient.SetBearerToken(tokenResponse.AccessToken);

                Client c1 = new Client
                {
                    ClientId = "superAdmin",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "IdentityServerApi" },
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                };
                string strJson = JsonConvert.SerializeObject(c1.ToEntity());
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //由HttpClient发出Post请求
                Task<HttpResponseMessage> response = postClient.PostAsync("http://localhost:5000/api/Client/", content);

                if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Result.StatusCode);
                }
                else
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static async Task Admin()
        {
            try
            {
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

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
                var adminClient = new HttpClient();
                adminClient.SetBearerToken(tokenResponse.AccessToken);

                Client c2 = new Client
                {
                    ClientId = "admin",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "IdentityServerApi" }
                    //,Claims = new List<Claim>
                    //{
                    //    new Claim(JwtClaimTypes.Role, "admin")
                    //},
                    //ClientClaimsPrefix = "" //把client_ 前缀去掉
                };
                string strJson = JsonConvert.SerializeObject(c2.ToEntity());
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //由HttpClient发出Post请求
                Task<HttpResponseMessage> response = client.PostAsync("http://localhost:5000/api/Client/", content);

                if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine(response.Result.StatusCode);
                }
                else
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
