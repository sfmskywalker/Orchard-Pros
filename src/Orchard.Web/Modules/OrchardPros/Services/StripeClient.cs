using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class StripeClient : IStripeClient, IDisposable {
        private readonly IStripeClientConfigurationAccessor _configAccessor;
        private readonly Lazy<HttpClient> _httpClient;

        public StripeClient(IStripeClientConfigurationAccessor configAccessor) {
            _configAccessor = configAccessor;
            _httpClient = new Lazy<HttpClient>(() => {
                var config = _configAccessor.GetConfiguration();
                var httpClient = new HttpClient {
                    BaseAddress = new Uri(config.EndpointUrl)
                };
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:", config.SecretKey))));
                return httpClient;
            });
        }

        private HttpClient HttpClient {
            get { return _httpClient.Value; }
        }

        public StripeCharge CreateCharge(int amount, string currency, string card, string description) {
            var result = HttpClient.PostAsync("v1/charges", new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"amount", amount.ToString(CultureInfo.InvariantCulture)},
                    {"currency", currency},
                    {"card", card},
                    {"description", description}
                })).Result;

            result.EnsureSuccessStatusCode();
            var content = result.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(content);
            return new StripeCharge {
                Id = json.Value<string>("id"),
                Amount = json.Value<int>("amount"),
                CreatedUtc = new DateTime(json.Value<long>("created")),
                BalanceTransaction = json.Value<string>("balance_transaction"),
                Currency = json.Value<string>("currency"),
                FailureCode = json.Value<string>("failure_code"),
                FailureMessage = json.Value<string>("failure_message"),
                LiveMode = json.Value<bool>("livemode"),
                Paid = json.Value<bool>("paid"),
                Refunded = json.Value<bool>("refunded"),
            };
        }

        void IDisposable.Dispose() {
            if (_httpClient.IsValueCreated) {
                _httpClient.Value.Dispose();
            }
        }
    }
}