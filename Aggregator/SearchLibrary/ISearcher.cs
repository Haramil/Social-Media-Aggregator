using System.Collections.Generic;

namespace SearchLibrary
{
    public interface ISearcher
    {
        void Search(string query, List<GeneralPost> searchResult);
    }
}
