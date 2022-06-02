using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public class TdVeoImportWebService : ITdVeoImportWebService, IDisposable
    {
        public Uri BaseAddress { get; private set; }

        public string ApiKey { private set; get; }

        public string SesionToken { private set; get; }

        public string LogoutAdress { private set; get; }

        /// <summary>
        /// Token expiration time in UTC
        /// </summary>
        public DateTime? TokenExpirationTime { private set; get; }

        public bool IsAuthorised
        {
            get
            {
                return TokenExpirationTime != null && DateTime.Compare(TokenExpirationTime.Value, DateTime.Now.ToUniversalTime().AddMinutes(5)) > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAdrress">REST API base http address</param>
        public TdVeoImportWebService(IConfiguration configuration, string baseAdrress)
        {
            if (configuration["TDVeoURI"] != null && configuration["TDVeoAPIkey"] != null)
            {
                BaseAddress = new Uri(baseAdrress);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress">REST API base http address</param>
        /// <param name="apiKey">REST API key</param>
        public TdVeoImportWebService(string baseAddress, string apiKey)
        {
            if (baseAddress != null && apiKey != null)
            {
                BaseAddress = new Uri(baseAddress);
                ApiKey = apiKey;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
        }

        /// <summary>
        /// Creates a new instance with Base Address taken from the config file
        /// </summary>
        public TdVeoImportWebService(IConfiguration configuration)
        {
            if (configuration["TDVeoURI"] != null && configuration["TDVeoAPIkey"] != null)
            {
                BaseAddress = new Uri(configuration["TDVeoURI"]);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
        }


        private HttpClient GetTdVeoHttpClient()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = this.BaseAddress
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            // Add authorization header if authorized
            if (IsAuthorised)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "TDAToken " + ApiKey + " " + SesionToken);
            }

            return httpClient;
        }

        private HttpClient GetTdVeoHttpClientBySessionToken(string sessionToken)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = this.BaseAddress
            };

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

            // Add authorization header if authorized
            if (IsAuthorised)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "TDAToken " + ApiKey + " " + sessionToken);
            }

            return httpClient;
        }


        /// <summary>
        /// Authorizes a user asynchronously with apikey from the config file
        /// Throws an exception if something went wrong.
        /// </summary>
        public async Task AuthorizeAsync(string login, string password)
        {
            await AuthorizeAsync(login, password, ApiKey);
        }

        /// <summary>
        /// Authorizes a user asynchronously 
        /// Throws an exception if something went wrong.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="apikey"></param>
        public async Task AuthorizeAsync(string login, string password, string apikey)
        {
            using (var httpClient = GetTdVeoHttpClient())
            {
                var body = string.Format(@"<?xml version=""1.0""?>
                                            <AuthenticationRequest xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""  xmlns=""urn:trading.api.institutional.tda.com/Common"">
                                            <apikey>{0}</apikey>
                                            <authscheme>TDA-VEO-CREDENTIALS</authscheme>
                                            <tdauserid>{1}</tdauserid>
                                            <tdapassword>{2}</tdapassword>
                                            <tokenduration>480</tokenduration>
                                            </AuthenticationRequest>", ApiKey = apikey, login, password);

                HttpContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("inputXml", body)
                });


                // POST Authorization request
                var response = await httpClient.PostAsync("sessions/xml", content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(responseContent);

                // Fill the session properties
                SesionToken = xmlResponse.GetElementsByTagName("ns1:SessionKey").Item(0).InnerText;
                TokenExpirationTime =
                    DateTime.Parse(xmlResponse.GetElementsByTagName("ns1:Expires").Item(0).InnerText).ToUniversalTime();
                LogoutAdress = xmlResponse.GetElementsByTagName("ns1:LogoutLink").Item(0).InnerText.Substring(1) + "?method=delete";

            }
        }

        /// <summary>
        /// Deactivates Session Token asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            if (IsAuthorised)
            {
                using (var httpClient = GetTdVeoHttpClient())
                {
                    HttpContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());

                    // POST Logout request
                    var response = await httpClient.PostAsync(LogoutAdress, content);

                    response.EnsureSuccessStatusCode();

                    // Clean the session properties
                    SesionToken = null;
                    TokenExpirationTime = null;
                    LogoutAdress = null;
                }
            }
        }

        /// <summary>
        /// Deactivates Session Token asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync(string sessionToken)
        {
            using (var httpClient = GetTdVeoHttpClient())
            {
                HttpContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());

                // POST Logout request
                var response = await httpClient.PostAsync(BaseAddress + @"sessions/" + sessionToken + @"?method=delete", content);

                response.EnsureSuccessStatusCode();

                // Clean the session properties
                SesionToken = null;
                TokenExpirationTime = null;
                LogoutAdress = null;
            }
        }

        /// <summary>
        /// Gets account balances asynchronously 
        /// </summary>
        /// <param name="repCodes">List of repCodes</param>
        /// <returns>XML string with all requested balances</returns>
        public async Task<string> GetBalancesAsync(IEnumerable<string> repCodes)
        {
            if (IsAuthorised)
            {
                return await GetBalancesAsync(SesionToken, repCodes);
            }
            else
            {
                throw new HttpRequestException("The user is not authorized.");
            }
        }

        public async Task<string> GetBalancesAsync(string sessionToken, IEnumerable<string> repCodes = null, IEnumerable<string> accountGroupIds = null)
        {
            using (var httpClient = GetTdVeoHttpClientBySessionToken(sessionToken))
            {
                StringBuilder repCodesString = new StringBuilder(string.Empty),
                    accountGroupIdsString = new StringBuilder(string.Empty);


                // Add repCodes to the request
                if (repCodes != null)
                {
                    foreach (var repCode in repCodes)
                    {
                        repCodesString.AppendFormat(@"<repCode>{0}</repCode>", repCode);
                    }
                }

                // Add accountGroupIds to the request
                if (accountGroupIds != null)
                {
                    foreach (var accountGroupId in accountGroupIds)
                    {
                        accountGroupIdsString.AppendFormat(@"<accountGroupId>{0}</accountGroupId>", accountGroupId);
                    }
                }

                var body = string.Format(@"<getBalances> 
	                    <accountDescription></accountDescription> 
	                    <accountNumbers> 
		                    <accountNumber></accountNumber> 
	                    </accountNumbers> 
	                    <repCodes> 
		                    {0}
	                    </repCodes> 
	                    <accountGrouplds> 
		                    {1}
	                    </accountGrouplds> 
	                    <previousDayOnlyFlag>false</previousDayOnlyFlag> 
	                    <consolidateByBalancesFlag>false</consolidateByBalancesFlag> 
	                    <responseCriteria>
		                    <sortCriteria> 
			                    <balanceSortAttribute></balanceSortAttribute> 
			                    <sortDirection>ASCENDING</sortDirection> 
		                    </sortCriteria> 
	                    </responseCriteria> 
                    </getBalances>", repCodesString, accountGroupIdsString);


                HttpContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("inputXml", body)
                    });


                // POST the request
                var response = await httpClient.PostAsync("getbalances/xml", content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        public void Authorize(string login, string password)
        {
            AuthorizeAsync(login, password).Wait();
        }

        public void Authorize(string login, string password, string apikey)
        {
            AuthorizeAsync(login, password, apikey).Wait();
        }

        public string AuthorizeAndGetSessionToken(string login, string password)
        {
            AuthorizeAsync(login, password).Wait();
            return SesionToken;
        }

        public void Logout()
        {
            LogoutAsync().Wait();
        }

        public void Logout(string sessionToken)
        {
            LogoutAsync(sessionToken).Wait();
        }

        public string GetBalances(IEnumerable<string> repCodes)
        {
            return GetBalancesAsync(repCodes).Result;
        }

        public string GetBalances(string sessionToken, IEnumerable<string> repCodes)
        {
            return GetBalancesAsync(sessionToken, repCodes).Result;
        }

        public void Dispose()
        {
            Logout();
        }
    }
}
