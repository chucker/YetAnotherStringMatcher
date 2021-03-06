namespace YetAnotherStringMatcher.Requirements
{
    public class AnythingRequirement : IRequirement
    {
        public AnythingRequirement()
        {

        }

        public AnythingRequirement(int expectedLength)
        {
            ExpectedLength = expectedLength;
        }

        public string Name => "Match some element(s)";

        public CheckOptions Options { get; set; } = new CheckOptions();

        public bool IsLastRequirement => NextRequirement is null;

        public IRequirement NextRequirement { get; set; }

        private int? ExpectedLength { get; }

        public CheckResult Check(string original, int index)
        {
            if (original is null)
                return new CheckResult(false, index);

            var validIndex = IndexHelper.WithinBounds(original, index);

            if (!validIndex)
                return new CheckResult(false, index);

            if (IsLastRequirement)
            {
                // Because the way we manipulate indices,
                // we're standing on last index and we haven't checked it yet,
                // but since it can be Anything, then we're fine.
                // With this approach "last index" is at LastIndex+1 (Length/Count)

                if (index == original.Length)
                {
                    return new CheckResult(false, index);
                }
                else
                {
                    return new CheckResult(true, original.Length);
                }
            }

            if (ExpectedLength.HasValue)
            {
                var aheadIndex = index + ExpectedLength.Value;

                if (!IndexHelper.WithinBounds(original, aheadIndex))
                {
                    return new CheckResult(false, index);
                }

                var result = NextRequirement.Check(original, aheadIndex);

                if (result.Success)
                {
                    return new CheckResult(true, result.NewIndex);
                }
            }
            else
            {
                // + 1 because we want to match at least one character.
                for (int i = index + 1; i <= original.Length; i++)
                {
                    var result = NextRequirement.Check(original, i);

                    if (result.Success)
                    {
                        return new CheckResult(true, i + 1);
                    }
                }
            }

            return new CheckResult(false, index);
        }

        public override string ToString()
        {
            return "Then Anything Requirement";
        }
    }
}
