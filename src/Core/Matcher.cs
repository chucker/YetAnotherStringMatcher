using System;
using System.Linq;
using System.Collections.Generic;
using YetAnotherStringMatcher.Requirements;

namespace YetAnotherStringMatcher
{
    public class Matcher
    {
        private int Index { get; set; } = 0;

        internal List<IRequirement> Requirements { get; set; } = new List<IRequirement>();

        public string OriginalString { get; private set; }

        public Matcher(string item)
        {
            OriginalString = item;
        }

        public Matcher()
        {
        }

        // Public API here and inside MatcherExtensionMethods.cs

        public EvaluationResult Check()
        {
            try
            {
                Prepare();
                return Evaluate();
            }
            finally
            {
                Reset();
            }
        }

        public EvaluationResult Check(string input)
        {
            OriginalString = input;
            return Check();
        }

        public Matcher ThenCustom(IRequirement requirement)
        {
            Requirements.Add(requirement);
            return this;
        }

        // Internal Stuff/APIs

        private void Reset()
        {
            Index = 0;
        }

        private EvaluationResult Evaluate()
        {
            if (OriginalString is null)
                return new EvaluationResult(false, "Original string is null");

            for (int i = 0; i < Requirements.Count; i++)
            {
                var requirement = Requirements[i];

                var result = requirement.Check(OriginalString, Index);

                if (result.Success)
                {
                    if (result.NewIndex < Index)
                    {
                        throw new Exception("Requirement shouldn't return index smaller than it received." +
                            $"Received index: {Index}; New index: {result.NewIndex}. ");
                    }

                    if (IndexHelper.WithinBounds(OriginalString, result.NewIndex))
                    {
                        Index = result.NewIndex;
                    }
                    else
                    {
                        // We ignore index out of bounds for last requirement that isnt EndRequirement.
                        if (i != Requirements.Count - 1)
                        {
                            var next = Requirements[i + 1];
                            if (next is EndRequirement)
                            {
                                Index = result.NewIndex;
                            }
                            else
                            {
                                var msg = $"After successful check '{requirement.GetType().Name}'" +
                                    $" the new Index was set to incorrect value - {result.NewIndex}.";

                                throw new Exception(msg);
                            }
                        }
                        else if (i == Requirements.Count - 1)
                        {
                            break;
                        }
                    }
                }
                else if (!requirement.Options.Optional)
                {
                    return new EvaluationResult(false, $"Requirement number: {i + 1} ('{requirement.Name}') wasn't fulfilled.");
                }
            }

            return new EvaluationResult(true, "");
        }

        private void Prepare()
        {
            var endRequirements = Requirements
                .Select((x, Index) => new { Object = x, Index })
                .Where(data => data.Object is EndRequirement)
                .Select(x => x.Index)
                .ToList();

            if (endRequirements.Any() &&
                (endRequirements.Count > 1 || endRequirements[0] != Requirements.Count - 1))
            {
                throw new InvalidOperationException("NoMore can occur only once at the end of pattern");
            }

            var indices_to_to_remove = new List<int>();

            for (int i = 0; i < Requirements.Count; i++)
            {
                var current = Requirements[i];

                if (current is AnythingRequirement tar)
                {
                    if (i < Requirements.Count - 1)
                    {
                        i++;
                        tar.NextRequirement = Requirements[i];
                        indices_to_to_remove.Add(i);
                    }
                }
            }

            Requirements = Requirements.Where((req, index) => !indices_to_to_remove.Contains(index)).ToList();
        }
    }
}
