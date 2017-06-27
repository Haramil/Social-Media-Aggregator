using System.Collections.Generic;

namespace SearchLibrary
{
    public interface ISearcher
    {
        string Search(string query, List<GeneralPost> searchResult, string pageInfo, List<string> dict);
    }
}
