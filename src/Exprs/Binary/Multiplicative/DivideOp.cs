using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;

using KtExtensions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative
{
    public class DivideOp : MultiplicativeBinaryOp
    {
        protected DivideOp(Expr left, Expr right) : base(left, right)
        {
        }


        public static Expr Create(Expr left, Expr right) => (left, right) switch
        {
            (_, null) => throw new DivideByZeroException($"{left}/0"),
            (_, Zero _) => throw new DivideByZeroException($"{left}/0"),
            (_, One _) => left,
            (null, _) => new Zero(),
            (Zero _, _) => new Zero(),
            _ => new DivideOp(left, right)
        };

        public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);

        public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));

        public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

        protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

        internal override string Op => "/";

        internal override TermsBase Terms() => Left.Terms() / Right.Terms();

        public override Expr Simplify()
        {
            if (IsSimple) return this;
            var v = Left.Simplify() / Right.Simplify();
            v.IsSimple = true;
            return v;
        }

        public override IEnumerable<Expr> Factor() => Left.Factor().Concat(Right.Factor().Select(f => f.Invert()));

        public override Expr Invert() => Left switch
        {
            Literal l when l.Value == 1 => Right,
            NegativeOp { Expr: Literal l } when l.Value == 1 => -Right,
            NegativeOp n => Create(-Right, n.Expr),
            _ => Create(Right, Left)
        };

        public override int GetHashCode() => unchecked(539060727.ChainHashCode(Left).ChainHashCode(Right));

        public override FactorsBase InnerFactor() => IsSimple ? Left.InnerFactor().Concat(Right.InnerFactor().Invert()) : Simplify().InnerFactor();
    }
}