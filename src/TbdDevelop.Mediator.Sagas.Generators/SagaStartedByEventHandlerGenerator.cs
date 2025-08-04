using Microsoft.CodeAnalysis;
using TbdDevelop.Mediator.Sagas.Generators.Infrastructure;

namespace TbdDevelop.Mediator.Sagas.Generators;

[Generator]
public class SagaStartedByEventHandlerGenerator : SagaHandlerEventGenerator, IIncrementalGenerator
{
    protected override string TemplateName => "resources/SagaStartedByHandler.sbn-cs";
    protected override string HandlerInterfaceName => "IAmStartedBy";

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