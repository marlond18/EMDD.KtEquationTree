﻿using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;
using EMDD.KtEquationTree.MathStatements;

namespace EMDD.KtEquationTree.Exprs.Unary;
public class ImaginaryNumber : SqrtOp
{
    public ImaginaryNumber() : base(1)
    {
        IsSimple = true;
    }
}

public class SqrtOp : UnaryOp
{
    protected SqrtOp(Expr expr) : base(expr)
    {
    }

    protected SqrtOp(BigInteger i) : base(i)
    {
    }

    public static Expr Create(Expr expr) => expr switch
    {
        null => new Zero(),
        Zero zero => zero,
        One one => one,
        NegativeOne _ => new ImaginaryNumber(),
        _ => new SqrtOp(expr)
    };

    public static MathStatement Create(MathStatement ms) => ms switch
    {
        Expr e=> Create(e),
        EqualsOp eo=> EqualsOp.Create(Create(eo.Left), Create(eo.Right)),
        _ => throw new NotImplementedException()
    };

    public static Expr Create(BigInteger i) => i switch
    {
        { IsOne: true } => new ImaginaryNumber(),
        { IsZero: true } => new Zero(),
        _ when i < 0 => new ImaginaryNumber(),
        _ => new SqrtOp(i)
    };

    private bool Equals(SqrtOp other) => this.TestNullBeforeEquals(other, () => Expr.Equals(other.Expr));

    protected override bool Equals(UnaryOp other) => Equals(other as SqrtOp);

    public override string ToString() => $"√{ExprToString()}";

    internal override TermsBase Terms() => Expr.Simplify().Terms().Raise(TermsSingle.Create(TermConstant.Create(1, 2)));

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Expr.Sqrt();
        v.IsSimple = true;
        return v;
    }

    public override int GetHashCode() => unchecked(539060724.ChainHashCode(Expr));

    public override IEnumerable<Expr> Factor() => new[] { this };

    public override Expr Invert() => PowerOp.Create(Expr, DivideOp.Create(-1, 2));

    internal override FactorsBase InnerFactor() => FactorsSingle.Create(FactorPowN.Create(Expr, DivideOp.Create(1, 2)));

    public override bool TryToDouble(out double value)
    {
        if (Expr.TryToDouble(out double val1) && val1 > 0)
        {
            value = Math.Sqrt(val1);
            return true;
        }
        value = 0;
        return false;
    }

    public override Expr Substitute(Expr current, Expr replacement)
    {
        var newExpr = Expr == current ? replacement : Expr.Substitute(current, replacement);
        return Create(newExpr);
    }
}