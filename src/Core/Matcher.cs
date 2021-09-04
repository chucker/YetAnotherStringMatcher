using System;
using System.Linq;
using System.Collections.Generic;
using YetAnotherStringMatcher.Requirements;

namespace YetAnotherStringMatcher
{
    public class Matcher
    {
        public string OriginalString { get; }

        private int Index { get; set; } = 0;

        private List<IRequirement> Requirements { get; set; } = new List<IRequirement>();

        public Matcher(string item)
        {
            OriginalString = item;
        }

        // Public API

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

        public Matcher ThenCustom(IRequirement requirement)
        {
            Requirements.Add(requirement);
            return this;
        }

        public Matcher Match(string item)
        {
            return Then(item);
        }

        public Matcher MatchAnyOf(params string[] items)
        {
            return ThenCustom(new ThenAnyOfRequirement(items));
        }

        public Matcher Then(string item)
        {
            return ThenCustom(new ThenRequirement(item));
        }

        public Matcher ThenAnyOf(params string[] items)
        {
            return ThenCustom(new ThenAnyOfRequirement(items));
        }

        public Matcher ThenAnything()
        {
            return ThenCustom(new ThenAnythingRequirement());
        }

        public Matcher ThenAnythingOfLength(int i)
        {
            return ThenCustom(new ThenAnythingRequirement(i));
        }

        public Matcher IgnoreCase()
        {
            if (Requirements.Count == 0)
                throw new Exception("Cannot apply Options when there's not even one Requirement registered.");

            var last = Requirements.Last().Options.IgnoreCase = true;

            return this;
        }

        public Matcher IgnoreCaseForAllExisting()
        {
            if (Requirements.Count == 0)
                throw new Exception("Cannot apply Options when there's not even one Requirement registered.");

            foreach (var req in Requirements)
            {
                req.Options.IgnoreCase = true;
            }

            return this;
        }

        // Internal API

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
                        // We ignore index out of bounds for last requirement.
                        if (i != Requirements.Count - 1)
                        {
                            var msg = $"After successful check '{requirement.GetType().Name}'" +
                                $" the new Index was set to incorrect value - {result.NewIndex}.";

                            throw new Exception(msg);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    return new EvaluationResult(false, $"Requirement number: {i} ('{requirement.Name}') wasn't fulfilled.");
                }
            }

            return new EvaluationResult(true, "");
        }

        private void Prepare()
        {
            var indices_to_to_remove = new List<int>();

            for (int i = 0; i < Requirements.Count; i++)
            {
                var current = Requirements[i];

                if (current is ThenAnythingRequirement tar)
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
