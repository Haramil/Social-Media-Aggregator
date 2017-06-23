using SearchLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using System.Threading;

namespace TwitterSearcher.Services
{
    internal class HtmlService
    {
        List<GeneralPost> posts = new List<GeneralPost>();

        public List<GeneralPost> GetTweets(string html)
        {
            AngleSharp.Parser.Html.HtmlParser parser = new AngleSharp.Parser.Html.HtmlParser();
            AngleSharp.Dom.Html.IHtmlDocument htmldocument = parser.Parse(html);
            var list = new List<string>();
            var items = htmldocument.QuerySelectorAll("li").Where(item => item.ClassName != null && item.ClassName.Contains("stream-item")&&!item.ClassName.Contains("AdaptiveStream"));

            List<Thread> postThreads = new List<Thread>();
            foreach (var item in items)
            {
                Thread postThread = new Thread(() => PostSearch(item));
                postThreads.Add(postThread);
                postThread.Start();
            }
            postThreads.ForEach(t => t.Join());
            return posts;
        }

        private void PostSearch(IElement item)
        {
            GeneralPost tweet = new GeneralPost();
            var h = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("AdaptiveMediaOuterContainer"));
            if (!(h.Count() == 0))
            {
                if (h.First().QuerySelectorAll("img").Count() != 0)
                    tweet.Image = h.First().QuerySelectorAll("img").First().Attributes["src"].Value;
            }
            long id = long.Parse(item.Attributes["data-item-id"].Value);
            tweet.Text = item.QuerySelectorAll("p").Where(k => k.ClassName.Contains("tweet-text")).First().InnerHtml;
            tweet.Social = SocialMedia.Twitter;
            tweet.AuthorName = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("tweet")).First().Attributes["data-name"].Value;
            string linkname = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("tweet")).First().Attributes["data-screen-name"].Value;
            tweet.PostLink = "https://twitter.com/" + linkname + "/status/" + id;
            tweet.AuthorAvatar = item.QuerySelectorAll("img").Where(y => y.ClassName.Contains("avatar")).First().Attributes["src"].Value;
            try
            {
                var d = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("content")).First().QuerySelectorAll("div").Where(o => o.ClassName.Contains("stream-item-header")).First().QuerySelectorAll("small").Where(u => u.ClassName.Contains("time")).First().QuerySelectorAll("a").Where(f => f.ClassName.Contains("tweet-timestamp")).First().QuerySelectorAll("span").Where(p => p.ClassName.Contains("_timestamp")).First().Attributes["data-time-ms"].Value;
                tweet.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(long.Parse(d)).ToLocalTime();
            }
            catch { }
            AngleSharp.Parser.Html.HtmlParser parser = new AngleSharp.Parser.Html.HtmlParser();
            AngleSharp.Dom.Html.IHtmlDocument htmldocument = parser.Parse(tweet.Text);

            var links = htmldocument.QuerySelectorAll("a");
            
            foreach (var link in links)
                tweet.Text = tweet.Text.Replace(link.OuterHtml, link.InnerHtml);

            

            lock (posts)
            {
                posts.Add(tweet);
            }
        }
    }
}
