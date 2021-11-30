using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

namespace EMDD.KtEquationTree.Factors
{
    internal class FactorsZero : FactorsBase, IEquatable<FactorsZero>
    {
        public override FactorsBase Concat(IEnumerable<FactorBase> factors) => new FactorsZero();

        public override bool Equals(FactorsBase other) => Equals(other as FactorsZero);

        public override IEnumerator<FactorBase> GetEnumerator()
        {
            yield return new FactorZero();
        }

        public override FactorsBase Invert() => new FactorsZero();

        public bool Equals(FactorsZero other) => this.TestNullBeforeEquals(other, () => true);

        public override Expr ToExpr() => new Zero();

        public override FactorsBase Raise(Expr exponent) => new FactorsZero();

        public override int GetHashCode() => -395800912;

        public static bool operator ==(FactorsZero d1, FactorsZero d2) => d1.DefaultEquals(d2);

        public static bool operator !=(FactorsZero d1, FactorsZero d2) => !(d1 == d2);

    }
}