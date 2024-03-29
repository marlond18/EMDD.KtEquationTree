﻿using EMDD.KtEquationTree.Exprs.Singles;
using EMDD.KtEquationTree.Factors;
using EMDD.KtEquationTree.MathStatements;
using EMDD.KtEquationTree.Relations;

using Parser.Expression;

namespace EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
public class MultiplyOp : MultiplicativeBinaryOp
{
    protected MultiplyOp(Expr left, Expr right) : base(left, right)
    {
    }

    public static Expr Create(BigInteger left, Expr right) => Create(Literal.Create(left), right);
    public static Expr Create(Expr left, BigInteger right) => Create(left, Literal.Create(right));
    public static Expr Create(BigInteger left, BigInteger right) => Create(Literal.Create(left), Literal.Create(right));

    public static Expr Create(Expr left, Expr right) => (left, right) switch
    {
        (null, _) => new Zero(),
        (_, null) => new Zero(),
        (_, Zero z) => z,
        (Zero z, _) => z,
        (One _, _) => right,
        (_, One _) => left,
        (Literal l, Literal r) => Literal.Create(l.Value * r.Value),
        (FractOp d1, FractOp d2) => DivideOp.Create(d1.Left * d2.Left, d1.Right * d2.Right),
        _ => new MultiplyOp(left, right)
    };

    public static MathStatement Create(MathStatement left, MathStatement right) => (left, right) switch
    {
        (Expr el, Expr er) => Create(el, er),
        (Expr el, EqualsOp eor) => EqualsOp.Create(Create(el, eor.Left), Create(el, eor.Right)),
        (EqualsOp eol, EqualsOp eor) => EqualsOp.Create(Create(eol.Left, eor.Left), Create(eol.Right, eor.Right)),
        (EqualsOp eol, Expr er) => EqualsOp.Create(Create(eol.Left, er), Create(eol.Right, er)),
        _ => throw new NotImplementedException()
    };

    internal override string Op => "×";

    protected override (Expr left, Expr right) Arrange(Expr a, Expr b)
    {
        if (a is not Literal && b is Literal) return (b, a);
        return (a, b);
    }

    internal override TermsBase Terms() => Left.Simplify().Terms() * Right.Simplify().Terms();

    public override Expr Simplify()
    {
        if (IsSimple) return this;
        var v = Left.Simplify() * Right.Simplify();
        v.IsSimple = true;
        return v;

    }

    public override IEnumerable<Expr> Factor() =>
        Left.Factor().Concat(Right.Factor()).ToLookup(f => f).Select(g => (b: g.Key, e: g.Count())).Select(pair => pair.e > 1 ? PowerOp.Create(pair.b, pair.e) : pair.b);

    public override Expr Invert() => Left.Invert() * Right.Invert();

    public override int GetHashCode() => unchecked(539060728.ChainHashCode(Left).ChainHashCode(Right));

    internal override FactorsBase InnerFactor() => IsSimple ? Left.InnerFactor().Concat(Right.InnerFactor()) : Simplify().InnerFactor();

    public override bool TryToDouble(out double value)
    {
        if (Left.TryToDouble(out double valLeft) && Right.TryToDouble(out double valRight))
        {
            value = valLeft * valRight;
            return true;
        }
        value = 0;
        return false;
    }

    public override Expr Substitute(Expr current, Expr replacement)
    {
        var newLeft = Left == current ? replacement : Left.Substitute(current, replacement);
        var newRight = Right == current ? replacement : Right.Substitute(current, replacement);
        return Create(newLeft, newRight);
    }
}