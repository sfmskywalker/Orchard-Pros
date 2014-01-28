using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace OrchardPros {
    public class Permissions : IPermissionProvider {
        public static readonly Permission SolveOwnTickets = new Permission { Description = "Solve own tickets", Name = "SolveOwnTickets" };
        public static readonly Permission ManageOwnProfile = new Permission { Description = "Manage own profile", Name = "ManageOwnProfile" };
        public static readonly Permission WriteRecommendation = new Permission { Description = "Write a recommendation for someone", Name = "WriteRecommendation" };
        public static readonly Permission PublishRecommendation = new Permission { Description = "Approve a recommendation from someone", Name = "PublishRecommendation" };
        public static readonly Permission DeleteRecommendation = new Permission { Description = "Delete a recommendation from someone", Name = "DeleteRecommendation" };
        public static readonly Permission StartBounty = new Permission { Description = "Start a bounty", Name = "StartBounty" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                SolveOwnTickets
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {SolveOwnTickets, ManageOwnProfile, WriteRecommendation, PublishRecommendation, DeleteRecommendation, StartBounty}
                },
                new PermissionStereotype {
                    Name = "Member",
                    Permissions = new[] {SolveOwnTickets, ManageOwnProfile, WriteRecommendation, PublishRecommendation, DeleteRecommendation, StartBounty}
                },
            };
        }
    }
}


