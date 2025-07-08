using System.Diagnostics;
using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Saga.Generators.Infrastructure;

namespace TbdDevelop.Mediator.Saga.Generators;

[Generator]
public class SagaIHandleEventHandlerGenerator : SagaHandlerEventGenerator, IIncrementalGenerator
{
    public override string TemplateName => "resources/SagaEventHandler.sbn-cs";
    public override string HandlerInterfaceName => "IHandle";

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