using System;
using Orchard.Templates.Services;
using Orchard.Tokens;

namespace Orchard.Templates.Tokens {
    public class ShapeTokenProvider : Component, ITokenProvider {
        private readonly ITemplateService _templateService;

        public ShapeTokenProvider(ITemplateService templateService) {
            _templateService = templateService;
        }

        public void Describe(DescribeContext context) {
            context.For("Shape", T("Shape"), T("Shape tokens."))
                .Token("Execute:*", T("Execute:<shape name>"), T("Executes the specified shape."));
        }

        public void Evaluate(EvaluateContext context) {
            context.For("Shape", "")
                .Token(t => t.StartsWith("Execute:", StringComparison.OrdinalIgnoreCase) ? t.Substring("Execute:".Length) : null, TokenValue);
        }

        private object TokenValue(string shapeName, string data) {
            return _templateService.ExecuteShape(shapeName);
        }
    }
}