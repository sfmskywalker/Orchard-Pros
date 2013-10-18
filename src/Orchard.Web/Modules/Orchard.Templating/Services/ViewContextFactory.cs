using System.Web.Mvc;

namespace Orchard.Templating.Services {
    public interface IViewContextFactory : IDependency {
        ViewContext Create();
    }

    public class ViewContextFactory : IViewContextFactory {
        public ViewContext Create() {
            return new ViewContext { };
        }
    }
}