using System;
using System.Numerics;

using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Fractions;

using KtExtensions;

using Single = EMDD.KtEquationTree.Exprs.Singles.Single;

namespace EMDD.KtEquationTree.Exprs.Unary
{
    public abstract class UnaryOp : Expr, IEquatable<Expr>
    {
        public Expr Expr { get; }

        protected UnaryOp(Expr expr)
        {
            Expr = expr ?? throw new ArgumentNullException(nameof(expr));
        }

        protected UnaryOp(BigInteger i) : this(i.ToExpr())
        {
        }

        public override bool Equals(Expr other) =>
            ReferenceEquals(this, other) ? true :
            this is null ? false :
            other is null ? false :
            !IsSimple ? Simplify().Equals(other) :
            other switch
            {
                { IsSimple: false } => Equals(other.Simplify()),
                UnaryOp b => Equals(b),
                _ => false
            };

        protected abstract bool Equals(UnaryOp other);

        protected string ExprToString() => !(Expr is Single) ? Expr.Parenthesize() : Expr.ToString();

        public abstract override int GetHashCode();

        public abstract override string ToString();

        public static bool operator ==(UnaryOp a, UnaryOp b) => a.DefaultEquals(b);

        public static bool operator !=(UnaryOp a, UnaryOp b) => !(a == b);
    }
}