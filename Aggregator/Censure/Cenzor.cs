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
    }
}
