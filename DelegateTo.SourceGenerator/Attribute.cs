using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DelegateTo.SourceGenerator
{
    public class AttributeGenerator
    {
        public const string Name = "GenerateDelegate";
        public const string Namespace = "DelegateTo.SourceGenerator";
        private static bool HasGenerated = false;

        private static readonly string _attributeText = $@"using System;
        namespace {Namespace}
        {{
            [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            public sealed class {Name} : Attribute
            {{
            }}
        }}
                ";

        public static void Generate(GeneratorExecutionContext context) => Generate((name, src) => context.AddSource(name, src));
        public static void Generate(GeneratorPostInitializationContext context) => Generate((name, src) => context.AddSource(name, src));

        public static void Generate(Action<string, string> addSource)
        {
            lock (Name)
            {
                if (HasGenerated)
                    return;
                HasGenerated = true;
            }
            addSource("GenerateMatch_attribute.cs", _attributeText);
        }

        public static (Compilation, INamedTypeSymbol) GetCompilationAndSymbol(GeneratorExecutionContext context)
        {
            if ((context.Compilation as CSharpCompilation)?.SyntaxTrees[0].Options is not CSharpParseOptions options)
            {
                throw new System.Exception("");
            }

            Compilation compilation =
                context.Compilation.AddSyntaxTrees(
                    CSharpSyntaxTree.ParseText(SourceText.From(_attributeText, Encoding.UTF8), options));

            INamedTypeSymbol? attributeSymbol =
                compilation.GetTypeByMetadataName($"{Namespace}.{Name}");

            return (compilation, attributeSymbol!);
        }
    }
}