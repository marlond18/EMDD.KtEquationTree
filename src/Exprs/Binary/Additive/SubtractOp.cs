using System.Collections.Generic;
using System.Numerics;

using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;
using EMDD.KtEquationTree.Factors;

using KtExtensions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Additive
{
    public class SubtractOp : AdditiveBinaryOp
    {
        protected SubtractOp(Expr left, Expr right) : base(left, right)
        {
        }

        public static Expr Create(Expr left, Expr right) => (left, right) switch
        {
            (Zero _, _) => -right,
            (_, Zero _) => left,
            (_, NegativeOp { Expr: Literal l }) => AddOp.Create(left, l),
            _ => new SubtractOp(left, right)
        };

        public static Expr Create(BigInteger left, Expr right) => (left, right) switch
        {
            ({ IsZero: true }, _) => -right,
            (_, Zero _) => Literal.Create(left),
            (_, NegativeOp { Expr: Literal l }) => AddOp.Create(left, l),
            _ => Create(Literal.Create(left), right)
        };

        public static Expr Create(Expr left, BigInteger right) => (left, right) switch
        {
            (Zero _, _) => Literal.Create(-right),
            (_, { IsZero: true }) => left,
            _ => Create(left, Literal.Create(right))
        };

        public static Expr Create(BigInteger left, BigInteger right) => (left, right) switch
        {
            ({ IsZero: true }, _) => Literal.Create(-right),
            (_, { IsZero: true }) => Literal.Create(left),
            _ => Create(Literal.Create(left), Literal.Create(right))
        };

        public override string ToString() => $"{Left}-{RightExprToString()}";

        protected override (Expr left, Expr right) Arrange(Expr a, Expr b) => (a, b);

        internal override TermsBase Terms() => Left.Simplify().Terms() - Right.Simplify().Terms();

        public override Expr Simplify()
        {
            if (IsSimple) return this;
            var v = Left.Simplify() - Right.Simplify();
            v.IsSimple = true;
            return v;
        }

        public override IEnumerable<Expr> Factor() => MethodShortcuts.SeedProcess(
            () => Left.Factor().Venn(Right.Factor()),
            d => d.AandB.Concat(new[] { d.OnlyA.Product() - d.OnlyB.Product() }));

        public override int GetHashCode() => unchecked(539060730.ChainHashCode(Left).ChainHashCode(Right));

        public override FactorsBase InnerFactor()
        {
            return CreateFactor(Left.InnerFactor().Venn(Right.InnerFactor()));
            FactorsBase CreateFactor((FactorsBase OnlyA, FactorsBase AandB, FactorsBase OnlyB) venn) => venn.AandB.Concat(FactorsSingle.Create(FactorSingleN.Create(venn.OnlyA.ToExpr() - venn.OnlyB.ToExpr())));
        }
    }
}