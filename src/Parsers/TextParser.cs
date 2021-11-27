using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

using Pidgin;

using static EMDD.KtEquationTree.Parsers.Tokens;
using static Pidgin.Parser;

namespace Parser.Methods;

internal static class TextParser
{
    internal static ParserExpr IndentifierWithSubscript => Try(Map((h1, h2, d, t) => MapToIdenfier(h1, h2, d, t), SingleLetter, LettersOrDigits, U, LettersOrDigits));

    private static Expr MapToIdenfier(char h1, string h2, char d, string t)
    {
        return new Identifier(h1 + h2 + d + t);
    }

    private static readonly ParserChar U = Tok('_');

    private static readonly ParserChar SingleLetter = Letter;

    private static readonly ParserStr LettersOrDigits = LetterOrDigit.ManyString();
}
