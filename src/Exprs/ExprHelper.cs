using System.Collections.Generic;
using System.Linq;

using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;

using KtExtensions;

namespace EMDD.KtEquationTree.Exprs
{
    public static class ExprHelper
    {
        public static Expr RaiseToNegativeOne(this Expr expr) => PowerOp.Create(expr, new NegativeOne());

        public static Expr Sum(this IEnumerable<Expr> exprs) =>
            exprs.Aggregate((e1, e2) => e1 + e2);

        public static Expr Product(this IEnumerable<Expr> exprs) => exprs.Aggregate((e1, e2) => e1 * e2);

        public static Expr Root(this Expr @base, Expr root) => PowerOp.Create(@base, DivideOp.Create(1, root));

        public static string Parenthesize(this Expr expr) => $"({expr})";
    }
}