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
        protected const int alternatesCount = 100;
        protected Random random = new Random();
        protected static readonly IReadOnlyList<char> vowels = (new List<char>() { 'a', 'e', 'i', 'o', 'u' }).AsReadOnly();
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
                StringBuilder replacementWord = new StringBuilder();
                while (replacementWord.Length < originalWord.Length)
                {
                    replacementWord.Append(random.Next(10));
                }
                return replacementWord.ToString();
            }
            else if (Regex.IsMatch(originalWord, @"^\w+$")
                     && Regex.IsMatch(originalWord, @"\d+"))
            {
                StringBuilder replacementWord = new StringBuilder();

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
                        replacementWord.Append(randomInt);
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
                        replacementWord.Append(randomChar);
                    }
                }

                return replacementWord.ToString();
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
