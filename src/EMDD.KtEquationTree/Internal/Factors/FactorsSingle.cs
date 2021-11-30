using EMDD.KtEquationTree.Exprs;

namespace EMDD.KtEquationTree.Factors
{
    internal class FactorsSingle : FactorsBase, IEquatable<FactorsSingle>
    {
        private readonly FactorBase _f;

        private FactorsSingle(FactorBase f)
        {
            _f = f;
        }

        public static FactorsBase Create(FactorBase f)
        {
            if (f == null || f is FactorOne) return new FactorsOne();
            if (f == null || f is FactorZero) return new FactorsZero();
            return new FactorsSingle(f);
        }

        public override FactorsBase Concat(IEnumerable<FactorBase> factors) => Factors.Create(factors.Concat(new[] { _f }));

        public override bool Equals(FactorsBase other) => Equals(other as FactorsSingle);

        public override IEnumerator<FactorBase> GetEnumerator()
        {
            yield return _f;
        }

        public bool Equals(FactorsSingle other) => this.TestNullBeforeEquals(other, () => _f.Equals(other._f));

        public override FactorsBase Invert() => Create(_f.Invert());

        public override Expr ToExpr() => _f.ToExpr();

        public override FactorsBase Raise(Expr exponent) => Create(_f.Raise(exponent));

        public override int GetHashCode() => unchecked(-642707624.ChainHashCode(_f));

        public static bool operator ==(FactorsSingle single1, FactorsSingle single2) => single1.DefaultEquals(single2);

        public static bool operator !=(FactorsSingle single1, FactorsSingle single2) => !(single1 == single2);
    }
}