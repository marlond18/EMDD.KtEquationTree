
using EMDD.KtEquationTree.Exprs.Binary.Main;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.MathStatements;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Unary
{
    public class NegativeOne : NegativeOp
    {
        public NegativeOne() : base(new One())
        {
            IsSimple = true;
        }
    }

    public class NegativeOp : UnaryOp
    {
        protected NegativeOp(Expr expr) : base(expr)
        {
        }

        protected NegativeOp(int i) : base(i)
        {
        }

        public static Expr Create(Expr expr) => expr switch
        {
            Zero z => z,
            One _ => new NegativeOne(),
            NegativeOp { Expr: Literal l } => l,
            _ => new NegativeOp(expr)
        };

        public static MathStatement Create(MathStatement ms) => ms switch
        {
            Expr e => Create(e),
            EqualsOp eo => EqualsOp.Create(Create(eo.Left), Create(eo.Right)),
            _ => throw new NotImplementedException()
        };

        public static Expr Create(int i)
        {
            if (i < 0) return Literal.Create(Math.Abs(i));
            if (i == 1) return new NegativeOne();
            return new NegativeOp(Literal.Create(i));
        }

        protected override bool Equals(UnaryOp other) => Equals(other as NegativeOp);

        public override string ToString() => $"-{(Expr is BinaryOp ? $"({Expr})" : $"{Expr}")}";

        private bool Equals(NegativeOp other) => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr));

        internal override TermsBase Terms() => -Expr.Simplify().Terms();

        public override Expr Simplify()
        {
            if (IsSimple) return this;
            var v = -Expr;
            v.IsSimple = true;
            return v;
        }

        public override int GetHashCode() => unchecked(539060725.ChainHashCode(Expr));

        public override IEnumerable<Expr> Factor() => new[] { new NegativeOne() }.Concat(Expr.Factor().WhereNot(e => e == new One()));

        public override Expr Invert() => this.RaiseToNegativeOne();

        internal override FactorsBase InnerFactor() => FactorsSingle.Create(FactorSingleN.Create(new NegativeOne())).Concat(Expr.InnerFactor());

        public override bool TryToDouble(out double value)
        {
            if(Expr.TryToDouble(out double val1))
            {
                value = -val1;
                return true;
            }
            value = 0;
            return false;
        }

        public override Expr Subtitute(Expr current, Expr replacement)
        {
            var newExpr = Expr== current ? replacement : Expr.Subtitute(current, replacement);
            return Create(newExpr);
        }
    }
}