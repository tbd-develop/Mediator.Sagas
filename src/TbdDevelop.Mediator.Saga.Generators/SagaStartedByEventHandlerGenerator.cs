using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Saga.Generators.Infrastructure;
using TbdDevelop.Mediator.Saga.Generators.Receivers;

namespace TbdDevelop.Mediator.Saga.Generators;

[Generator]
public class SagaStartedByEventHandlerGenerator : SagaHandlerEventGenerator, ISourceGenerator
{
    public override string TemplateName => "resources/SagaStartedByHandler.sbn-cs";
    public override string HandlerInterfaceName => "IAmStartedBy";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new FindSagasWithHandlerSyntaxReceiver(HandlerInterfaceName));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not FindSagasWithHandlerSyntaxReceiver receiver)
            return;

        Generate(context, receiver);
    }
}