﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;

namespace PoExtractor.DotNet
{
    public class DescriptionAttributeStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string DisplayAttributeName = "Description";

        public DescriptionAttributeStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider) : base(metadataProvider)
        {
        }

        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            result = null;

            if (node is AttributeSyntax accessor && accessor.ToFullString().StartsWith(DisplayAttributeName) && accessor.ArgumentList != null)
            {
                var argument = accessor.ArgumentList.Arguments[0];

                if (argument != null && argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    result = new LocalizableStringOccurence()
                    {
                        Text = literal.Token.ValueText,
                        Context = MetadataProvider.GetContext(node),
                        Location = MetadataProvider.GetLocation(node)
                    };

                    return true;
                }
            }

            return false;
        }      
    }
}
