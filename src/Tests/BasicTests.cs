using System;
using Xunit;
using YetAnotherStringMatcher;
using System.Collections.Generic;

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

        [Fact]
        public void Test017_EndRequirement_Fails()
        {
            var result = new Matcher("abc_c")
                            .Match("abc")
                            .NoMore()
                            .Check();

            Assert.False(result.Success);
        }

        [Fact]
        public void Test018_EndRequirement()
        {
            var result = new Matcher("abc_")
                            .Match("abc")
                            .ThenAnything()
                            .NoMore()
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test019_BadEndRequirement()
        {
            var result = new Matcher("abc_")
                            .Match("abc_")
                            .NoMore()
                            .NoMore();

            Assert.Throws<InvalidOperationException>(() => result.Check());
        }

        [Fact]
        public void Test020_BadEndRequirement()
        {
            var result = new Matcher("abc_1")
                            .Match("abc_")
                            .NoMore()
                            .Then("1");

            Assert.Throws<InvalidOperationException>(() => result.Check());
        }

        [Fact]
        public void Test021_DigitsWithLengthBetween()
        {
            var inputs = new List<string> { "+1", "+12", "+123", "+1234" };

            var pattern = new Matcher()
                            .Match("+")
                            .ThenDigitsWithLengthBetween(1, 3)
                            .NoMore();

            Assert.True(pattern.Check(inputs[0]).Success);
            Assert.True(pattern.Check(inputs[1]).Success);
            Assert.True(pattern.Check(inputs[2]).Success);

            Assert.False(pattern.Check(inputs[3]).Success);
        }

        [Fact]
        public void Test022_ThenAnything_EdgeCase()
        {
            var result = new Matcher("12")
                            .Match("1")
                            .ThenAnything()
                            .Then("2")
                            .Check();

            Assert.False(result.Success);
        }

        [Fact]
        public void Test023_ThenAnything()
        {
            var result = new Matcher("12")
                            .Match("1")
                            .ThenAnything()
                            .Then("2")
                            .Then("2")
                            .Check();

            Assert.False(result.Success);
        }

        [Fact]
        public void Test024_Optional()
        {
            var result = new Matcher("13")
                            .Match("1")
                            .Then("2").IsOptional()
                            .Then("3")
                            .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test025_ThenAnyOf()
        {
            var input = new List<string>
            {
                "[2021-09-05] ERROR: Message1",
                "[2021-09-05] WARNING: Message1",
                "[2021-09-07] WARNING: Message1",
            };

            var pattern = new Matcher()
                              .Match("[2021-09-05] ")
                              .ThenAnyOf("WARNING:", "ERROR:");

            Assert.True(pattern.Check(input[0]).Success);
            Assert.True(pattern.Check(input[1]).Success);

            Assert.False(pattern.Check(input[2]).Success);
        }
    }
}
