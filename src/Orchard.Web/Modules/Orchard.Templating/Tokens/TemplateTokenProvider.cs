using System;
using Orchard.Templating.Services;
using Orchard.Tokens;

namespace Orchard.Templating.Tokens {
    public class TemplateTokenProvider : Component, ITokenProvider {
        private readonly ITemplateService _templateService;

        public TemplateTokenProvider(ITemplateService templateService) {
            _templateService = templateService;
        }

        public void Describe(DescribeContext context) {
            context.For("Template", T("Template"), T("Template tokens."))
                .Token("Execute:*", T("Execute:<shape name>"), T("Executes the specified shape."));
        }

        public void Evaluate(EvaluateContext context) {
            context.For("Template", "")
                .Token(t => t.StartsWith("Execute:", StringComparison.OrdinalIgnoreCase) ? t.Substring("Execute:".Length) : null, TokenValue);
        }

        private object TokenValue(string shapeName, string data) {
            return _templateService.ExecuteShape(shapeName);
        }
    }
}