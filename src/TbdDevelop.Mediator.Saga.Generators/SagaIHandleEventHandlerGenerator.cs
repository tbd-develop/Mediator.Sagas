using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Saga.Generators.Infrastructure;
using TbdDevelop.Mediator.Saga.Generators.Receivers;

namespace TbdDevelop.Mediator.Saga.Generators;

[Generator]
public class SagaIHandleEventHandlerGenerator : SagaHandlerEventGenerator, ISourceGenerator
{
    public override string TemplateName => "resources/SagaEventHandler.sbn-cs";
    public override string HandlerInterfaceName => "IHandle";

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