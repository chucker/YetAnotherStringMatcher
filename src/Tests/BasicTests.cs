using System;
using Xunit;
using YetAnotherStringMatcher;

namespace Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test000_NoRequirements()
        {
            var result = new Matcher("abc_aBc").Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test001_Then()
        {
            var matcher = new Matcher("123_123x")
                            .Match("123")
                            .Then("_")
                            .Then("123")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test002_ThenAnyOf()
        {
            var matcher = new Matcher("123_123")
                            .Match("123")
                            .ThenAnyOf("123", "_", "_1", "_12")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test003_Multiple_Evaluations()
        {
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
        public void Test004_IgnoreCaseOption()
        {
            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_", "_Ab", "_aE", "_ABC").IgnoreCase()
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test005_ThenAnyOf_Fail()
        {
            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnyOf("_aBC", "_Ab", "_A", "_ABC")
                            .Check();

            Assert.False(result.Success);
        }

        [Fact]
        public void Test006_ThenAnything()
        {
            var result = new Matcher("abc_aBc")
                            .Match("abc")
                            .ThenAnything()
                            .Then("c")
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test007_ThenAnythingOfLength()
        {
            var result = new Matcher("abc12c")
                            .Match("abc")
                            .ThenAnythingOfLength(2)
                            .Then("c")
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test008_ThenDigitsOfLength()
        {
            var result = new Matcher("abc12c")
                            .Match("abc")
                            .ThenDigitsOfLength(2)
                            .Then("c")
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test009_ThenAnything_v2()
        {
            var result = new Matcher("abc_123aBcQQQQQQQQQQQQ")
                            .Match("abc")
                            .ThenAnything()
                            .Then("c")
                            .ThenAnything()
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test010_ThenAnyOf()
        {
            var matcher = new Matcher("a")
                            .ThenAnyOf("aa")
                            .Check();

            Assert.False(matcher.Success);
        }

        [Fact]
        public void Test011_ThenSymbols()
        {
            var matcher = new Matcher("abc12c")
                            .Match("abc")
                            .ThenSymbolsOfLength("abc12345".ToCharArray(), 2)
                            .Then("c")
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test012_ThenSymbols()
        {
            var matcher = new Matcher("abc12c")
                            .Match("abc")
                            .ThenSymbolsOfLength("abc1345".ToCharArray(), 20)
                            .Then("c")
                            .Check();

            Assert.False(matcher.Success);
        }

        [Fact]
        public void Test013_ThenCustomOfLength()
        {
            Func<char, CheckOptions, bool> pred =
                (char c, CheckOptions o) => c == '1' || c == '3';

            var matcher = new Matcher("123")
                            .ThenCustomOfLength(pred, 1)
                            .ThenAnything()
                            .ThenCustomOfLength(pred, 1)
                            .Check();

            Assert.True(matcher.Success);
        }

        [Fact]
        public void Test014_ThenAnythingOfLength()
        {
            var result = new Matcher("abc12c")
                            .Match("abc")
                            .ThenAnythingOfLength(3)
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test015_ThenSymbols_WithoutIgnoreCase_Fails()
        {
            var matcher = new Matcher("ABC12c")
                            .ThenSymbolsOfLength("cb12345a".ToCharArray(), 6)
                            .Check();

            Assert.False(matcher.Success);
        }

        [Fact]
        public void Test016_ThenSymbols_IgnoreCase()
        {
            var matcher = new Matcher("ABC12c")
                            .ThenSymbolsOfLength("cb12345a".ToCharArray(), 6)
                            .IgnoreCase()
                            .Check();

            Assert.True(matcher.Success);
        }
    }
}
