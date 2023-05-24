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

        public static string MaskText(MaskDictionary maskDictionary, string orginalText)
        {
            StringBuilder replacementText = new StringBuilder();

            string pattern = @"(\W*|\s*|\d*)(\w+)(\W*|\s*|\d*)";
            foreach (Match match in Regex.Matches(orginalText, pattern, RegexOptions.IgnoreCase))
            {
                if (match.Groups.Count > 3)
                {
                    string originalWord = match.Groups[2].Value;
                    string replacementWord = maskDictionary.GetReplacement(originalWord);
                    string replacementValue = match.Value.Replace(originalWord, replacementWord);
                    replacementText.Append(replacementValue);
                }
                else
                {
                    replacementText.Append(match.Value);
                }
            }

            return replacementText.ToString();
        }
    }
}
