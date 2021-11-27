using Pidgin;
using Pidgin.Expression;

using static Parser.Methods.ParenthesisOperator;
using static EMDD.KtEquationTree.Parsers.Tokens;
using static Pidgin.Parser;
using OperatorRow = Pidgin.Expression.OperatorTableRow<char, EMDD.KtEquationTree.Exprs.Expr>;
using UnaryFunc = System.Func<EMDD.KtEquationTree.Exprs.Expr, EMDD.KtEquationTree.Exprs.Expr>;
using Parser.Methods;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

namespace EMDD.KtEquationTree.Parsers;
public static class ExprParser
{
    private static readonly ParserExpr Identifier = Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t)).Select<Expr>(name => new Identifier(name)).Labelled("identifier");

    private static readonly ParserExpr IdentifierWithS = Tok(TextParser.IndentifierWithSubscript).Labelled("identifier with subscript");

    private static readonly ParserExpr Literal = Tok(NumberParser.Int).Labelled("integer literal");

    private static readonly ParserExpr Float = Tok(NumberParser.Float).Labelled("float value");

    private static UnaryFunc CreateCall(IEnumerable<Expr> args) => method => new Call(method, args.ToArray());

    private static ParserExpr Terms(ParserExpr recursion) => OneOf(
        IdentifierWithS,
        Identifier,
        Float,
        Literal,
        Parenthesized(recursion).Labelled("parenthesized Expression"),
        CurlyBraced(recursion).Labelled("Curly Bracked Expression"),
        Bracketed(recursion).Labelled("Bracketed")
        );

    private static OperatorRow[] FunctionCallOperators(ParserExpr recursion) => new[] { Operator.PostfixChainable(Parenthesized(recursion.Separated(Tok(","))).Select(CreateCall).Labelled("function call")) };

    private static OperatorRow[] FirstLevelBinary() => new[] { MathOperatorRow.Add, MathOperatorRow.Subtract };

    private static OperatorRow[] SecondLevelBinary() => new[] { MathOperatorRow.Raise, MathOperatorRow.Mult, MathOperatorRow.Div, MathOperatorRow.Exp };

    private static OperatorRow[] UnaryOp() => new[] { MathOperatorRow.Negate, MathOperatorRow.Sqrt, MathOperatorRow.Complement };

    private static OperatorRow[] NegativeAndPowerOp() => new[] { MathOperatorRow.Negate, MathOperatorRow.Raise };

    private static ParserExpr BuildExpressionParser => MethodShortcuts.Recursion<ParserExpr>(
        expr => ExpressionParser.Build(
            Terms(Rec(expr)), new[] {
                    FunctionCallOperators(Rec(expr)),
                    NegativeAndPowerOp(),
                    UnaryOp(),
                    SecondLevelBinary(),
                    FirstLevelBinary() }));

    private static readonly ParserExpr Expr = BuildExpressionParser;

    public static Expr ParseOrThrow(string input) => Expr.ParseOrThrow(input);
}