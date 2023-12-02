using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TbdDevelop.Mediator.Saga.Generators.Receivers;

public class FindSagasWithHandlerSyntaxReceiver : ISyntaxReceiver
{
    private readonly string _sagaHandlerInterfaceName;
    public Dictionary<ClassDeclarationSyntax, List<IdentifierNameSyntax>> Candidates { get; } = new();

    public FindSagasWithHandlerSyntaxReceiver(string sagaHandlerInterfaceName)
    {
        _sagaHandlerInterfaceName = sagaHandlerInterfaceName;
    }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        if (classDeclaration.BaseList == null) return;
        if (!classDeclaration
                .BaseList
                .Types
                .Any(t => t.Type is GenericNameSyntax { Identifier.ValueText: "Saga" }))
            return;

        foreach (var baseType in classDeclaration.BaseList.Types)
        {
            if (baseType.Type is not GenericNameSyntax genericName) continue;
            if (genericName.Identifier.ValueText == "Saga") continue;
            if (genericName.Identifier.ValueText != _sagaHandlerInterfaceName) continue;
            if (genericName.TypeArgumentList.Arguments.Count != 1) continue;
            if (genericName.TypeArgumentList.Arguments[0] is not IdentifierNameSyntax identifierName) continue;

            if (Candidates.TryGetValue(classDeclaration, out var candidate))
            {
                candidate.Add(identifierName);
            }
            else
            {
                Candidates.Add(classDeclaration, new List<IdentifierNameSyntax> { identifierName });
            }
        }
    }
}