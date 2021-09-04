namespace YetAnotherStringMatcher
{
    public class EvaluationResult
    {
        public EvaluationResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }

        public string ErrorMessage { get; }

        public override string ToString()
        {
            return $"Success? {Success} ErrorMessage? \"{ErrorMessage}\"";
        }
    }
}
