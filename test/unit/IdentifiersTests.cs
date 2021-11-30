namespace KtEquationTreeTest;

[TestClass]
public class IdentifiersTests
{
    [DataTestMethod]
    [WithSubscriptTestTestData]
    public void WithSubscriptTest(string expr)
    {
        var e = ExprParser.ParseOrThrow(expr);
        if (e is Identifier id)
        {
            Assert.AreEqual(id.Definition, expr);
        }
        else
        {
            Assert.Fail($"{e} from {expr} is not an identifier");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class WithSubscriptTestTestDataAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] { "a_a" };
            yield return new object[] { "a_" };
            yield return new object[] { "b_a" };
            yield return new object[] { "b_adaa1" };
            yield return new object[] { "b1_adaa" };
            yield return new object[] { "b1_1adaa" };
            yield return new object[] { "b1_1" };
            yield return new object[] { "fᐠ_c" };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return $"{data[0]} is an identifier";
        }
    }
}