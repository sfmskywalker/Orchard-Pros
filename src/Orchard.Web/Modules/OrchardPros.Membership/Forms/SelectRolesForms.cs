using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Roles.Services;

namespace OrchardPros.Membership.Forms {
    public class SelectRolesForms : Component, IFormProvider {
        private readonly IRoleService _roleService;
        protected dynamic Shape { get; set; }

        public SelectRolesForms(
            IShapeFactory shapeFactory,
            IRoleService roleService) {

            _roleService = roleService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, dynamic> form =
                shape => {

                    var f = Shape.Form(
                        Id: "Roles",
                        _Roles: Shape.SelectList(
                            Id: "roles", Name: "Roles",
                            Title: T("Roles"),
                            Description: T("Select one or more roles."),
                            Size: 10,
                            Multiple: true));

                    foreach (var role in _roleService.GetRoles().OrderBy(x => x.Name)) {
                        f._Roles.Add(new SelectListItem { Value = role.Id.ToString(), Text = role.Name });
                    }

                    return f;
                };

            context.Form("SelectRoles", form);

        }
    }
}