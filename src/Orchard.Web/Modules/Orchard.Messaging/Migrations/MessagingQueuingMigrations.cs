using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Orchard.Messaging.Migrations {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class MessagingQueuingMigrations : DataMigrationImpl {

        public int Create() {
            
            return 1;
        }
    }
}