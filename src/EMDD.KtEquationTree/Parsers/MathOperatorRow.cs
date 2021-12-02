using EMDD.KtEquationTree.Exprs.Binary.Additive;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Relations;

using Pidgin.Expression;

using static Pidgin.Parser;

using BinaryFunc = System.Func<EMDD.KtEquationTree.MathStatements.MathStatement, EMDD.KtEquationTree.MathStatements.MathStatement, EMDD.KtEquationTree.MathStatements.MathStatement>;
using InfixOperator = System.Func<Pidgin.Parser<char, System.Func<EMDD.KtEquationTree.MathStatements.MathStatement, EMDD.KtEquationTree.MathStatements.MathStatement, EMDD.KtEquationTree.MathStatements.MathStatement>>, Pidgin.Expression.OperatorTableRow<char, EMDD.KtEquationTree.MathStatements.MathStatement>>;
using OperatorRow = Pidgin.Expression.OperatorTableRow<char, EMDD.KtEquationTree.MathStatements.MathStatement>;
using UnaryFunc = System.Func<EMDD.KtEquationTree.MathStatements.MathStatement, EMDD.KtEquationTree.MathStatements.MathStatement>;

namespace Parser.Methods
{
    internal static class MathOperatorRow
    {
        private static OperatorRow CreateUnaryOperator(char chr, UnaryFunc func) => Operator.Prefix(Char(chr).Between(SkipWhitespaces).ThenReturn(func));

        private static OperatorRow CreateInfixOperator(InfixOperator parser, char chr, BinaryFunc func) => parser(CIChar(chr).Between(SkipWhitespaces).ThenReturn(func));

        internal static readonly OperatorRow Sqrt = CreateUnaryOperator('√', x => SqrtOp.Create(x));
        internal static readonly OperatorRow Complement = CreateUnaryOperator('~', x => ComplementOp.Create(x));
        internal static readonly OperatorRow Negate = CreateUnaryOperator('-', NegativeOp.Create);
        internal static readonly OperatorRow Add = CreateInfixOperator(Operator.InfixL, '+', AddOp.Create);
        internal static readonly OperatorRow Eq = CreateInfixOperator(Operator.InfixL, '=', EqualsOp.Create);
        internal static readonly OperatorRow Subtract = CreateInfixOperator(Operator.InfixL, '-', SubtractOp.Create);
        internal static readonly OperatorRow Exp = CreateInfixOperator(Operator.InfixL, 'e', (l, r) =>
        MultiplyOp.Create(l, PowerOp.Create(10, r)));
        internal static readonly OperatorRow Mult = CreateInfixOperator(Operator.InfixL, '*', MultiplyOp.Create);
        internal static readonly OperatorRow Mult2 = CreateInfixOperator(Operator.InfixL, '×', MultiplyOp.Create);
        internal static readonly OperatorRow Div = CreateInfixOperator(Operator.InfixL, '/', DivideOp.Create);
        internal static readonly OperatorRow Raise = CreateInfixOperator(Operator.InfixR, '^', PowerOp.Create);
    }
}