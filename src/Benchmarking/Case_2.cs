using YetAnotherStringMatcher;
using BenchmarkDotNet.Attributes;
using System.Text.RegularExpressions;

namespace Benchmarking
{
    [MemoryDiagnoser]
    public class Case_2
    {
        [Params
        (
            "12312312312asdfsdfsdfdsApple",
            "qqqqqqqqqZZZZZZAppleWatermelon",
            "xcvxcvcxvcxvcxvxcvxcvxcvxcvxc12312Watermelon"
        )]
        public string text;

        [Benchmark]
        public bool Case2_YASM()
        {
            return new Matcher(text)
                .MatchAnything()
                .Then("Apple")
                .Check()
                .Success;
        }

        [Benchmark]
        public bool Case2_Regex()
        {
            return new Regex("Apple").IsMatch(text);
        }
    }
}
