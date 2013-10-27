﻿using Autofac;
using NUnit.Framework;
using Orchard.Templates.Parsers;
using Orchard.Templates.Services;

namespace Orchard.Templates.Tests {
    [TestFixture]
    public class RazorParserTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterType<RazorParser>().As<IRazorParser>();
            _container = builder.Build();
        }

        [Test]
        public void ParseSomething() {
            var razorParser = _container.Resolve<IRazorParser>();
            var result = razorParser.Parse<dynamic>("@{ int i = 42;} The answer to everything is @i.");
            Assert.That(result.Trim(), Is.EqualTo("The answer to everything is 42."));
        }
    }
}
