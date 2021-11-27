using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Fractions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Constant;
internal class ConstantFraction : Constant, IEquatable<ConstantFraction>
{
    protected ConstantFraction(Fraction value)
    {
        Value = value;
    }

    public Fraction Value { get; }

    public static Constant Create(Fraction value)
    {
        var val = value.Reduce();
        if (val.den == 1) return ConstantWhole.Create(val.num);
        return new ConstantFraction(val);
    }

    public static Constant Create((BigInteger numerator, BigInteger denominator) value) =>
        Create(value);

    public override TermBase Raise(Constant coefficient) => coefficient switch
    {
        ConstantZero _ => new TermOne(),
        ConstantOne _ => TermConstant.Create(this),
        ConstantWhole w => TermConstant.Create(Create(Value.Raise(w.Value))),
        ConstantFraction f => Value.num.Raise(f.Value).ConvertBigIntRaiseToTerm() / Value.den.Raise(f.Value).ConvertBigIntRaiseToTerm(),
        _ => throw new InvalidOperationException($"Cannot determine type of {coefficient}")
    };

    public override string ToString() => Value.ToString();

    protected override Constant Mult(Constant b) => b switch
    {
        ConstantZero _ => new ConstantZero(),
        ConstantOne _ => this,
        ConstantWhole w => Create(Value * w.Value),
        ConstantFraction f => Create(Value * f.Value),
        _ => throw new InvalidOperationException($"Cannot determine type of {b}")
    };

    protected override Constant Div(Constant b) => b switch
    {
        ConstantZero _ => throw new DivideByZeroException($"{this}/ 0"),
        ConstantOne _ => this,
        ConstantWhole w => Create(Value / w.Value),
        ConstantFraction f => Create(Value / f.Value),
        _ => throw new InvalidOperationException($"Cannot determine type of {b}")
    };

    protected override Constant Add(Constant b)
    {
        return b switch
        {
            ConstantZero _ => this,
            ConstantOne _ => Create(Value + 1),
            ConstantWhole w => Create(Value + w.Value),
            ConstantFraction f => Create(Value + f.Value),
            _ => throw new InvalidOperationException($"Cannot determine type of {b}")
        };
    }

    protected override Constant Negate() => Create(-Value);

    public override Expr ToExpr() => Value.ToExpr();

    public override bool Equals(Constant other) => Equals(other as ConstantFraction);

    public bool Equals(ConstantFraction other) => this.TestNullBeforeEquals(other, () => Value.Equals(other.Value));

    public override int GetHashCode() => unchecked(-1534900553.ChainHashCode(Value));

    public static bool operator ==(ConstantFraction fraction1, ConstantFraction fraction2) => fraction1.DefaultEquals(fraction2);

    public static bool operator !=(ConstantFraction fraction1, ConstantFraction fraction2) => !(fraction1 == fraction2);
}
