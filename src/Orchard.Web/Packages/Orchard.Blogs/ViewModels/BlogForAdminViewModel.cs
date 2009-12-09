using Orchard.Blogs.Models;
using Orchard.Models.ViewModels;
using Orchard.Mvc.ViewModels;

namespace Orchard.Blogs.ViewModels {
    public class BlogForAdminViewModel : AdminViewModel {
        public ItemDisplayViewModel<Blog> Blog { get; set; }
    }
}