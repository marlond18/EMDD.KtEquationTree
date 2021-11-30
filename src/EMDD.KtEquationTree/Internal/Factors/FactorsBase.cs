using EMDD.KtEquationTree.Exprs;

using System.Collections;

namespace EMDD.KtEquationTree.Factors
{
    internal abstract class FactorsBase : IEnumerable<FactorBase>, IEquatable<FactorsBase>
    {
        public abstract IEnumerator<FactorBase> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract FactorsBase Concat(IEnumerable<FactorBase> factors);

        public abstract bool Equals(FactorsBase other);

        public abstract FactorsBase Invert();

        public abstract Expr ToExpr();

        public (FactorsBase OnlyA, FactorsBase AandB, FactorsBase OnlyB) Venn(FactorsBase b)
        {
            var (onlyA, aAndB, onlyB) = this.Venn<FactorBase>(b);
            return (Factors.Create(onlyA), Factors.Create(aAndB), Factors.Create(onlyB));
        }

        public abstract FactorsBase Raise(Expr exponent);

        public abstract override int GetHashCode();

        public static bool operator ==(FactorsBase factors1, FactorsBase factors2) => factors1.DefaultEquals(factors2);

        public static bool operator !=(FactorsBase factors1, FactorsBase factors2) => !(factors1 == factors2);
    }
}