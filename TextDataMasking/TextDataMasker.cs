using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace TextDataMasking
{
    public class TextDataMasker
    {
        private static Random random = new Random();

#if DEBUG
        private static int numStartWithZero = 0;
        private static int replacementEqualsOriginal = 0;
#endif
        private static List<string> GetAllAttributeNames(JsonElement jsonElement)
        {
            List<string> retValue = new List<string>();

            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var jsonProperty in jsonElement.EnumerateObject())
                {
                    retValue.Add(jsonProperty.Name.Trim());
                    var childAttributeNames = GetAllAttributeNames(jsonProperty.Value);
                    retValue.AddRange(childAttributeNames);
                }
            }
            else if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in jsonElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object
                        || item.ValueKind == JsonValueKind.Array)
                    {
                        var childAttributeNames = GetAllAttributeNames(item);
                        retValue.AddRange(childAttributeNames);
                    }
                }
            }

            return retValue;
        }

        public static string MaskText(string originalText, DataMaskerOptions options, MaskDictionary maskDictionary)
        {
            if (string.IsNullOrWhiteSpace(originalText))
                return originalText;

            StringBuilder replacementText = new StringBuilder();

            string pattern = @"(\w+)";
            List<int> matchIndexes = new List<int>();
            List<int> matchLength = new List<int>();
            List<Tuple<int, int>> angleBracketIndexes = new List<Tuple<int, int>>();
            List<Tuple<int, int>> jsonAttributePositions = new List<Tuple<int, int>>();

            bool IsJson = false;
            List<string> jsonAttributeNames = new List<string>();
            if (options.IgnoreJsonAttributes)
            {
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(originalText);
                    IsJson = true;
                    jsonAttributeNames = GetAllAttributeNames(jsonElement);
                }
                catch (Exception ex)
                {
                }
            }

#if DEBUG
            bool IsLocation = false;
            string lastAttribute = string.Empty;
#endif
            foreach (Match match in Regex.Matches(originalText, pattern, RegexOptions.IgnoreCase))
            {
                string preceedingNonMatchingString = string.Empty;

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
                    preceedingNonMatchingString = originalText.Substring(nonMatchStartIndex, nonMatchLength);
                }

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

                        if (preceedingChar2 == "</" || preceedingChar2 == "<!" || preceedingChar2 == "<?")
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

                bool IsWithinJsonAttribute = false;
                if (options.IgnoreJsonAttributes && IsJson)
                {
                    var lastJsonAttributePos = jsonAttributePositions.LastOrDefault();

                    if (lastJsonAttributePos != null && match.Index >= lastJsonAttributePos.Item1 && match.Index <= lastJsonAttributePos.Item2)
                    {
                        //Already within json attribute
                        IsWithinJsonAttribute = true;
                    }

                    if (!IsWithinJsonAttribute && match.Index > 0)
                    {
                        int lastIndexOfSingleQuote = originalText.LastIndexOf("'", match.Index);
                        int lastIndexOfDoubleQuote = originalText.LastIndexOf('"', match.Index);

                        if (lastIndexOfSingleQuote > -1 || lastIndexOfDoubleQuote > -1)
                        {
                            int lastIndexOfQuote = Math.Max(lastIndexOfSingleQuote, lastIndexOfDoubleQuote);

                            char preceedingQuote =
                                lastIndexOfQuote == lastIndexOfSingleQuote
                                ? '\''
                                : '"';

                            string textBetweenMatchAndPrevQuote = originalText.Substring(lastIndexOfQuote, match.Index - lastIndexOfQuote);

                            if (textBetweenMatchAndPrevQuote.Trim().ToCharArray().Last() == preceedingQuote)
                            {
                                int indexImmeidatelyAfterMatch = match.Index + match.Value.Length;
                                int nextIndexOfQuote = originalText.IndexOf(preceedingQuote, indexImmeidatelyAfterMatch);

                                if (nextIndexOfQuote > -1)
                                {
                                    string possibleAttributeName = originalText.Substring(lastIndexOfQuote, nextIndexOfQuote - lastIndexOfQuote + 1).Trim(preceedingQuote).Trim();

                                    if (jsonAttributeNames.Contains(possibleAttributeName))
                                    {
                                        IsWithinJsonAttribute = true;
                                        jsonAttributePositions.Add(new Tuple<int, int>(lastIndexOfQuote, nextIndexOfQuote));
                                    }
                                }
                            }
                        }
                    }
                }

                if (nonMatchLength > -1)
                {
                    //Append preceeding non-matching string
                    replacementText.Append(preceedingNonMatchingString);
                }

                if (options.IgnoreAngleBracketedTags && IsWithinAngleBracketedTag)
                {
                    replacementText.Append(match.Value);
                }
                else if (options.IgnoreJsonAttributes && IsWithinJsonAttribute)
                {
                    replacementText.Append(match.Value);
#if DEBUG
                    lastAttribute = match.Value;
                    if (lastAttribute == "location")
                        IsLocation = true;
#endif
                }
                else if (options.IgnoreNumbers && Regex.IsMatch(match.Value, @"^\d+$"))
                {
                    replacementText.Append(match.Value);
                }
                else
                {
                    var valLowerCase = match.Value.ToLower();
                    string replacement = maskDictionary.GetReplacement(match.Value);
                    while (replacement == valLowerCase)
                    {
#if DEBUG
                        ++replacementEqualsOriginal;
#endif
                        replacement = maskDictionary.GetReplacement(match.Value);
                    }

                    var originalChars = match.Value.ToList();
                    var replacementChars = replacement.ToList();

                    if (IsJson)
                    {
                        if (valLowerCase == "null")
                        {
                            replacementChars = new List<char>() { 'n', 'u', 'l', 'l' };
                        }
                        else if (valLowerCase == "true")
                        {
                            replacementChars = new List<char>() { 't', 'r', 'u', 'e' };
                        }
                        else if (valLowerCase == "false")
                        {
                            replacementChars = new List<char>() { 'f', 'a', 'l', 's', 'e' };
                        }

                        string prevJsonChar = "";
                        string nextJsonChar = "";
                        if (!IsWithinJsonAttribute
                            && !options.IgnoreNumbers
                            && Regex.IsMatch(match.Value, @"^\d+$")
                            && replacement.StartsWith("0"))
                        {
                            int startIndex = int.MinValue;
                            int endIndex = int.MaxValue;
                            bool HasNonSpaceBefore = false;
                            int minusCount = 0;
                            int periodCount1 = 0;
                            if (match.Index > 0)
                            {
                                for (int f = match.Index - 1; f >= 0; f--)
                                {
                                    string character = originalText[f].ToString();
                                    if (!Regex.IsMatch(character, @"^\s$"))
                                    {
                                        if (Regex.IsMatch(character, @"^[:]$"))
                                        {
                                            HasNonSpaceBefore = false;
                                            startIndex = f;
                                            break;
                                        }
                                        else if (Regex.IsMatch(character, @"^[.]$"))
                                        {
                                            periodCount1++;
                                            if (periodCount1 > 1)
                                            {
                                                HasNonSpaceBefore = true;
                                                startIndex = f;
                                                break;
                                            }

                                            int index = originalText.LastIndexOf(".", match.Index);
                                            if (index > 0 && index != match.Index - 1)
                                            {
                                                HasNonSpaceBefore = true;
                                                startIndex = f;
                                                break;
                                            }
                                        }
                                        else if (Regex.IsMatch(character, @"^[0-9]$"))
                                        {
                                            if (periodCount1 < 1)
                                            {
                                                HasNonSpaceBefore = true;
                                                startIndex = f;
                                                break;
                                            }
                                        }
                                        else if (Regex.IsMatch(character, @"^\-$"))
                                        {
                                            minusCount++;
                                            if (minusCount > 1)
                                            {
                                                HasNonSpaceBefore = true;
                                                startIndex = f;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            HasNonSpaceBefore = true;
                                            startIndex = f;
                                            break;
                                        }
                                    }
                                }
                            }

                            bool HasNonSpaceAfter = false;
                            int periodCount2 = 0;
                            if (match.Index < originalText.Length - 1)
                            {
                                for (int f = (match.Index + match.Length); f < originalText.Length; f++)
                                {
                                    string character = originalText[f].ToString();
                                    if (!Regex.IsMatch(character, @"^\s$"))
                                    {
                                        if (Regex.IsMatch(character, @"^[,}]$"))
                                        {
                                            HasNonSpaceAfter = false;
                                            endIndex = f;
                                            break;
                                        }
                                        else if (Regex.IsMatch(character, @"^[.]$"))
                                        {
                                            periodCount2++;
                                            if (periodCount2 > 1)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f;
                                                break;
                                            }

                                            int index = originalText.IndexOf(".", match.Index);
                                            if (index > 0 && index != match.Index + match.Value.Length)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f;
                                                break;
                                            }
                                        }
                                        else if (Regex.IsMatch(character, @"^[0-9]$"))
                                        {
                                            if (periodCount2 < 1)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            HasNonSpaceAfter = true;
                                            endIndex = f;
                                            break;
                                        }
                                    }
                                }
                            }
#if DEBUG
                            string scopeString = originalText.Substring(startIndex, endIndex - startIndex + 1);
#endif
                            if (!HasNonSpaceBefore && !HasNonSpaceAfter)
                            {
#if DEBUG
                                ++numStartWithZero;
#endif
                                UInt64 uint64 = UInt64.Parse(replacement);
                                replacementChars = uint64 == 0 ? new List<char>() : uint64.ToString().ToList();
                                int diffLength = originalChars.Count - replacementChars.Count;
                                for (int d = 0; d < diffLength; d++)
                                {
                                    replacementChars.Add((random.Next(9) + 1).ToString()[0]);
                                }
                            }
#if DEBUG
                            else
                            {
                            }
#endif
                        }
                    }

                    if (options.PreserveCase && !Regex.IsMatch(match.Value, @"^\d+$"))
                    {
                        for (int i = 0; i < originalChars.Count; i++)
                        {
                            string originalChar = originalChars[i].ToString();
                            if (originalChar.ToUpper() == originalChar)
                            {
                                replacementText.Append(
                                    replacementChars[i]
                                    .ToString()
                                    .ToUpper()
                                );
                            }
                            else
                            {
                                replacementText.Append(replacementChars[i]);
                            }
                        }
                    }
                    else
                    {
                        replacementText.Append(string.Join("", replacementChars));
                    }
                }

                matchIndexes.Add(match.Index);
                matchLength.Add(match.Value.Length);
            }

            if (matchIndexes.Any() && matchLength.Any())
            {
                int finalNonMatchStartIndex = matchIndexes.Last() + matchLength.Last();
                if (finalNonMatchStartIndex < originalText.Length)
                    replacementText.Append(originalText.Substring(finalNonMatchStartIndex));
            }

            return replacementText.ToString();
        }
    }
}
