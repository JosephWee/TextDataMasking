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
                        if (originalWord.Length > 26 || !replacementWord.Contains(randomChar))
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
