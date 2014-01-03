using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace OrchardPros {
    public class Permissions : IPermissionProvider {
        public static readonly Permission SolveOwnTickets = new Permission { Description = "Solve own tickets", Name = "SolveOwnTickets" };
        public static readonly Permission ManageOwnProfile = new Permission { Description = "Manage own profile", Name = "ManageOwnProfile" };

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
                    Permissions = new[] {SolveOwnTickets, ManageOwnProfile}
                },
                new PermissionStereotype {
                    Name = "Member",
                    Permissions = new[] {SolveOwnTickets, ManageOwnProfile}
                },
            };
        }

    }
}


