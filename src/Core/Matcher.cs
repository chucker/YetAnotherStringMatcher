using System;
using System.Linq;
using System.Collections.Generic;
using YetAnotherStringMatcher.Requirements;

namespace YetAnotherStringMatcher
{
    public class Matcher
    {
        private int Index { get; set; } = 0;

        private List<IRequirement> Requirements { get; set; } = new List<IRequirement>();

        public string OriginalString { get; private set; }

        public Matcher(string item)
        {
            OriginalString = item;
        }

        public Matcher()
        {
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

        public EvaluationResult Check(string input)
        {
            OriginalString = input;
            return Check();
        }

        // Basic

        public Matcher Match(string item)
        {
            return Then(item);
        }

        public Matcher Then(string item)
        {
            return ThenCustom(new ThenRequirement(item));
        }

        // AnyOf
        public Matcher MatchAnyOf(params string[] items)
        {
            return ThenCustom(new AnyOfRequirement(items));
        }

        public Matcher ThenAnyOf(params string[] items)
        {
            return ThenCustom(new AnyOfRequirement(items));
        }

        // Anything
        public Matcher MatchAnything()
        {
            return ThenCustom(new AnythingRequirement());
        }

        public Matcher ThenAnything()
        {
            return ThenCustom(new AnythingRequirement());
        }

        // Anything Of Length

        public Matcher MatchAnythingOfLength(int length)
        {
            return ThenCustom(new AnythingRequirement(length));
        }

        public Matcher ThenAnythingOfLength(int length)
        {
            return ThenCustom(new AnythingRequirement(length));
        }

        // Digits Of Length

        public Matcher MatchDigitsOfLength(int length)
        {
            Func<char, CheckOptions, bool> predicate =
                (char c, CheckOptions o) => char.IsDigit(c);

            return ThenCustom(new SomethingOfLenghtRequirement(length, predicate));
        }

        public Matcher ThenDigitsOfLength(int length)
        {
            Func<char, CheckOptions, bool> predicate =
                (char c, CheckOptions o) => char.IsDigit(c);

            return ThenCustom(new SomethingOfLenghtRequirement(length, predicate));
        }

        // Digits With Length Between

        public Matcher MatchDigitsWithLengthBetween(int min, int max)
        {
            Func<char, CheckOptions, bool> predicate =
                (char c, CheckOptions o) => char.IsDigit(c);

            return ThenCustom(new SomethingWithLenghtBetweenRequirement(min, max, predicate));
        }

        public Matcher ThenDigitsWithLengthBetween(int min, int max)
        {
            Func<char, CheckOptions, bool> predicate =
                (char c, CheckOptions o) => char.IsDigit(c);

            return ThenCustom(new SomethingWithLenghtBetweenRequirement(min, max, predicate));
        }

        // Symbols Of Length

        public Matcher MatchSymbolsOfLength(char[] symbols, int length)
        {
            return ThenCustom(new SymbolsOfLengthRequirement(length, symbols));
        }

        public Matcher ThenSymbolsOfLength(char[] symbols, int length)
        {
            return ThenCustom(new SymbolsOfLengthRequirement(length, symbols));
        }

        // Custom Of Length

        public Matcher MatchCustomOfLength(Func<char, CheckOptions, bool> pred, int length)
        {
            return ThenCustom(new SomethingOfLenghtRequirement(length, pred));
        }

        public Matcher ThenCustomOfLength(Func<char, CheckOptions, bool> pred, int length)
        {
            return ThenCustom(new SomethingOfLenghtRequirement(length, pred));
        }

        // Other:

        public Matcher NoMore()
        {
            return ThenCustom(new EndRequirement());
        }

        public Matcher IgnoreCase()
        {
            if (Requirements.Count == 0)
                throw new Exception("Cannot apply Options when there's not even one Requirement registered.");

            var last = Requirements.Last().Options.IgnoreCase = true;

            return this;
        }

        public Matcher IsOptional()
        {
            if (Requirements.Count == 0)
                throw new Exception("Cannot apply Options when there's not even one Requirement registered.");

            var last = Requirements.Last().Options.Optional = true;

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
