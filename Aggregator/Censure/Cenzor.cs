using System.Collections.Generic;

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
    }
}
