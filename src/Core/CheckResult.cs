namespace YetAnotherStringMatcher
{
    public class CheckResult
    {
        public CheckResult(bool success, int newIndex)
        {
            Success = success;
            NewIndex = newIndex;
        }

        public bool Success { get; set; }

        public int NewIndex { get; set; }

        public override string ToString()
        {
            return $"Success? {Success} Index? {NewIndex}";
        }
    }
}
