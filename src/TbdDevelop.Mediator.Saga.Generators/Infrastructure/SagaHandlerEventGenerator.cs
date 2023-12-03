using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using TbdDevelop.Mediator.Saga.Generators.Receivers;

namespace TbdDevelop.Mediator.Saga.Generators.Infrastructure;

public abstract class SagaHandlerEventGenerator
{
    public abstract string TemplateName { get; }
    public abstract string HandlerInterfaceName { get; }

    protected void Generate(GeneratorExecutionContext context, FindSagasWithHandlerSyntaxReceiver receiver)
    {
        foreach (var row in receiver.Candidates)
        {
            var sagaClassDeclaration = row.Key;
            var sagaName = sagaClassDeclaration.Identifier.ValueText;
            var handlers = row.Value;

            var namespaceDeclaration = sagaClassDeclaration
                .Ancestors()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .First();

            var template = Template.Parse(
                EmbeddedResource.GetResourceContents(TemplateName), TemplateName);

            foreach (var handlerIdentifierNameSyntax in handlers)
            {
                var notificationName = handlerIdentifierNameSyntax.Identifier.ValueText;
                var className = $"{sagaName}{notificationName}Handler";

                var symbolInfo = context.Compilation.GetSemanticModel(handlerIdentifierNameSyntax.SyntaxTree)
                    .GetSymbolInfo(handlerIdentifierNameSyntax);

                var handlerNameSpace = symbolInfo.Symbol?.ContainingNamespace.ToString();

                var source = template.Render(new
                {
                    Namespace = namespaceDeclaration.Name,
                    Classname = className,
                    Saga = sagaName,
                    Notification = notificationName,
                    Usings = new[] { handlerNameSpace }
                });

                context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }
}