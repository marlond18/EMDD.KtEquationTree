using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Exprs.Unary;

namespace EMDD.KtEquationTree.Factors
{
    internal class Factors : FactorsBase, IEquatable<Factors>
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

    internal abstract class FactorBase : IEquatable<FactorBase>
    {
        public abstract bool Equals(FactorBase other);

        public abstract FactorBase Invert();

        public abstract Expr ToExpr();

        public abstract FactorBase Raise(Expr exponent);

        public abstract override int GetHashCode();

        public static bool operator ==(FactorBase a, FactorBase b) => a.DefaultEquals(b);

        public static bool operator !=(FactorBase a, FactorBase b) => !(a == b);
    }

    internal class FactorPowN : FactorBase, IEquatable<FactorPowN>
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

    internal class FactorPowD : FactorBase, IEquatable<FactorPowD>
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

    internal class FactorSingleN : FactorBase, IEquatable<FactorSingleN>
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

    internal class FactorSingleD : FactorBase, IEquatable<FactorSingleD>
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

    internal class FactorOne : FactorBase, IEquatable<FactorOne>
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

    internal class FactorZero : FactorBase, IEquatable<FactorZero>
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