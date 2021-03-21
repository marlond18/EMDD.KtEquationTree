using System.Collections.Generic;
using System.Linq;

using KtExtensions;

namespace EMDD.KtEquationTree.Constant
{
    internal static class ConstantsHelper
    {
        public static Constant Sum(this IEnumerable<Constant> collection) =>
            collection.Aggregate((c1, c2) => c1 + c2);
    }
}