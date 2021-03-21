using EMDD.KtEquationTree.Exprs.Binary.Main;
using EMDD.KtEquationTree.Exprs.Singles;

using System.Numerics;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative
{
    public abstract class MultiplicativeBinaryOp : BinaryOp
    {
        internal abstract string Op { get; }

        protected MultiplicativeBinaryOp(Expr left, Expr right) : base(left, right)
        {
        }

        protected MultiplicativeBinaryOp(BigInteger i, Expr right) : base(i, right)
        {
        }

        protected MultiplicativeBinaryOp(Expr left, BigInteger i) : base(left, i)
        {
        }

        protected MultiplicativeBinaryOp(BigInteger i, BigInteger j) : base(i, j)
        {
        }

        public override string ToString() => LeftExprToString() + Op + RightExprToString();

        private string LeftExprToString() => !(Left is Single) ? Left.Parenthesize() : Left.ToString();

        protected override bool Equals(BinaryOp other) =>
            ReferenceEquals(this, other) ? true :
            this is null ? false :
            other is null ? false :
            !IsSimple ? Simplify().Equals(other) :
            other switch
            {
                { IsSimple: false } => Equals(other.Simplify()),
                MultiplicativeBinaryOp s => InnerFactor().Equals(s.InnerFactor()),
                _ => false
            };
    }
}