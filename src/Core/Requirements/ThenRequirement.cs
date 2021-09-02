using System;

namespace YetAnotherStringMatcher.Requirements
{
    public class ThenRequirement : IRequirement
    {
        public string Name => "Match one element";

        public string Item { get; set; }

        private CheckOptions Options { get; set; }

        public ThenRequirement(string item)
        {
            Item = item;
        }

        public CheckResult Check(string original, int index)
        {
            if (original is null)
                return new CheckResult(false, index);

            var strComparison = Options?.IgnoreCase ?? false ?
                                StringComparison.OrdinalIgnoreCase :
                                StringComparison.Ordinal;

            var result = original.IndexOf(Item, index, strComparison) == index;

            var newIndex = result ? index + Item.Length : index;

            return new CheckResult(result, newIndex);
        }

        public void ApplyOptions(CheckOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            Options = options;
        }
    }
}
