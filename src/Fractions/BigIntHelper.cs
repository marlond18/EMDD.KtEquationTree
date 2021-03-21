using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using EMDD.KtEquationTree.Constant;
using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using KtExtensions;

using Parser.Expression;
using Parser.Expression.Var;

namespace EMDD.KtEquationTree.Fractions
{
    public static class BigIntHelper
    {
        public static Expr ToExpr(this BigInteger i)
        {
            if (i < 0) return NegativeOp.Create(Literal.Create(BigInteger.Abs(i)));
            return Literal.Create(i);
        }

        public static Fraction Raise(this BigInteger a, BigInteger b)
        {
            if (a == 1 || b == 0) return (1, 1);
            if (a == 0) return (0, 1);
            if (b == 1) return (a, 1);
            if (BigInteger.Abs(b) > int.MaxValue) throw new OverflowException($"cannot do this {a}^{b}");
            if (b < 0) return (1, BigInteger.Pow(a, (int)BigInteger.Abs(b)));
            return (BigInteger.Pow(a, (int)b), 1);
        }

        public static IEnumerable<(BigInteger b, Fraction e)> Raise(this BigInteger b, Fraction e) =>
            b.Factor().ToLookup(ff => ff).Select(g => (b: g.Key, e: g.Count())).Select(f => (f.b, e: new Fraction(f.e * e.num, e.den).Reduce()));

        public static bool ExceedsIntMax(this BigInteger i) => BigInteger.Abs(i) > int.MaxValue;

        internal static TermBase ConvertBigIntRaiseToTerm(this IEnumerable<(BigInteger b, Fraction e)> r) =>
            CreateFromFork(r.Fork(f => f.e.den == 1));

        private static TermBase CreateFromFork((IEnumerable<(BigInteger b, Fraction e)> matches, IEnumerable<(BigInteger b, Fraction e)> nonMatches) newF)
        {
            var constant = newF.matches.Aggregate(ConstantWhole.Create(1), (total, f) => total * ConstantFraction.Create(f.b.Raise(f.e.num)));
            var variables = Variables.Create(newF.nonMatches.WhereNot(f => f.b == 1).Select(f => VarRootN.Create(f.b.Raise(f.e.num).ToExprDiv(), f.e.den)));
            return Term.Create(constant, variables);
        }
    }
}