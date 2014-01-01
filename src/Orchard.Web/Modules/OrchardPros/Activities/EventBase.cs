using Orchard.Localization;
using Orchard.Workflows.Services;

namespace OrchardPros.Activities {
    public abstract class EventBase : Event {
        protected EventBase() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override bool CanStartWorkflow {
            get { return true; }
        }
    }
}