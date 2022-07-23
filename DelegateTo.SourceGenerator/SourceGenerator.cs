namespace DelegateTo.SourceGenerator;

[Generator]
public class SourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            return;
        var compilation = context.Compilation;

        var fieldsByClass = receiver
            .Fields
            .GroupBy(f => f.ContainingType.ToDisplayString())
            .ToDictionary(f => f.Key);

        foreach (var (container, properties) in fieldsByClass)
        {
            var source = CreateDelegates(compilation, container, properties);
            context.AddSource(container, source);
        }
    }

    private string CreateDelegates(Compilation compilation, string _, IGrouping<string, IPropertySymbol> properties)
    {
        var interfaces = "";
        var container = properties.First().ContainingType;
        var delegates = properties.Select(p => CreateDelegate(compilation, p));
        var access = container.DeclaredAccessibility >= Accessibility.Internal ? container.DeclaredAccessibility.ToString().ToLower() : "";
        var result = @$"
namespace {container.ContainingNamespace.ToDisplayString()}
{{
    {access} partial class {container.Name} {interfaces}
    {{
        {string.Join("\n", delegates)}
    }}
}}
";
        return result;
    }

    private string CreateDelegate(Compilation compilation, IPropertySymbol property)
    {
        var type = property.Type;
        var publicMembers = type.GetMembers()
            .Where(m => m.CanBeReferencedByName && m.DeclaredAccessibility > Accessibility.Internal);

        var methods = publicMembers.Where(m => m is IMethodSymbol).Cast<IMethodSymbol>();
        var properties = publicMembers.Where(m => m is IPropertySymbol).Cast<IPropertySymbol>();

        var methodExpressions = methods.Select(m =>
        {
            var parameters = m.Parameters.Join(", ");
            var parameterNames = m.Parameters.Select(p => p.Name).Join(", ");
            var methodName = m.Name;
            var returnType = m.ReturnType.ToDisplayString();
            return $"public {returnType} {methodName}({parameters}) => {property.Name}.{methodName}({parameterNames});";
        });

        var propertyExpressions = properties.Select(m =>
        {
            var methodName = m.Name;
            var returnType = m.Type.ToDisplayString();
            var getExp = m.GetMethod is null ? "" : $"get => {property.Name}.{methodName}; ";
            var setExp = m.SetMethod is null ? "" : $"set => {property.Name}.{methodName} = value; ";
            return $"public {returnType} {methodName} {{ {getExp} {setExp} }}";
        });

        return methodExpressions
            .Concat(propertyExpressions)
            .Join("\n");
    }

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            // Debugger.Launch();
        }
#endif
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }
}

/// <summary>
/// Created on demand before each generation pass
/// </summary>
class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<IPropertySymbol> Fields { get; } = new();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // any field with at least one attribute is a candidate for property generation
        if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax
            && propertyDeclarationSyntax.AttributeLists.Count > 0)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax);
            if (symbol!.GetAttributes().Any(ad => ad.AttributeClass!.ToDisplayString().Contains("GenerateDelegate")))
            {
                Fields.Add(symbol);
            }
        }
    }
}
