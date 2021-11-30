
using Pidgin;

using static Pidgin.Parser;

namespace EMDD.KtEquationTree.Parsers
{
    public static class Tokens
    {
        internal static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).Before(SkipWhitespaces);

        internal static ParserStr Tok(string token) => Tok(String(token));

        internal static ParserChar Tok(char token) => Tok(Char(token));
    }
}