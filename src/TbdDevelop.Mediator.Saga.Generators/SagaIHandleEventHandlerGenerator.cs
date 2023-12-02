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
            var handlers = row.Value;

            foreach (var handler in handlers)
            {
                // get namespace of saga.Handlers

                // get handlername

                // build TenantCreatedSagaTenantCreatedEventHandler : INotificationHandler<TenantCreatedEvent>

                // Load Saga (class)

                // if does not exist, move on! 

                // update saga with event 

                // save saga

                // var namespaceDeclaration = classDeclaration
                //     .Ancestors()
                //     .OfType<NamespaceDeclarationSyntax>()
                //     .First();
                //
                // var className = $"{classDeclaration.Identifier.ValueText}EventHandler";
                //
                // var template = Template.Parse(
                //     EmbeddedResource.GetResourceContents("resources/SagaIHandleEventHandler.sbn-cs"),
                //     "resources/SagaIHandleEventHandler.sbn-cs");
                //
                // var source = template.Render(new
                // {
                //     Namespace = namespaceDeclaration.Name,
                //     Classname = className,
                //     Saga = classDeclaration.Identifier.ValueText,
                //     Events = identifierNames.Select(i => i.Identifier.ValueText)
                // });
                //
                // context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }
}