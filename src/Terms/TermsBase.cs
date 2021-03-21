using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using EMDD.KtEquationTree.Exprs;

using KtExtensions;

using Parser.Expression.Var;

namespace Parser.Expression
{

    internal abstract class TermsBase : IEnumerable<TermBase>, IEquatable<TermsBase>
    {
        public abstract IEnumerator<TermBase> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static TermsBase operator +(TermsBase a, TermsBase b) => a.Concat(b);

        public abstract TermsBase Concat(IEnumerable<TermBase> terms);

        public static TermsBase operator -(TermsBase a, TermsBase b) => a + (-b);

        public static TermsBase operator -(TermsBase a) => a.Negate();

        protected abstract TermsBase Negate();

        public static TermsBase operator *(TermsBase a, TermsBase b) => a.Mult(b);

        protected abstract TermsBase Mult(TermsBase b);

        public static TermsBase operator /(TermsBase a, TermsBase b) => a.Div(b);

        public abstract TermsBase Div(TermsBase s);

        public static bool operator ==(TermsBase terms1, TermsBase terms2) => terms1.DefaultEquals(terms2);

        public static bool operator !=(TermsBase terms1, TermsBase terms2) => !(terms1 == terms2);

        public abstract TermsBase Raise(TermsBase b);

        internal static TermsBase Root(Expr @base, BigInteger root) => TermsSingle.Create(TermVariables.Create(VarRootN.Create(@base, root)));

        public abstract Expr ToExpr();

        public override string ToString() => this.BuildString("+");

        public bool Equals(TermsBase other) => this.TestNullBeforeEquals(other, () => this.ContentEquals(other));

        public override int GetHashCode() => unchecked(236811845.ChainHashCode(this.GetHashCodeOfEnumerable()));

        public override bool Equals(object obj) => Equals(obj as TermsBase);
    }
}