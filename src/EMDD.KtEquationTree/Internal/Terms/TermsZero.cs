﻿using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Singles;

namespace Parser.Expression;
internal class TermsZero : TermsBase
{
    public override TermsBase Concat(IEnumerable<TermBase> terms) => Terms.Create(terms);

    public override TermsBase Div(TermsBase s) => new TermsZero();

    public override IEnumerator<TermBase> GetEnumerator()
    {
        yield break;
    }

    public override TermsBase Raise(TermsBase b) => new TermsZero();

    public override Expr ToExpr() => new Zero();

    protected override TermsBase Mult(TermsBase b) => new TermsZero();

    protected override TermsBase Negate() => new TermsZero();
}
