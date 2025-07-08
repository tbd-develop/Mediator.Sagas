using System.Diagnostics;
using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Saga.Generators.Infrastructure;

namespace TbdDevelop.Mediator.Saga.Generators;

[Generator]
public class SagaStartedByEventHandlerGenerator : SagaHandlerEventGenerator, IIncrementalGenerator
{
    public override string TemplateName => "resources/SagaStartedByHandler.sbn-cs";
    public override string HandlerInterfaceName => "IAmStartedBy";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DebugGenerator
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif

        Generate(context);
    }
}