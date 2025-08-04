using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Sagas.Generators.Infrastructure;

namespace TbdDevelop.Mediator.Sagas.Generators;

[Generator]
public class SagaIHandleEventHandlerGenerator : SagaHandlerEventGenerator, IIncrementalGenerator
{
    protected override string TemplateName => "resources/SagaEventHandler.sbn-cs";
    protected override string HandlerInterfaceName => "IHandle";

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