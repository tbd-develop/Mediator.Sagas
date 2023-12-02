using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using TbdDevelop.Mediator.Saga.Generators.Infrastructure;
using TbdDevelop.Mediator.Saga.Generators.Receivers;

namespace TbdDevelop.Mediator.Saga.Generators;

[Generator]
public class SagaIHandleEventHandlerGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new FindSagasWithHandlerSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not FindSagasWithHandlerSyntaxReceiver receiver)
            return;

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
                EmbeddedResource.GetResourceContents("resources/SagaEventHandler.sbn-cs"),
                "resources/SagaEventHandler.sbn-cs");

            foreach (var handlerIdentifierNameSyntax in handlers)
            {
                var notificationName = handlerIdentifierNameSyntax.Identifier.ValueText;
                var className = $"{sagaName}{notificationName}Handler";

                var handlerNameSpace = handlerIdentifierNameSyntax
                    .Ancestors()
                    .OfType<FileScopedNamespaceDeclarationSyntax>()
                    .First();

                var source = template.Render(new
                {
                    Namespace = namespaceDeclaration.Name,
                    Classname = className,
                    Saga = sagaName,
                    Notification = notificationName,
                    Usings = new[] { handlerNameSpace.Name.ToString() }
                });

                context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }
}