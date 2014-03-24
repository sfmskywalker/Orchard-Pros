using System;
using Orchard.DisplayManagement;
using Orchard.Utility.Extensions;
using OrchardPros.Services.Commerce;

namespace OrchardPros.Services.Commerce {
    public abstract class PayoutProviderBase : IPayoutProvider {
        public virtual string Name {
            get { return GetType().Name.Replace("PayoutProvider", ""); }
        }

        public string DisplayName {
            get { return Name.CamelFriendly(); }
        }
        public IShape BuildDisplay(IShapeFactory shapeFactory) {
            dynamic shape = OnBuildDisplay(shapeFactory);
            shape.Provider = this;
            return shape;
        }

        protected virtual IShape OnBuildDisplay(IShapeFactory shapeFactory) {
            var factory = shapeFactory;
            return factory.Create(String.Format("PayoutProvider__{0}__Connect", Name));
        }
    }
}