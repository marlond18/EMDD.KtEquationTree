﻿using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.MathStatements;

using Pidgin;

using static EMDD.KtEquationTree.Parsers.Tokens;
using static Pidgin.Parser;

namespace Parser.Methods;
internal static class NumberParser
{
    private static BigInteger ToBigInt(this string b) => BigInteger.Parse(b);

    private static MathStatement ToFraction(this string b) => DivideOp.Create(b.ToBigInt(), BigInteger.Pow(10, b.Length));

    public static MathStatement ToDecimal(string h, string d) => Literal.Create(h.ToBigInt()) + d.ToFraction();

    private static readonly ParserStr DigitAtleastOne = Digit.AtLeastOnceString();

    private static readonly ParserStr DigitZeroToMany = Digit.ManyString();

    private static readonly ParserChar Dot = Tok('.');

    private static ParserExpr Decimal => Try(Map((h, d, t) => ToDecimal(h, t), DigitZeroToMany, Dot, DigitAtleastOne));

    private static ParserExpr Whole => Try(Map(h => (MathStatement)Literal.Create(h.ToBigInt()), DigitAtleastOne));

    private static readonly ParserExpr Base = OneOf(Decimal, Whole);

    public static readonly ParserExpr Float = Decimal;

    public static readonly ParserExpr Int = Whole;
}
