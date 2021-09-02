using Xunit;
using YetAnotherStringMatcher;

namespace Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test1()
        {
            // Testing Then

            var matcher = new Matcher("123_123")
                            .Match("123")
                            .Then("_")
                            .Then("123")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test2()
        {
            // Testing ThenAnyOf

            var matcher = new Matcher("123_123")
                            .Match("123")
                            .ThenAnyOf("123", "_", "_1", "_12")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test3()
        {
            // Multiple_Evaluations

            var matcherRules = new Matcher("123_123")
                            .Match("123")
                            .ThenAnyOf("123", "_", "_1", "_12");

            var evaluation1 = matcherRules.Check();
            var evaluation2 = matcherRules.Check();
            var evaluation3 = matcherRules.Check();

            Assert.True(evaluation1.Success);
            Assert.True(evaluation2.Success);
            Assert.True(evaluation3.Success);
        }

        [Fact]
        public void Test4()
        {
            // Test IgnoreCaseOptions

            var options = new CheckOptions
            {
                IgnoreCase = true
            };

            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_", "_Ab", "_aE", "_ABC").WithOptions(options)
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test5()
        {
            // Test Case Fail

            var options = new CheckOptions
            {
                IgnoreCase = true
            };

            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_aBC", "_Ab", "_A", "_ABC")
                            .Check();

            Assert.False(result.Success);
        }
    }
}
