using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Censure
{
    public class Cenzor
    {

        public string Cenz(string text, List<string> dict)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            Censor cenz = new Censor(dict);
            return cenz.cenzor(text);
        }

        public bool HasCensuredWord(string text, string filewithwords)
        {
            Censor cenz = new Censor(File.ReadAllLines(filewithwords, Encoding.GetEncoding("windows-1251")).ToList());
            return cenz.HasCensoredWord(text);
        }

    }
}
