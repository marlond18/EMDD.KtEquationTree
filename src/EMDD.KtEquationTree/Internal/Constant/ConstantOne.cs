using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Fractions;

using Parser.Expression;

namespace EMDD.KtEquationTree.Constant;
internal class ConstantOne : Constant, IEquatable<ConstantOne>
{
    public override TermBase Raise(Constant coefficient) => new TermOne();

    public override int GetHashCode() => -1937169420;

    public bool Equals(ConstantOne other) => true;

    public override string ToString() => "1";

    protected override Constant Add(Constant b) => b switch
    {
        ConstantZero _ => this,
        ConstantOne _ => ConstantWhole.Create(2),
        ConstantWhole w => ConstantWhole.Create(1 + w.Value),
        ConstantFraction f => ConstantFraction.Create(1 + f.Value),
        _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
    };

    protected override Constant Negate() => ConstantWhole.Create(-1);

    protected override Constant Mult(Constant b) => b;

    protected override Constant Div(Constant b) => b switch
    {
        ConstantZero _ => throw new DivideByZeroException($"{this}/0"),
        ConstantWhole w => ConstantFraction.Create(new Fraction(1, w.Value)),
        ConstantOne o => o,
        ConstantFraction f => ConstantFraction.Create(f.Value),
        _ => throw new InvalidOperationException($"Cannot determine the type of {b}")
    };

    public override Expr ToExpr() => new One();

    public override bool Equals(Constant other) => Equals(other as ConstantOne);
}