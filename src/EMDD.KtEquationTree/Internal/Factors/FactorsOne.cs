using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

namespace EMDD.KtEquationTree.Factors
{
    internal class FactorsOne : FactorsBase, IEquatable<FactorsOne>
    {
        public override FactorsBase Concat(IEnumerable<FactorBase> factors) => Factors.Create(factors.Concat(new[] { new FactorOne() }));

        public override IEnumerator<FactorBase> GetEnumerator()
        {
            yield return new FactorOne();
        }

        public bool Equals(FactorsOne other) => this.TestNullBeforeEquals(other, () => true);

        public override bool Equals(FactorsBase other) => Equals(other as FactorsOne);

        public override FactorsBase Invert() => new FactorsOne();

        public override Expr ToExpr() => new One();

        public override FactorsBase Raise(Expr exponent) => new FactorsOne();

        public override int GetHashCode() => 395800912;

        public static bool operator ==(FactorsOne d1, FactorsOne d2) => d1.DefaultEquals(d2);

        public static bool operator !=(FactorsOne d1, FactorsOne d2) => !(d1 == d2);

    }
}