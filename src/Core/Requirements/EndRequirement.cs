namespace YetAnotherStringMatcher.Requirements
{
    public class EndRequirement : IRequirement
    {
        public string Name => "Indicates that pattern has to end";

        public CheckOptions Options { get; set; } = new CheckOptions();

        public CheckResult Check(string original, int index)
        {
            if (original is null)
                return new CheckResult(false, index);

            var stringIsLongerThanCurrentIndex = original.Length - 1 > index;

            return new CheckResult(!stringIsLongerThanCurrentIndex, index);
        }

        public override string ToString()
        {
            return "End Requirement";
        }
    }
}
