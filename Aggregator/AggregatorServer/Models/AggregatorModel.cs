using InstagramSearcher;
using SearchLibrary;
using System.Collections.Generic;
using VKSearcher;
using System.Linq;
using System.Threading;

namespace AggregatorServer.Models
{
    public class AggregatorModel
    {
        public SearchResult Search(string query)
        {
            SearchResult searchResult = new SearchResult();
            searchResult.Query = query;

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();

            Thread instThread = new Thread(() => searchResult.InstPagination = instagram.Search(query, searchResult.Posts));
            Thread vkThread = new Thread(() => searchResult.VKPagination = vk.Search(query, searchResult.Posts));

            instThread.Start();
            vkThread.Start();
            instThread.Join();
            vkThread.Join();

            searchResult.Posts = searchResult.Posts.OrderByDescending(p => p.Date).ToList();

            return searchResult;
        }

        public SearchResult More(string query, string vkPageInfo, string instPageInfo)
        {
            SearchResult searchResult = new SearchResult();
            searchResult.Query = query;

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();

            Thread instThread = new Thread(() => searchResult.InstPagination = instagram.Search(query, searchResult.Posts, instPageInfo));
            Thread vkThread = new Thread(() => searchResult.VKPagination = vk.Search(query, searchResult.Posts, vkPageInfo));

            instThread.Start();
            vkThread.Start();
            instThread.Join();
            vkThread.Join();

            searchResult.Posts = searchResult.Posts.OrderByDescending(p => p.Date).ToList();

            return searchResult;
        }

        public class SearchResult
        {
            public string Query { get; set; }
            public List<GeneralPost> Posts { get; set; } = new List<GeneralPost>();
            public string VKPagination { get; set; }
            public string InstPagination { get; set; }
            //public string TwitterPagination { get; set; }
        }
    }
}