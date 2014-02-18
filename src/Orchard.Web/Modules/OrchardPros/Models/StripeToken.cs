using System;

namespace OrchardPros.Models {
    public class StripeToken {
        public string TokenType { get; set; }
        public string PublishableKey { get; set; }
        public string Scope { get; set; }
        public bool LiveMode { get; set; }
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}