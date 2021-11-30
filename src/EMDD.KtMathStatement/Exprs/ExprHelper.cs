using EMDD.KtEquationTree.Exprs.Binary.Multiplicative;
using EMDD.KtEquationTree.Exprs.Unary;

namespace EMDD.KtMathStatement.Exprs;
public static class ExprHelper
{
    public static Expr RaiseToNegativeOne(this Expr expr) => PowerOp.Create(expr, new NegativeOne());

    public static Expr Sum(this IEnumerable<Expr> exprs) =>
        exprs.Aggregate((e1, e2) => e1 + e2);

    public static Expr Product(this IEnumerable<Expr> exprs) => exprs.Aggregate((e1, e2) => e1 * e2);

    public static Expr Root(this Expr @base, Expr root) => PowerOp.Create(@base, DivideOp.Create(1, root));

    public static string Parenthesize(this Expr expr) => $"({expr})";

    public static IEnumerable<T1> ReplaceAll<T1, T>(this T enumerable, T1 original, T1 replacement) where T : IEnumerable<T1>
    {
        foreach (var val in enumerable)
        {
            if (val.Equals(original)) yield return replacement;
            else yield return val;
        }
    }
}