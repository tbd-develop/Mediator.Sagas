using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace TbdDevelop.Mediator.Saga.Generators.Infrastructure;

public abstract class SagaHandlerEventGenerator
{
    public abstract string TemplateName { get; }
    public abstract string HandlerInterfaceName { get; }

    protected void Generate(IncrementalGeneratorInitializationContext context)
    {
        var sagaCandidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => IsEntityClass(node),
                transform: (ctx, _) => GetEntityInfo(ctx));

        var candidateCompilation = context.CompilationProvider.Combine(sagaCandidates.Collect());

        context.RegisterSourceOutput(candidateCompilation, GenerateEntityCode);
    }

    private bool IsEntityClass(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDeclaration)
        {
            return false;
        }

        if (classDeclaration.BaseList == null)
        {
            return false;
        }

        var types = classDeclaration
            .BaseList
            .Types;

        var isSaga = types.Any(t => t.Type is GenericNameSyntax
        {
            Identifier.ValueText: "Saga"
        });

        var containsHandlerInterface = types
            .Select(t => t.Type)
            .OfType<GenericNameSyntax>()
            .Any(t => t.Identifier.ValueText == HandlerInterfaceName);

        return isSaga && containsHandlerInterface;
    }

    private EntityInfo? GetEntityInfo(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax declaration)
        {
            return null;
        }

        var handlerIdentifiers = declaration.BaseList!.Types
            .Select(bt => bt.Type)
            .OfType<GenericNameSyntax>()
            .Where(genericName =>
                genericName.Identifier.ValueText != "Saga" &&
                genericName.Identifier.ValueText == HandlerInterfaceName &&
                genericName.TypeArgumentList.Arguments.Count == 1 &&
                genericName.TypeArgumentList.Arguments[0] is IdentifierNameSyntax)
            .Select(genericName => genericName.TypeArgumentList.Arguments[0] as IdentifierNameSyntax)
            .Where(id => id != null)
            .ToList();

        return handlerIdentifiers.Any()
            ? new EntityInfo(declaration, handlerIdentifiers)
            : null;
    }

    private void GenerateEntityCode(
        SourceProductionContext productionContext,
        (Compilation, ImmutableArray<EntityInfo?>) source)
    {
        var (compilation, candidates) = source;

        foreach (var candidate in candidates)
        {
            var sagaClassDeclaration = candidate!.Declaration;
            var sagaName = sagaClassDeclaration.Identifier.ValueText;
            var handlers = candidate.HandlerIdentifiers;

            var namespaceDeclaration = sagaClassDeclaration
                .Ancestors()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .First();

            var template = Template.Parse(
                EmbeddedResource.GetResourceContents(TemplateName), TemplateName);

            var model = compilation.GetSemanticModel(sagaClassDeclaration.SyntaxTree);

            foreach (var handlerIdentifierNameSyntax in handlers.Where(x => x is not null))
            {
                var notificationName = handlerIdentifierNameSyntax!.Identifier.ValueText;
                var className = $"{sagaName}{notificationName}Handler";
                var handlerNamespace = model.GetDeclaredSymbol(sagaClassDeclaration)
                    ?.ContainingNamespace.ToString() ?? string.Empty;

                var sourceText = template.Render(new
                {
                    Namespace = namespaceDeclaration.Name,
                    Classname = className,
                    Saga = sagaName,
                    Notification = notificationName,
                    Usings = new[] { handlerNamespace },
                });

                productionContext.AddSource($"{className}.cs", SourceText.From(sourceText, Encoding.UTF8));
            }
        }
    }

    private sealed class EntityInfo(ClassDeclarationSyntax declaration, IEnumerable<IdentifierNameSyntax> identifiers)
    {
        public ClassDeclarationSyntax Declaration { get; } = declaration;
        public IEnumerable<IdentifierNameSyntax> HandlerIdentifiers { get; } = identifiers;
    }
}