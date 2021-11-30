
using EMDD.KtEquationTree.Exprs;

namespace Parser.Expression
{
    internal abstract class TermBase : IEquatable<TermBase>
    {
        public static TermBase operator -(TermBase a) => a switch
        {
            null => null,
            _ => a.Negate()
        };

        public abstract TermBase Negate();

        public static TermBase operator *(TermBase a, TermBase b) => (a, b) switch
        {
            _ when a is null || b is null => null,
            _ => a.Mult(b)
        };

        public abstract TermBase Mult(TermBase b);

        public static TermBase operator /(TermBase a, TermBase b) => (a, b) switch
        {
            (null, _) => null,
            (_, null) => throw new NullReferenceException(nameof(b)),
            _ => a.Div(b)
        };

        public abstract TermBase Div(TermBase b);

        public static bool operator ==(TermBase a, TermBase b) => a.DefaultEquals(b);

        public static bool operator !=(TermBase a, TermBase b) => !(a == b);

        public abstract Expr ToExpr();

        public abstract TermBase Raise(TermBase power);

        public abstract override string ToString();

        public abstract bool Equals(TermBase other);

        public override bool Equals(object obj) => Equals(obj as TermBase);

        public abstract override int GetHashCode();
    }
}