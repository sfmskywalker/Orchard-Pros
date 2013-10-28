using System;
using Orchard.DisplayManagement.Implementation;

namespace Orchard.Templates.Services {
    public abstract class TemplateProcessorImpl : ITemplateProcessor {
        public abstract string Type { get; }
        public abstract string Process(string template, DisplayContext context = null, dynamic model = null);
        public virtual void Verify(string template) { }
    }
}