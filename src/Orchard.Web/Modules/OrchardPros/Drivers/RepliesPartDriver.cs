using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Models;

namespace OrchardPros.Drivers {
    public class RepliesPartDriver : ContentPartDriver<RepliesPart> {
        private readonly IContentManager _contentManager;

        public RepliesPartDriver(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        protected override DriverResult Display(RepliesPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Replies", () => {
                var replies = part.Replies.Select(x => _contentManager.BuildDisplay(x, "Summary")).ToArray();
                return shapeHelper.Parts_Replies(Replies: replies);
            });
        }
    }
}