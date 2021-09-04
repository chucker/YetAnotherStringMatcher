namespace YetAnotherStringMatcher
{
    public interface IRequirement
    {
        /// <summary>
        /// User friendly name of this requirement.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Takes original string and current index
        /// e.g "abc" and 1
        /// and has to return whether current requirement is fulfilled and
        /// at which index should we move after this requirement.
        /// For example: We have string "abc" and current index "1"
        /// And our requirement tries to find string "b"
        /// Since "b" is at current index, then we return success and the next index within bounds - 2.
        /// </summary>
        /// <param name="original">Full, original string</param>
        /// <param name="index">Current index within original string which indicates start place for our requirement</param>
        /// <returns>Returns whether current requirement is fulfilled and at which next index we should move.</returns>
        CheckResult Check(string original, int index);

        CheckOptions Options { get; set; }
    }
}