using System;
using System.Collections.Generic;
using System.Linq;

using EMDD.KtEquationTree.Exprs;
using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;

using KtExtensions;

using Parser.Expression.Var;

namespace Parser.Expression
{

    internal class TermsSingle : TermsBase, IEnumerable<TermBase>
    {
        public TermBase Term { get; }

        private TermsSingle(TermBase term)
        {
            Term = term;
        }

        public static TermsBase Create(TermBase term) => term switch
        {
            TermZero _ => new TermsZero(),
            _ => new TermsSingle(term)
        };

        public override TermsBase Concat(IEnumerable<TermBase> terms) => Terms.Create(new[] { Term }.Concat(terms));

        public override IEnumerator<TermBase> GetEnumerator()
        {
            yield return Term;
        }

        protected override TermsBase Mult(TermsBase b) => b switch
        {
            TermsZero _ => new TermsZero(),
            TermsSingle s => Create(Term * s.Term),
            _ => Terms.Create(b.Select(t => t * Term))
        };

        protected override TermsBase Negate() => new TermsSingle(-Term);

        public override TermsBase Raise(TermsBase b) => b switch
        {
            TermsZero _ => Create(new TermOne()),
            TermsSingle s => Create(Term.Raise(s.Term)),
            _ => Create(TermVariables.Create(VarPowN.Create(ToExpr(), b.ToExpr())))
        };

        public override Expr ToExpr() => Term.ToExpr();

        public override TermsBase Div(TermsBase s)
        {
            return s switch
            {
                TermsZero _ => throw new DivideByZeroException($"{this}/{s}"),
                TermsSingle ss => new TermsSingle(Term / ss.Term),
                _ => Create(TermVariables.Create(Variables.Create(VarN.Create(DivideOp.Create(ToExpr(), s.ToExpr())))))
            };
        }
    }
}