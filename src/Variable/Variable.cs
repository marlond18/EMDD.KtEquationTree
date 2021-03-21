using System;
using System.Collections.Generic;

using EMDD.KtEquationTree.Exprs;

using KtExtensions;

namespace Parser.Expression.Var
{
    internal abstract class Variable : IEquatable<Variable>
    {
        public abstract Expr ToExpr();

        public abstract override string ToString();

        public abstract Variable Raise(Expr c);

        public abstract Variable Invert();

        public abstract IEnumerable<Variable> Factor();

        public abstract (Expr Base, Expr Expo) Pair();

        public abstract bool Equals(Variable other);

        public override bool Equals(object obj) => Equals(obj as Variable);

        public static bool operator ==(Variable a, Variable b) => a.DefaultEquals(b);

        public static bool operator !=(Variable a, Variable b) => !(a == b);

        public abstract override int GetHashCode();
    }
}