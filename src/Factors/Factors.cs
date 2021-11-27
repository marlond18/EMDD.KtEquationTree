using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

using System.Collections;

namespace EMDD.KtEquationTree.Factors
{
    public abstract class FactorsBase : IEnumerable<FactorBase>, IEquatable<FactorsBase>
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

    public class Factors : FactorsBase, IEquatable<Factors>
    {
        private readonly IEnumerable<FactorBase> _factors;

        private Factors(IEnumerable<FactorBase> factors)
        {
            _factors = factors;
        }

        public static FactorsBase Create(IEnumerable<FactorBase> factors)
        {
            if (factors.IsEmpty()) return new FactorsOne();
            var (fPN, fPD, fsN, fsD, fZ) = factors.OfType<FactorPowN, FactorPowD, FactorSingleN, FactorSingleD, FactorZero>();
            if (!fZ.IsEmpty()) return new FactorsZero();
            var newN =
                fPN.ToLookup(f => f.B, f => f.E)
                .Concat(fsN.ToLookup(f => f.B, _ => new One()))
                .Concat(fPD.ToLookup(f => f.B, f => f.E))
                .Concat(fsD.ToLookup(f => f.B, _ => new One())).Select(f => FactorPowN.Create(f.Key, f.Sum()));
            if (newN.IsEmpty()) return new FactorsOne();
            return new Factors(newN);
        }

        public override FactorsBase Concat(IEnumerable<FactorBase> factors) => Create(_factors.Concat(factors));

        public override bool Equals(FactorsBase other) => Equals(other as Factors);

        public override IEnumerator<FactorBase> GetEnumerator() => _factors.GetEnumerator();

        public bool Equals(Factors other) => this.TestNullBeforeEquals(other, () => this.ContentEquals(other));

        public override FactorsBase Invert() => Create(this.Select(f => f.Invert()));

        public override Expr ToExpr() => _factors.Select(f => f.ToExpr()).Product();

        public override FactorsBase Raise(Expr exponent) => Create(_factors.Select(f => f.Raise(exponent)));

        public override int GetHashCode() => unchecked(-715237698.ChainHashCode(_factors.GetHashCodeOfEnumerable()));

        public static bool operator ==(Factors factors1, Factors factors2) => factors1.DefaultEquals(factors2);

        public static bool operator !=(Factors factors1, Factors factors2) => !(factors1 == factors2);
    }

    public class FactorsSingle : FactorsBase, IEquatable<FactorsSingle>
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

    public class FactorsOne : FactorsBase, IEquatable<FactorsOne>
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

    public class FactorsZero : FactorsBase, IEquatable<FactorsZero>
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

    public abstract class FactorBase : IEquatable<FactorBase>
    {
        public abstract bool Equals(FactorBase other);

        public abstract FactorBase Invert();

        public abstract Expr ToExpr();

        public abstract FactorBase Raise(Expr exponent);

        public abstract override int GetHashCode();

        public static bool operator ==(FactorBase a, FactorBase b) => a.DefaultEquals(b);

        public static bool operator !=(FactorBase a, FactorBase b) => !(a == b);
    }

    public class FactorPowN : FactorBase, IEquatable<FactorPowN>
    {
        private FactorPowN(Expr b, Expr e)
        {
            B = b;
            E = e;
        }

        public Expr B { get; }
        public Expr E { get; }

        public static FactorBase Create(Expr b, Expr e) => (b, e) switch
        {
            (PowerOp p, _) => Create(p.Left, p.Right * e),
            (Literal l, _) when l.Value == 1 => new FactorOne(),
            (Literal l, _) when l.Value == 0 => new FactorZero(),
            (_, Literal l) when l.Value == 1 => FactorSingleN.Create(b),
            (_, Literal l) when l.Value == 0 => new FactorOne(),
            (_, NegativeOp n) => Create(b, n.Expr).Invert(),
            _ => new FactorPowN(b, e)

        };

        public override bool Equals(FactorBase other) => Equals(other as FactorPowN);

        public bool Equals(FactorPowN other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (this is null) return false;
            if (other is null) return false;
            return B.Equals(other.B) && E.Equals(other.E);
        }

        public override int GetHashCode() => unchecked(-677158451.ChainHashCode(B).ChainHashCode(E));

        public override FactorBase Invert() => FactorPowD.Create(B, E);

        public override FactorBase Raise(Expr exponent) => E switch
        {
            Literal l when l.Value == 1 => Create(B, exponent),
            _ => Create(B, E * exponent)
        };

        public override Expr ToExpr() => E switch
        {
            Literal l when l.Value == 1 => B,
            Literal l when l.Value == 0 => new One(),
            _ => PowerOp.Create(B, E)
        };


        public static bool operator ==(FactorPowN fPN1, FactorPowN fPN2) => fPN1.DefaultEquals(fPN2);

        public static bool operator !=(FactorPowN fPN1, FactorPowN fPN2) => !(fPN1 == fPN2);
    }

    public class FactorPowD : FactorBase, IEquatable<FactorPowD>
    {
        private FactorPowD(Expr b, Expr e)
        {
            B = b;
            E = e;
        }

        public Expr B { get; }
        public Expr E { get; }

        public static FactorBase Create(Expr b, Expr e) => b switch
        {
            PowerOp p => Create(p.Left, p.Right * e),
            _ => e switch
            {
                Literal l when l.Value == 1 => FactorSingleD.Create(b),
                Literal ll when ll.Value == 0 => new FactorOne(),
                NegativeOp n => Create(b, n.Expr).Invert(),
                _ => new FactorPowD(b, e)
            }
        };

        public override bool Equals(FactorBase other) => Equals(other as FactorPowD);

        public bool Equals(FactorPowD other) => this.TestNullBeforeEquals(other, () => other.B.Equals(B) && E.Equals(other.E));

        public override int GetHashCode() => unchecked(677158451.ChainHashCode(B).ChainHashCode(E));

        public override FactorBase Invert() => FactorPowN.Create(B, E);

        public override FactorBase Raise(Expr exponent) => E switch
        {
            Literal l when l.Value == 1 => Create(B, exponent),
            _ => Create(B, E * exponent)
        };

        public override Expr ToExpr() => PowerOp.Create(B, -E);

        public static bool operator ==(FactorPowD fPD1, FactorPowD fPD2) => fPD1.DefaultEquals(fPD2);

        public static bool operator !=(FactorPowD fPD1, FactorPowD fPD2) => !(fPD1 == fPD2);
    }

    public class FactorSingleN : FactorBase, IEquatable<FactorSingleN>
    {
        private FactorSingleN(Expr b)
        {
            B = b;
        }

        public static FactorBase Create(Expr b) => b switch
        {
            Literal l when l.Value == 1 => new FactorOne(),
            Literal s when s.Value == 0 => new FactorZero(),
            _ => new FactorSingleN(b)
        };

        public Expr B { get; }

        public override bool Equals(FactorBase other) => Equals(other as FactorSingleN);

        public override FactorBase Invert() => FactorSingleD.Create(B);

        public override FactorBase Raise(Expr exponent) => FactorPowN.Create(B, exponent);

        public override Expr ToExpr() => B;

        public bool Equals(FactorSingleN other) => this.TestNullBeforeEquals(other, () => B.Equals(other.B));

        public override int GetHashCode() => unchecked(395800909.ChainHashCode(B));

        public static bool operator ==(FactorSingleN n1, FactorSingleN n2) => n1.DefaultEquals(n2);

        public static bool operator !=(FactorSingleN n1, FactorSingleN n2) => !(n1 == n2);
    }

    public class FactorSingleD : FactorBase, IEquatable<FactorSingleD>
    {
        private FactorSingleD(Expr b)
        {
            B = b;
        }

        public Expr B { get; }

        public static FactorBase Create(Expr b) => b switch
        {
            Literal l when l.Value == 1 => new FactorOne(),
            Literal s when s.Value == 0 => new FactorZero(),
            _ => new FactorSingleD(b)
        };

        public override bool Equals(FactorBase other) => Equals(other as FactorSingleD);

        public bool Equals(FactorSingleD other) => this.TestNullBeforeEquals(other, () => B.Equals(other.B));

        public override int GetHashCode() => unchecked(-395800909.ChainHashCode(B));

        public override FactorBase Invert() => FactorSingleN.Create(B);

        public override FactorBase Raise(Expr exponent) => FactorPowD.Create(B, exponent);

        public override Expr ToExpr() => PowerOp.Create(B, -1);

        public static bool operator ==(FactorSingleD d1, FactorSingleD d2) => d1.DefaultEquals(d2);

        public static bool operator !=(FactorSingleD d1, FactorSingleD d2) => !(d1 == d2);
    }

    public class FactorOne : FactorBase, IEquatable<FactorOne>
    {
        public override bool Equals(FactorBase other) => Equals(other as FactorOne);

        public override FactorBase Invert() => new FactorOne();

        public bool Equals(FactorOne other) => this.TestNullBeforeEquals(other, () => true);

        public override Expr ToExpr() => new One();

        public override FactorBase Raise(Expr exponent) => new FactorOne();

        public override int GetHashCode() => 395800930;

        public static bool operator ==(FactorOne d1, FactorOne d2) => d1.DefaultEquals(d2);

        public static bool operator !=(FactorOne d1, FactorOne d2) => !(d1 == d2);
    }

    public class FactorZero : FactorBase, IEquatable<FactorZero>
    {
        public override bool Equals(FactorBase other) => Equals(other as FactorZero);

        public override FactorBase Invert() => new FactorZero();

        public override Expr ToExpr() => new Zero();

        public bool Equals(FactorZero other) => this.TestNullBeforeEquals(other, () => true);

        public override FactorBase Raise(Expr exponent) => new FactorZero();

        public override int GetHashCode() => -395800930;

        public static bool operator ==(FactorZero d1, FactorZero d2) => d1.DefaultEquals(d2);

        public static bool operator !=(FactorZero d1, FactorZero d2) => !(d1 == d2);
    }
}