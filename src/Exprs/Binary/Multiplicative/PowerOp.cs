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
    public class PowerOp : MultiplicativeBinaryOp
    {
        internal override string Op => "^";

        protected PowerOp(Expr left, Expr right) : base(left, right)
        {
        }

        public static Expr Create(Expr left, Expr right) => (left, right) switch
        {
            (null, null) => throw new InvalidOperationException($"0^0"),
            (_, null) => new One(),
            (null, _) => new Zero(),
            (Zero _, Zero _) => throw new InvalidOperationException($"0^0"),
            (_, Zero _) => new One(),
            (Zero z, _) => z,
            (_, One _) => left,
            (One o, _) => o,
            (DivideOp d, NegativeOp n) => new PowerOp(DivideOp.Create(d.Right, d.Left), n.Expr),
            (_, NegativeOp n) => DivideOp.Create(1, Create(left, n.Expr)),
            _ => new PowerOp(left, right)
        };

        public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);

        public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));

        public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

        protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

        internal override TermsBase Terms() => Left.Simplify().Terms().Raise(Right.Simplify().Terms());

        public override Expr Simplify()
        {
            if (IsSimple) return this;
            var v = Left.Simplify().Raise(Right.Simplify());
            v.IsSimple = true;
            return v;
        }

        public override IEnumerable<Expr> Factor() =>
            Left.Factor().Select(f => f is PowerOp p ? Create(p.Left, p.Right * Right) : Create(f, Right));

        public override Expr Invert()
        {
            var simp = Simplify();
            return simp switch
            {
                PowerOp(Expr left, NegativeOp { Expr: Expr e }) => e switch
                {
                    Literal l when l.Value == 1 => left,
                    _ => Create(left, e)
                },
                PowerOp(Expr l, Expr r) => Create(l, -r),
                _ => simp.Invert()
            };
        }

        public override int GetHashCode() => unchecked(HashCode.Combine(539060729, Left, Right));

        public override FactorsBase InnerFactor() => IsSimple ? Factors.Factors.Create(Left.InnerFactor().Raise(Right)) : Simplify().InnerFactor();
    }
}