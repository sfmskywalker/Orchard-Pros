using System;

namespace Orchard.Messaging.Models {
    public class MessageRecipient {
        public string Name { get; set; }
        public string AddressOrAlias { get; set; }

        public MessageRecipient() {}

        public MessageRecipient(string addressOrAlias) {
            AddressOrAlias = addressOrAlias;
        }

        public MessageRecipient(string addressOrAlias, string name) {
            AddressOrAlias = addressOrAlias;
            Name = name;
        }

        public override string ToString() {
            return !String.IsNullOrWhiteSpace(Name) ? String.Format("{0} <{1}>", Name, AddressOrAlias) : "<" + AddressOrAlias + ">";
        }
    }
}