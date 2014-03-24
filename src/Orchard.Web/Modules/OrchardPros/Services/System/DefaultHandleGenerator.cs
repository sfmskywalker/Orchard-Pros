using System;
using System.Security.Cryptography;
using System.Text;

namespace OrchardPros.Services.System {
    public class DefaultHandleGenerator : IHandleGenerator {
        public string Generate() {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[4];
            var sb = new StringBuilder(32);

            while (sb.Length < 16) {
                rng.GetBytes(buffer);
                var number = Math.Abs(BitConverter.ToInt32(buffer, 0));
                sb.Append(number);
            }
            return sb.ToString();
        }
    }
}