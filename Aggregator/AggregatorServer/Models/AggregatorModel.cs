using Censure;
using InstagramSearcher;
using SearchLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TwitterSearcher;
using VKSearcher;
using System.Web;

namespace AggregatorServer.Models
{
    public class AggregatorModel
    {
        private static List<string> dict = Regex.Split(Properties.Resources.badwords, @"\r?\n|\r").ToList();

        private Thread instThread, vkThread, twitterThread;

        public SearchResult Search(string query)
        {
            SearchResult searchResult = new SearchResult();
            searchResult.Query = HttpUtility.UrlEncode(query);

            Cenzor cenzor = new Cenzor();

            if (query != cenzor.Cenz(query, dict))
                return searchResult;

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();
            TwitterSearch twitter = new TwitterSearch();

            instThread = new Thread(() => searchResult.InstPagination = instagram.Search(query, searchResult.Posts, "", dict));
            vkThread = new Thread(() => searchResult.VKPagination = vk.Search(query, searchResult.Posts, "", dict));
            twitterThread = new Thread(() => searchResult.TwitterPagination = twitter.Search(query, searchResult.Posts, "", dict));

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
            searchResult.Query = HttpUtility.UrlEncode(query);

            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();
            TwitterSearch twitter = new TwitterSearch();

            instThread = new Thread(() => searchResult.InstPagination = string.IsNullOrEmpty(instPageInfo)
                ? instPageInfo 
                : instagram.Search(query, searchResult.Posts, instPageInfo, dict));
            vkThread = new Thread(() => searchResult.VKPagination = string.IsNullOrEmpty(vkPageInfo)
                ? vkPageInfo
                : vk.Search(query, searchResult.Posts, vkPageInfo, dict));
            twitterThread = new Thread(() => searchResult.TwitterPagination = string.IsNullOrEmpty(twitterPageInfo)
                ? twitterPageInfo
                : twitter.Search(query, searchResult.Posts, twitterPageInfo, dict));

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