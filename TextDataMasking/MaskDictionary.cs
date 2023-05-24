using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<string> GetReplacements(IEnumerable<string> originalWords)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < originalWords.Count(); i++)
            {
                var originalWord = originalWords.ElementAt(i);

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
                            if (!replacementWord.Contains(randomChar))
                                replacementWord.Add(randomChar);
                        }

                        replacements.Add(string.Join("", replacementWord));
                    }

                    replacementDictionary.Add(originalWord.Length, replacements);
                }

                int replacementIndex = random.Next(replacements.Count);
                result.Add(replacements[replacementIndex]);
            }

            return result;
        }
    }
}
