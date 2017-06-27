using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Censure
{
    internal class Censor
    {
        internal List<string> CensoredWords { get; set; }


        internal Censor(List<string> censoredWords = null)
        {
            CensoredWords = censoredWords ?? new List<string>();
        }

        internal string cenzor(string cenz)
        {
            return CensorText(cenz);
        }  


        private string CensorText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var censoredText = text;

            foreach (string censoredWord in CensoredWords)
            {
                var regularExpression = ToRegexPattern(censoredWord);

                censoredText = Regex.Replace(censoredText, regularExpression, StarCensoredMatch,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            return censoredText;
        }

        static string StarCensoredMatch(Group m)
        {
            return new string('*', m.Captures[0].Value.Length);
        }


        static string ToRegexPattern(string wildcardSearch)
        {
            var regexPattern = Regex.Escape(wildcardSearch);

            regexPattern = regexPattern.Replace(@"\*", ".*?");
            regexPattern = regexPattern.Replace(@"\?", ".");

            if (regexPattern.StartsWith(".*?", StringComparison.Ordinal))
            {
                regexPattern = regexPattern.Substring(3);
                regexPattern = @"(^\b)*?" + regexPattern;
            }

            regexPattern = @"\b" + regexPattern + @"\b";

            return regexPattern;
        }
                                                  

    }
}
