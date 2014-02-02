using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class StripeClient : IStripeClient {
        public StripeCharge CreateCharge(string secretKey, int amount, string currency, string card, string description) {
            using (var httpClient = new HttpClient()) {
                httpClient.BaseAddress = new Uri("https://api.stripe.com/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", secretKey + ":");
                var result = httpClient.PostAsync("v1/charges", new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"amount", amount.ToString()},
                    {"currency", currency},
                    {"card", card},
                    {"description", description}
                })).Result;

                result.EnsureSuccessStatusCode();
                return new StripeCharge();
            }
        }
    }
}