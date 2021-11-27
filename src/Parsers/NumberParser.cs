
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;

using Pidgin;

using static EMDD.KtEquationTree.Parsers.Tokens;
using static Pidgin.Parser;



namespace Parser.Methods
{
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

    internal static class NumberParser
    {
        private static BigInteger ToBigInt(this string b) => BigInteger.Parse(b);

        private static Expr ToFraction(this string b) => DivideOp.Create(b.ToBigInt(), BigInteger.Pow(10, b.Length));

        public static Expr ToDecimal(string h, string d) => Literal.Create(h.ToBigInt()) + d.ToFraction();

        private static readonly ParserStr DigitAtleastOne = Digit.AtLeastOnceString();

        private static readonly ParserStr DigitZeroToMany = Digit.ManyString();

        private static readonly ParserChar Dot = Tok('.');

        private static ParserExpr Decimal => Try(Map((h, d, t) => ToDecimal(h, t), DigitZeroToMany, Dot, DigitAtleastOne));
        
        private static ParserExpr Whole => Try(Map(h => Literal.Create(h.ToBigInt()), DigitAtleastOne));

        private static readonly ParserExpr Base = OneOf(Decimal, Whole);

        public static readonly ParserExpr Float = Decimal;

        public static readonly ParserExpr Int = Whole;
    }
}