using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace TestPlagiarismCA.Shared;

public static class Tokenization
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
        tokens.AddRange(GetMethodBodyTokens(method.Body));

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
            .Where(token => IsTokenValid(token))
            .Select(token => token.Text.Trim())
            .ToList();

        // Extract comments
        tokens.AddRange(body.DescendantTrivia()
            .Where(trivia => IsTriviaValid(trivia))
            .Select(trivia => trivia.ToString().Trim()));

        return tokens;
    }

    private static IEnumerable<string> GetParameterTokens(ParameterSyntax parameter)
    {
        var tokens = parameter.DescendantTokens()
            .Where(token => IsTokenValid(token))
            .Select(token => token.Text.Trim())
            .ToList();

        return tokens;
    }

    private static bool IsTokenValid(SyntaxToken token)
    {
        // Check for operators and punctuation
        if (token.IsKind(SyntaxKind.PlusToken) || token.IsKind(SyntaxKind.MinusToken) ||
            // Include other operators as needed
            token.IsKind(SyntaxKind.OpenBraceToken) || token.IsKind(SyntaxKind.CloseBraceToken) ||
            // Include other punctuation marks as needed
            // Type-related tokens
            token.IsKind(SyntaxKind.IntKeyword) || token.IsKind(SyntaxKind.StringKeyword) ||
            // Include other data types and user-defined types
            // Include other type-specific elements (new, etc.)
            // Control flow keywords
            token.IsKind(SyntaxKind.IfKeyword) || token.IsKind(SyntaxKind.ElseKeyword) ||
            // Include other control flow keywords
            // Method invocation tokens
            token.IsKind(SyntaxKind.IdentifierToken) || token.IsKind(SyntaxKind.OpenParenToken) ||
            token.IsKind(SyntaxKind.CloseParenToken) ||
            // Include tokens related to method invocations and arguments
            // Preprocessor directives for C#
            token.IsKind(SyntaxKind.IfDirectiveTrivia) || token.IsKind(SyntaxKind.ElseDirectiveTrivia) ||
            // Include other preprocessor directives
            // Add more conditions for other language-specific tokens
            // Defaulting to known token types for illustration
            token.IsKeyword() || token.IsKind(SyntaxKind.StringLiteralToken) ||
            token.IsKind(SyntaxKind.NumericLiteralToken))
        {
            return true;
        }
        return false;
    }

    private static bool IsTriviaValid(SyntaxTrivia trivia)
    {
        // Include conditions for comments and preprocessor directives if applicable
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || 
            trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
            // Include other comment types
            // For C# preprocessor directives
            trivia.IsKind(SyntaxKind.IfDirectiveTrivia) || trivia.IsKind(SyntaxKind.EndIfDirectiveTrivia) ||
            // Include other preprocessor directives
            // Add more conditions for other language-specific trivia
            // Defaulting to known trivia types for illustration
            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
        {
            return true;
        }
        return false;
    }

}