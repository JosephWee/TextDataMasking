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
        protected const int alternatesCount = 30;
        protected Random random = new Random();
        protected static readonly IReadOnlyList<char> vowels = (new List<char>() { 'a', 'e', 'i', 'o', 'u' }).AsReadOnly();
        protected static readonly IReadOnlyList<int> vowelCodes = vowels.Select(x => (int)Convert.ToByte(x)).ToList().AsReadOnly();
        protected Dictionary<int, List<string>> replacementDictionary = new Dictionary<int, List<string>>();
        
        public MaskDictionary()
        {
        }

        public string GetReplacement(string originalWord)
        {
            if (Regex.IsMatch(originalWord, @"\d"))
            {
                StringBuilder replacementWord = new StringBuilder();
                while (replacementWord.Length < originalWord.Length)
                {
                    replacementWord.Append(random.Next(10));
                }
                return replacementWord.ToString();
            }

            List<string> replacements = new List<string>();

            if (replacementDictionary.ContainsKey(originalWord.Length))
            {
                replacements = replacementDictionary[originalWord.Length];
            }
            else
            {
                for (int j = 0; j < alternatesCount; j++)
                {
                    List<char> replacementWord = new List<char>();
                    
                    while (replacementWord.Count < originalWord.Length)
                    {
                        int charCodeIndex = random.Next(26);
                        char randomChar = Convert.ToChar(97 + charCodeIndex);

                        bool isVowel = vowels.Contains(randomChar);
                        if (replacementWord.Count > 0)
                        {
                            char prevChar = replacementWord.Last();
                            bool isPrevVowel = vowels.Contains(prevChar);
                            if (replacementWord.Count == 1
                                && ((isPrevVowel && isVowel) || (!isPrevVowel && !isVowel)))
                            {
                                continue;
                            }
                            else if (!isPrevVowel && !isVowel)
                            {
                                if (replacementWord.Count > 2)
                                {
                                    char prevPrevChar = replacementWord[replacementWord.Count - 2];
                                    if (!vowels.Contains(prevPrevChar))
                                        continue;
                                }
                            }
                        }

                        replacementWord.Add(randomChar);
                    }

                    replacements.Add(string.Join("", replacementWord));
                }

                replacementDictionary.Add(originalWord.Length, replacements);
            }

            int replacementIndex = random.Next(replacements.Count);
            
            return replacements[replacementIndex];
        }
    }
}
