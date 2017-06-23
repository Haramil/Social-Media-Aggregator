using InstagramSearcher;
using SearchLibrary;
using System.Collections.Generic;
using VKSearcher;
using System.Linq;
using System.Threading;
using TwitterSearcher;

namespace AggregatorServer.Models
{
    public class AggregatorModel
    {
        private Thread instThread, vkThread, twitterThread;

        public SearchResult Search(string query)
        {
            SearchResult searchResult = new SearchResult();
            searchResult.Query = query;

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();
            TwitterSearch twitter = new TwitterSearch();

            instThread = new Thread(() => searchResult.InstPagination = instagram.Search(query, searchResult.Posts, ""));
            vkThread = new Thread(() => searchResult.VKPagination = vk.Search(query, searchResult.Posts, ""));
            twitterThread = new Thread(() => searchResult.TwitterPagination = twitter.Search(query, searchResult.Posts, ""));

            instThread.Start();
            vkThread.Start();
            twitterThread.Start();
            instThread.Join();
            vkThread.Join();
            twitterThread.Join();

            searchResult.Posts = searchResult.Posts.OrderByDescending(p => p.Date).ToList();

            return searchResult;
        }

        public SearchResult More(string query, string vkPageInfo, string instPageInfo, string twitterPageInfo)
        {
            SearchResult searchResult = new SearchResult();
            searchResult.Query = query;

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();
            TwitterSearch twitter = new TwitterSearch();

            instThread = new Thread(() => searchResult.InstPagination = instPageInfo == "" 
                ? instPageInfo 
                : instagram.Search(query, searchResult.Posts, instPageInfo));
            vkThread = new Thread(() => searchResult.VKPagination = vkPageInfo == ""
                ? vkPageInfo
                : vk.Search(query, searchResult.Posts, vkPageInfo));
            twitterThread = new Thread(() => searchResult.TwitterPagination = twitterPageInfo == ""
                ? twitterPageInfo
                : twitter.Search(query, searchResult.Posts, twitterPageInfo));

            instThread.Start();
            vkThread.Start();
            twitterThread.Start();
            instThread.Join();
            vkThread.Join();
            twitterThread.Join();

            searchResult.Posts = searchResult.Posts.OrderByDescending(p => p.Date).ToList();

            return searchResult;
        }
    }

    public class SearchResult
    {
        public string Query { get; set; }
        public List<GeneralPost> Posts { get; set; } = new List<GeneralPost>();
        public string VKPagination { get; set; }
        public string InstPagination { get; set; }
        public string TwitterPagination { get; set; }
    }
}