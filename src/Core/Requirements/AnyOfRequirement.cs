using System;
using System.Linq;

namespace YetAnotherStringMatcher.Requirements
{
    public class AnyOfRequirement : IRequirement
    {
        public string Name => "Match one element that's also an element of provided Items list";

        public string[] Items { get; set; }

        public CheckOptions Options { get; set; } = new CheckOptions();

        public AnyOfRequirement(params string[] items)
        {
            // We want to find longest string that matches,
            // so we start searching with the longest strings at the beginning.
            Items = items.OrderByDescending(x => x.Length).ToArray();
        }

        public CheckResult Check(string original, int index)
        {
            if (original is null)
                return new CheckResult(false, index);

            var strComparison = Options?.IgnoreCase ?? false ?
                                StringComparison.OrdinalIgnoreCase :
                                StringComparison.Ordinal;

            foreach (var item in Items)
            {
                if (original.IndexOf(item, index, strComparison) == index)
                {
                    return new CheckResult(true, index + item.Length);
                }
            }

            return new CheckResult(false, index);
        }

        public override string ToString()
        {
            return "Then Any Of Requirement";
        }
    }
}
