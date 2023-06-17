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

        public class AngleTagType
        {
            public string BracketStart { get; set; } = string.Empty;
            public string BracketEnd { get; set; } = string.Empty;
            public int MaxOffSet_Left { get; set; } = int.MaxValue;
        }

        public class AngledTagScope
        {
            public AngleTagType AngledTagType { get; set; }
            public int StartIndex { get; set; } = -1;
            public int EndIndex { get; set; } = -1;
        }

        private static List<AngleTagType> angledTagTypes = new List<AngleTagType>()
        {
            // Comments
            new AngleTagType
            {
                BracketStart = "<!--",
                BracketEnd = "-->",
                MaxOffSet_Left = int.MaxValue
            },
            // CDATA
            new AngleTagType
            {
                BracketStart = "<![CDATA[",
                BracketEnd = "]]>",
                MaxOffSet_Left = 3
            },
            // XHTML DOCTYPE Declaration
            new AngleTagType
            {
                BracketStart = "<!DOCTYPE",
                BracketEnd = ">",
                MaxOffSet_Left = 2
            },
            // Declaration and Processing
            new AngleTagType
            {
                BracketStart = "<?",
                BracketEnd = "?>",
                MaxOffSet_Left = 2
            },
            // End Tag
            new AngleTagType
            {
                BracketStart = "</",
                BracketEnd = ">",
                MaxOffSet_Left = 2
            },
            // Empty Element Tag
            new AngleTagType
            {
                BracketStart = "<",
                BracketEnd = "/>",
                MaxOffSet_Left = 1
            },
            // Start Tag
            new AngleTagType
            {
                BracketStart = "<",
                BracketEnd = ">",
                MaxOffSet_Left = 1
            }
        };

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
            List<AngledTagScope> angledBracketScopes = new List<AngledTagScope>();
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

                bool IsWithinAngledTagScope = false;
                if (options.IgnoreAngleBracketedTags)
                {
                    var lastAngleTagScope = angledBracketScopes.LastOrDefault();

                    if (lastAngleTagScope != null && match.Index > lastAngleTagScope.StartIndex && match.Index < lastAngleTagScope.EndIndex)
                    {
                        //Already within tag
                        IsWithinAngledTagScope = true;
                    }

                    if (!IsWithinAngledTagScope)
                    {
                        for (int tagTypeIndex = 0; tagTypeIndex < angledTagTypes.Count; tagTypeIndex++)
                        {
                            AngleTagType angledTagType = angledTagTypes[tagTypeIndex];
                            int bracketStartIndex = -1;
                            string bracketStart = string.Empty;

                            if (match.Index >= angledTagType.MaxOffSet_Left)
                            {
                                bracketStartIndex = match.Index - angledTagType.MaxOffSet_Left;
                                if (bracketStartIndex >= 0 && originalText.Length - bracketStartIndex >= angledTagType.BracketStart.Length)
                                {
                                    bracketStart =
                                        originalText.Substring(
                                            bracketStartIndex,
                                            angledTagType.BracketStart.Length);
                                }
                            }
                            else if (angledTagType.MaxOffSet_Left == int.MaxValue)
                            {
                                bracketStartIndex =
                                    originalText
                                    .LastIndexOf(
                                        angledTagType.BracketStart,
                                        match.Index);

                                if (bracketStartIndex >= 0)
                                {
                                    bracketStart =
                                        originalText.Substring(
                                        bracketStartIndex,
                                        angledTagType.BracketStart.Length);

                                    int spaceBetweenStartIndex =
                                        bracketStartIndex
                                        + angledTagType.BracketStart.Length;

                                    string spaceBetween =
                                        originalText.Substring(
                                            spaceBetweenStartIndex,
                                            match.Index - spaceBetweenStartIndex);

                                    // Confirm that there is only empty space between
                                    // the match and the start tag
                                    if (!string.IsNullOrWhiteSpace(spaceBetween))
                                    {
                                        bracketStartIndex = -1;
                                        bracketStart = string.Empty;
                                    }
                                }
                            }

                            if (bracketStartIndex >= 0 && bracketStart == angledTagType.BracketStart)
                            {
                                int bracketEndIndex =
                                    originalText.IndexOf(
                                        angledTagType.BracketEnd,
                                        bracketStartIndex + angledTagType.BracketStart.Length);

                                if (bracketEndIndex > bracketStartIndex)
                                {
                                    bracketEndIndex += angledTagType.BracketEnd.Length;
#if DEBUG
                                    string scopeString = originalText.Substring(bracketStartIndex, bracketEndIndex - bracketStartIndex);
#endif
                                    IsWithinAngledTagScope = true;
                                    AngledTagScope newScope =
                                        new AngledTagScope()
                                        {
                                            AngledTagType = angledTagType,
                                            StartIndex = bracketStartIndex,
                                            EndIndex = bracketEndIndex
                                        };
                                    angledBracketScopes.Add(newScope);
                                    break;
                                }
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

                if (options.IgnoreAngleBracketedTags && IsWithinAngledTagScope)
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
                else if (options.IgnoreAlphaNumeric && Regex.IsMatch(match.Value, @"[A-Za-z]+") && Regex.IsMatch(match.Value, @"[0-9]+") && !Regex.IsMatch(match.Value, @"\W+"))
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
                                            endIndex = f + 1;
                                            break;
                                        }
                                        else if (Regex.IsMatch(character, @"^[.]$"))
                                        {
                                            periodCount2++;
                                            if (periodCount2 > 1)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f + 1;
                                                break;
                                            }

                                            int index = originalText.IndexOf(".", match.Index);
                                            if (index > 0 && index != match.Index + match.Value.Length)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f + 1;
                                                break;
                                            }
                                        }
                                        else if (Regex.IsMatch(character, @"^[0-9]$"))
                                        {
                                            if (periodCount2 < 1)
                                            {
                                                HasNonSpaceAfter = true;
                                                endIndex = f + 1;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            HasNonSpaceAfter = true;
                                            endIndex = f + 1;
                                            break;
                                        }
                                    }
                                }
                            }
#if DEBUG
                            if (startIndex == int.MinValue)
                                startIndex = match.Index;
                            if (endIndex == int.MaxValue)
                                endIndex = match.Index + match.Length;
                            string scopeString = originalText.Substring(startIndex, endIndex - startIndex);
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
