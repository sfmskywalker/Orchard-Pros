using Autofac;
using NUnit.Framework;
using Orchard.Templates.Services;

namespace Orchard.Templates.Tests {
    [TestFixture]
    public class RazorParserTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterType<RazorTemplateParser>().As<ITemplateParser>();
            _container = builder.Build();
        }

        [Test]
        public void ParseSomething() {
            var razorParser = _container.Resolve<ITemplateParser>();
            var result = razorParser.Parse<dynamic>("@{ int i = 42;} The answer to everything is @i.", null);
            Assert.That(result.Trim(), Is.EqualTo("The answer to everything is 42."));
        }
    }
}
