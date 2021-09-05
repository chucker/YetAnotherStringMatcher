using System;
using System.Collections.Generic;
using Xunit;
using YetAnotherStringMatcher;

namespace Tests
{
    public class Examples
    {
        [Fact]
        public void Test000_PolishPostalCode()
        {
            // Polish Postal Code
            var input = "12-345";

            var result = new Matcher(input)
                             .MatchDigitsOfLength(2)
                             .Then("-")
                             .ThenDigitsOfLength(3)
                             .Check();

            Assert.True(result.Success);
        }

        [Fact]
        public void Test001_SamplePhoneNumbers()
        {
            // Sample Phone Number / Reusable Pattern
            var input = new List<string> { "+123 345 67 89", "+1424 345 67 89" };

            var pattern = new Matcher()
                              .Match("+")
                              .ThenDigitsOfLength(3)
                              .Then(" ")
                              .ThenDigitsOfLength(3)
                              .Then(" ")
                              .ThenDigitsOfLength(2)
                              .Then(" ")
                              .ThenDigitsOfLength(2);

            Assert.True(pattern.Check(input[0]).Success);
            Assert.False(pattern.Check(input[1]).Success);
        }

        // For docs purposes
        [Fact]
        public void Test001_Test()
        {
            var result = new Matcher("Apple Pineapple")
                            .Match("Apple ")
                            .Then("Coconut ").IsOptional()
                            .Then("Pineapple")
                            .Check();

            Assert.True(result.Success);
        }
    }
}
