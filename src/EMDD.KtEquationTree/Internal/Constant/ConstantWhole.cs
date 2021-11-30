using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Fractions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Constant;
internal class ConstantWhole : Constant, IEquatable<ConstantWhole>
{
    protected ConstantWhole(BigInteger value)
    {
        Value = value;
    }

    public BigInteger Value { get; }

    public static Constant Create(BigInteger value) => value switch
    {
        { IsZero: true } => new ConstantZero(),
        { IsOne: true } => new ConstantOne(),
        _ => new ConstantWhole(value)
    };

    public override TermBase Raise(Constant coefficient) => coefficient switch
    {
        ConstantZero _ => new TermOne(),
        ConstantOne _ => TermConstant.Create(this),
        ConstantWhole w => TermConstant.Create(ConstantFraction.Create(Value.Raise(w.Value))),
        ConstantFraction f => Value.Raise(f.Value).ConvertBigIntRaiseToTerm(),
        _ => throw new InvalidOperationException($"Cannot determine type of {coefficient}")
    };

    public override string ToString() => Value.ToString("R");

    protected override Constant Mult(Constant b) => b switch
    {
        ConstantZero z => z,
        ConstantOne _ => this,
        ConstantWhole w => Create(Value * w.Value),
        ConstantFraction f => ConstantFraction.Create(f.Value * Value),
        _ => throw new InvalidOperationException($"Cannot determine type of {b}")
    };

    protected override Constant Div(Constant b) => b switch
    {
        ConstantZero _ => throw new DivideByZeroException($"{this}/ 0"),
        ConstantOne _ => this,
        ConstantWhole w => ConstantFraction.Create(new Fraction(Value, w.Value)),
        ConstantFraction f => ConstantFraction.Create(f.Value * Value),
        _ => throw new InvalidOperationException($"Cannot determine type of {b}")
    };

    protected override Constant Add(Constant b) => b switch
    {
        ConstantZero _ => this,
        ConstantOne _ => Create(Value + 1),
        ConstantWhole w => Create(Value + w.Value),
        ConstantFraction f => ConstantFraction.Create(f.Value + Value),
        _ => throw new InvalidOperationException($"Cannot determine type of {b}")
    };

    protected override Constant Negate() => Create(-Value);

    public override Expr ToExpr() => Value.ToExpr();

    public bool Equals(ConstantWhole other) => this.TestNullBeforeEquals(other, () => Value == other.Value);

    public override bool Equals(Constant other) => Equals(other as ConstantWhole);

    public override int GetHashCode() => unchecked(-1937169414.ChainHashCode(Value));

    public static bool operator ==(ConstantWhole whole1, ConstantWhole whole2) => whole1.DefaultEquals(whole2);

    public static bool operator !=(ConstantWhole whole1, ConstantWhole whole2) => !(whole1 == whole2);
}