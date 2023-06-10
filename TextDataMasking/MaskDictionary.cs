using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextDataMasking
{
    public class MaskDictionary
    {
        private const int alternatesCount = 100;
        private static Random random = new Random();
        private static readonly IReadOnlyList<char> vowels = (new List<char>() { 'a', 'e', 'i', 'o', 'u' }).AsReadOnly();
        protected Dictionary<int, List<string>> replacementDictionary = new Dictionary<int, List<string>>();
        
        public MaskDictionary()
        {
        }

        protected List<string> GenerateReplacementWords(int length)
        {
            List<string> generatedWords = new List<string>();

            for (int j = 0; j < alternatesCount; j++)
            {
                List<char> charCollection = new List<char>();

                while (charCollection.Count < length)
                {
                    int charCodeIndex = random.Next(26);
                    char randomChar = Convert.ToChar(97 + charCodeIndex);

                    bool isVowel = vowels.Contains(randomChar);
                    if (charCollection.Count > 0)
                    {
                        char prevChar = charCollection.Last();
                        bool isPrevVowel = vowels.Contains(prevChar);
                        if (charCollection.Count == 1
                            && ((isPrevVowel && isVowel) || (!isPrevVowel && !isVowel)))
                        {
                            continue;
                        }
                        else if (!isPrevVowel && !isVowel)
                        {
                            if (charCollection.Count > 2)
                            {
                                char prevPrevChar = charCollection[charCollection.Count - 2];
                                if (!vowels.Contains(prevPrevChar))
                                    continue;
                            }
                        }
                    }

                    charCollection.Add(randomChar);
                }

                generatedWords.Add(string.Join("", charCollection));
            }

            return generatedWords;
        }

        public string GetReplacement(string originalWord)
        {
            if (Regex.IsMatch(originalWord, @"^\d+$"))
            {
                List<int> originalChars = originalWord.ToCharArray().Select(x => int.Parse(x.ToString())).ToList();
                List<int> replacementChars = new List<int>();
                for (int i = 0; i < originalChars.Count; i++)
                {
                    int originalChar = originalChars[i];
                    int replacementChar = random.Next(10);
                    while (originalChar == replacementChar)
                    {
                        replacementChar = random.Next(10);
                    }
                    replacementChars.Add(replacementChar);
                }
                string replacementWord = string.Join("", replacementChars.Select(x => x.ToString()[0]).ToList());
                return replacementWord;
            }
            else if (Regex.IsMatch(originalWord, @"^\w+$")
                     && Regex.IsMatch(originalWord, @"\d+"))
            {
                StringBuilder replacementChars = new StringBuilder();

                List<string> originalChars =
                    originalWord
                    .ToCharArray()
                    .Select(x => x.ToString())
                    .ToList();

                for (int c = 0; c < originalChars.Count; c++)
                {
                    string oChar = originalChars[c];
                    bool isVowel = vowels.Contains(oChar[0]);

                    if (Regex.IsMatch(oChar, @"^\d$"))
                    {
                        int randomInt = random.Next(10);
                        replacementChars.Append(randomInt);
                    }
                    else
                    {
                        char randomChar = ' ';
                        bool isVowel2 = !isVowel;
                        while (randomChar == ' ' || isVowel != isVowel2)
                        {
                            int charCodeIndex = random.Next(26);
                            randomChar = Convert.ToChar(97 + charCodeIndex);
                            isVowel2 = vowels.Contains(randomChar);
                        }
                        replacementChars.Append(randomChar);
                    }
                }

                string replacementWord = replacementChars.ToString();
                return replacementWord;
            }

            List<string> replacements = new List<string>();

            if (replacementDictionary.ContainsKey(originalWord.Length))
            {
                replacements = replacementDictionary[originalWord.Length];
            }
            else if (originalWord.Length == 1)
            {
                replacements = vowels.Select(x => x.ToString()).ToList();
                replacementDictionary.Add(1, replacements);
            }
            else
            {
                replacements = GenerateReplacementWords(originalWord.Length);

                replacementDictionary.Add(originalWord.Length, replacements);
            }

            string returnValue = originalWord;

            while (originalWord == returnValue)
            {
                int replacementIndex = random.Next(replacements.Count);
                returnValue = replacements[replacementIndex];
            }

            return returnValue;
        }
    }
}
