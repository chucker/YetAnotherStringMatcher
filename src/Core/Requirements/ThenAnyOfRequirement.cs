using System;
using System.Linq;

namespace YetAnotherStringMatcher.Requirements
{
    public class ThenAnyOfRequirement : IRequirement
    {
        public string Name => "Match one element";

        public string[] Items { get; set; }

        private CheckOptions Options { get; set; }

        public ThenAnyOfRequirement(params string[] items)
        {
            Items = items.OrderByDescending(x => x.Length).ToArray();
        }

        public CheckResult Check(string original, int index)
        {
            if (original is null)
                return new CheckResult(false, index);

            foreach (var item in Items)
            {
                var strComparison = Options?.IgnoreCase ?? false ?
                                    StringComparison.OrdinalIgnoreCase :
                                    StringComparison.Ordinal;

                if (original.IndexOf(item, index, strComparison) == index)
                {
                    return new CheckResult(true, index + item.Length);
                }
            }

            return new CheckResult(false, index);
        }

        public void ApplyOptions(CheckOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            Options = options;
        }
    }
}
