
using Pidgin;

using static EMDD.KtEquationTree.Parsers.Tokens;

namespace Parser.Methods
{
    internal static class ParenthesisOperator
    {
        private static readonly ParserChar LParen = Tok('(');
        private static readonly ParserChar RParen = Tok(')');
        private static readonly ParserChar LBracket = Tok('[');
        private static readonly ParserChar RBracket = Tok(']');
        private static readonly ParserChar LCBraced = Tok('{');
        private static readonly ParserChar RCBraced = Tok('}');

        internal static Parser<char, T> Parenthesized<T>(Parser<char, T> parser) => parser.Between(LParen, RParen);

        internal static Parser<char, T> Bracketed<T>(Parser<char, T> parser) => parser.Between(LBracket, RBracket);

        internal static Parser<char, T> CurlyBraced<T>(Parser<char, T> parser) => parser.Between(LCBraced, RCBraced);
    }
}