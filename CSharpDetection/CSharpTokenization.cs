using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace TestPlagiarismCA.CSharpDetection;

public static class CSharpTokenization
{
    internal static Dictionary<string, List<string>> TokenizeMethods(string content)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(content);
        CompilationUnitSyntax? root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        Dictionary<string, List<string>> methodTokens = new Dictionary<string, List<string>>();

        if (root?.DescendantNodes().OfType<MethodDeclarationSyntax>() is { } methodDeclarations)
        {
            foreach (MethodDeclarationSyntax method in methodDeclarations)
            {
                List<string> methodTokenList = TokenizeMethod(method);
                methodTokens.Add(method.Identifier.Text, methodTokenList);
            }
        }

        return methodTokens;
    }

    private static List<string> TokenizeMethod(MethodDeclarationSyntax method)
    {
        List<string> tokens = new List<string>();

        // Extract method body tokens (identifiers, keywords, literals)
        if (method.Body != null) tokens.AddRange(GetMethodBodyTokens(method.Body));

        // Extract method parameter tokens
        foreach (var parameter in method.ParameterList.Parameters)
        {
            tokens.AddRange(GetParameterTokens(parameter));
        }

        return tokens;
    }

    private static IEnumerable<string> GetMethodBodyTokens(SyntaxNode body)
    {
        var tokens = body.DescendantTokens()
            .Where(IsTokenValid)
            .Select(token => token.Text.Trim())
            .ToList();

        // Extract comments
        tokens.AddRange(body.DescendantTrivia()
            .Where(IsTriviaValid)
            .Select(trivia => trivia.ToString().Trim()));

        return tokens;
    }

    private static IEnumerable<string> GetParameterTokens(ParameterSyntax parameter)
    {
        var tokens = parameter.DescendantTokens()
            .Where(IsTokenValid)
            .Select(token => token.Text.Trim())
            .ToList();

        return tokens;
    }

    private static bool IsTokenValid(SyntaxToken token)
    {
        return token.IsKind(SyntaxKind.PlusToken) || token.IsKind(SyntaxKind.MinusToken) ||
               token.IsKind(SyntaxKind.OpenBraceToken) || token.IsKind(SyntaxKind.CloseBraceToken) ||
               token.IsKind(SyntaxKind.IntKeyword) || token.IsKind(SyntaxKind.StringKeyword) ||
               token.IsKind(SyntaxKind.IfKeyword) || token.IsKind(SyntaxKind.ElseKeyword) ||
               token.IsKind(SyntaxKind.IdentifierToken) || token.IsKind(SyntaxKind.OpenParenToken) ||
               token.IsKind(SyntaxKind.CloseParenToken) ||
               token.IsKind(SyntaxKind.IfDirectiveTrivia) || token.IsKind(SyntaxKind.ElseDirectiveTrivia) ||
               token.IsKeyword() || token.IsKind(SyntaxKind.StringLiteralToken) ||
               token.IsKind(SyntaxKind.NumericLiteralToken);
    }

    private static bool IsTriviaValid(SyntaxTrivia trivia)
    {
        // Include conditions for comments and preprocessor directives if applicable
        return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || 
               trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
               trivia.IsKind(SyntaxKind.IfDirectiveTrivia) || trivia.IsKind(SyntaxKind.EndIfDirectiveTrivia) ||
               trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia);
    }
}