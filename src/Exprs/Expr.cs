using System;
using System.Collections.Generic;

using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Factors;

using KtExtensions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs
{
    public abstract class Expr : IEquatable<Expr>
    {
        protected Expr()
        {
        }

        public abstract bool Equals(Expr other);

        public static Expr operator -(Expr a) => a == null ? null : (-a.Terms()).ToExpr();

        public static Expr operator -(Expr a, Expr b) => (a.Terms() - b.Terms()).ToExpr();

        public static Expr operator *(Expr a, Expr b) => (a.Terms() * b.Terms()).ToExpr();

        public static Expr operator +(Expr a, Expr b) => (a.Terms() + b.Terms()).ToExpr();

        public static Expr operator /(Expr a, Expr b) => (a.Terms() / b.Terms()).ToExpr();

        public abstract Expr Simplify();

        public abstract IEnumerable<Expr> Factor();

        public abstract FactorsBase InnerFactor();

        public Expr Raise(Expr expr) => Terms().Raise(expr.Terms()).ToExpr();

        public Expr Sqrt() => Raise(DivideOp.Create(1, 2));

        internal abstract TermsBase Terms();

        public static bool operator ==(Expr a, Expr b) => a.DefaultEquals(b);

        public static bool operator !=(Expr a, Expr b) => !(a == b);

        public abstract Expr Invert();

        /// <summary>
        /// Try to Convert Expression to Decimal Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryToDouble(out double value);

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) ||
            this is null ? false :
            obj is null ? false :
            (IsSimple ? this : Simplify()).Equals(obj as Expr);

        public abstract override int GetHashCode();

        public bool IsSimple { get; internal set; }
    }
}