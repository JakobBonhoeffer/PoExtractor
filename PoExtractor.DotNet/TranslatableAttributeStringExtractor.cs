using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace PoExtractor.DotNet
{
    public class TranslatableAttributeStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string TranslatableAttributeName = "Translatable";
        private List<string> acceptedModifiers = new() { "const" };


        public TranslatableAttributeStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider)
        {
        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is LiteralExpressionSyntax literalExpression && 
                (literalExpression.Ancestors().Where(a => a is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.ToFullString().Contains(TranslatableAttributeName)).Any() &&
                 literalExpression.Ancestors().Where(a => a is FieldDeclarationSyntax fieldDeclarationSyntax && fieldDeclarationSyntax.Modifiers.Any(m => acceptedModifiers.Contains(m.ValueText))).Any()))
            {
                result = new LocalizableStringOccurence()
                {
                    Text = literalExpression.Token.ValueText,
                    Context = MetadataProvider.GetContext(literalExpression),
                    Location = MetadataProvider.GetLocation(literalExpression)
                };

                return true;
            }

            return false;
        }
    }
}
