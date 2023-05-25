using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace TextDataMasking
{
    public class DatabaseMasker
    {
        protected string connectionString = string.Empty;
        protected MaskDictionary maskDictionary = new MaskDictionary();

        public DatabaseMasker(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public void MaskData()
        {

        }

        public static string MaskText(string originalText, DatabaseMaskerOptions options, MaskDictionary maskDictionary)
        {
            StringBuilder replacementText = new StringBuilder();

            string pattern = @"(\w+)";
            List<int> matchIndexes = new List<int>();
            List<int> matchLength = new List<int>();
            List<Tuple<int, int>> angleBracketIndexes = new List<Tuple<int, int>>();

            foreach (Match match in Regex.Matches(originalText, pattern, RegexOptions.IgnoreCase))
            {
                
                bool IsWithinAngleBracketedTag = false;
                if (options.IgnoreAngleBracketedTags)
                {
                    var lastAngleBracket = angleBracketIndexes.LastOrDefault();

                    if (lastAngleBracket != null && match.Index >= lastAngleBracket.Item1 && match.Index <= lastAngleBracket.Item2)
                    {
                        //Already within tag
                        IsWithinAngleBracketedTag = true;
                    }

                    if (!IsWithinAngleBracketedTag && match.Index > 0)
                    {
                        //Determine if within tag
                        string preceedingChar1 = originalText.Substring(match.Index - 1, 1);

                        if (preceedingChar1 == "<")
                        {
                            int bracketEndIndex = originalText.IndexOf(">", match.Index);
                            int bracketStartIndex = originalText.IndexOf("<", match.Index);

                            if (bracketEndIndex > 0 && (bracketEndIndex < bracketStartIndex || bracketStartIndex < 0))
                            {
                                IsWithinAngleBracketedTag = true;
                                angleBracketIndexes.Add(new Tuple<int, int>(match.Index - 1, bracketEndIndex));
                            }
                        }
                    }

                    if (!IsWithinAngleBracketedTag && match.Index > 1)
                    {
                        //Determine if within tag
                        string preceedingChar2 = originalText.Substring(match.Index - 2, 2);

                        if (preceedingChar2 == "</" || preceedingChar2 == "<!")
                        {
                            int bracketEndIndex = originalText.IndexOf(">", match.Index);
                            int bracketStartIndex = originalText.IndexOf("<", match.Index);

                            if (bracketEndIndex > 0 && (bracketEndIndex < bracketStartIndex || bracketStartIndex < 0))
                            {
                                IsWithinAngleBracketedTag = true;
                                angleBracketIndexes.Add(new Tuple<int, int>(match.Index - 2, bracketEndIndex));
                            }
                        }
                    }
                }

                int nonMatchStartIndex = 0;
                int nonMatchLength = 0;
                if (matchIndexes.Count == 0)
                    nonMatchLength = match.Index;
                else if (matchIndexes.Count > 0)
                {
                    nonMatchStartIndex = matchIndexes.Last() + matchLength.Last();
                    nonMatchLength = match.Index - nonMatchStartIndex;
                }
                
                if (nonMatchLength > -1)
                {
                    string preceedingNonMatchingString = originalText.Substring(nonMatchStartIndex, nonMatchLength);
                    replacementText.Append(preceedingNonMatchingString);
                }

                if (options.IgnoreAngleBracketedTags && IsWithinAngleBracketedTag)
                {
                    replacementText.Append(match.Value);
                }
                else
                {
                    replacementText.Append(maskDictionary.GetReplacement(match.Value));
                }
                
                matchIndexes.Add(match.Index);
                matchLength.Add(match.Value.Length);
            }

            int finalNonMatchStartIndex = matchIndexes.Last() + matchLength.Last();
            if (finalNonMatchStartIndex < originalText.Length)
                replacementText.Append(originalText.Substring(finalNonMatchStartIndex));

            return replacementText.ToString();
        }
    }
}
