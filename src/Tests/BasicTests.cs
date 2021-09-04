using Xunit;
using YetAnotherStringMatcher;

namespace Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test000()
        {
            // No requirements 

            var result = new Matcher("abc_aBc").Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test001()
        {
            // Testing Then

            var matcher = new Matcher("123_123x")
                            .Match("123")
                            .Then("_")
                            .Then("123")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test002()
        {
            // Testing ThenAnyOf

            var matcher = new Matcher("123_123")
                            .Match("123")
                            .ThenAnyOf("123", "_", "_1", "_12")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test003()
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
        public void Test004()
        {
            // Test IgnoreCaseOptions

            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_", "_Ab", "_aE", "_ABC").IgnoreCase()
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test005()
        {
            // Test Case Fail

            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_aBC", "_Ab", "_A", "_ABC")
                            .Check();

            Assert.False(result.Success);
        }

        [Fact]
        public void Test006()
        {
            // Then Anything 

            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnything()
                            .Then("c")
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test007()
        {
            // Then Anything Of Length 

            var result = new Matcher("abc12c")
                            .Match("abc")
                            .ThenAnythingOfLength(2)
                            .Then("c")
                            .Check();

            Assert.True(result.Success);
        }
    }
}
