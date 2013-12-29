using System;
using System.Collections.Generic;
using System.Linq;
using NGM.OpenAuthentication.Activities;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Workflows.Models;

namespace OrchardPros.Membership.Activities {
    public class AssignRole : TaskBase {
        private readonly IMembershipService _membershipService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;

        public AssignRole(IMembershipService membershipService, IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) {
            _membershipService = membershipService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
        }

        public const string ActivityName = "AssignRole";
        
        public override string Name {
            get { return ActivityName; }
        }

        public override LocalizedString Category {
            get { return T("Authentication"); }
        }

        public override LocalizedString Description {
            get { return T("Assigns the user to the configured roles using the username value currently stored in the workflow."); }
        }

        public override string Form {
            get { return "SelectRoles"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Done");
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var userName = workflowContext.GetState<string>("UserName");
            var user = _membershipService.GetUser(userName);
            var roleIds = (activityContext.GetState<string>("Roles") ?? "").Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
            var currentUserRoleRecords = _userRolesRepository.Fetch(x => x.UserId == user.Id);
            var currentRoleRecords = currentUserRoleRecords.Select(x => x.Role);
            var targetRoleRecords = roleIds.Select(x => _roleService.GetRole(x));
            foreach (var addingRole in targetRoleRecords.Where(x => !currentRoleRecords.Contains(x))) {
                _userRolesRepository.Create(new UserRolesPartRecord { UserId = user.Id, Role = addingRole });
            }
            yield return T("Done");
        }
    }
}