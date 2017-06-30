using AngleSharp.Dom;
using Censure;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TwitterSearcher.Services
{
    internal class HtmlService
    {
        List<GeneralPost> posts = new List<GeneralPost>();

        public List<GeneralPost> GetTweets(string html, List<string> dict)
        {
            try
            {
                AngleSharp.Parser.Html.HtmlParser parser = new AngleSharp.Parser.Html.HtmlParser();
                AngleSharp.Dom.Html.IHtmlDocument htmldocument = parser.Parse(html);
                var list = new List<string>();
                var items = htmldocument.QuerySelectorAll("li").Where(item => item.ClassName != null && item.ClassName.Contains("stream-item") && !item.ClassName.Contains("AdaptiveStream"));

                List<Thread> postThreads = new List<Thread>();
                foreach (var item in items)
                {
                    Thread postThread = new Thread(() => PostSearch(item, dict));
                    postThreads.Add(postThread);
                    postThread.Start();
                }
                postThreads.ForEach(t => t.Join());
                return posts;
            }
            catch
            {
                return new List<GeneralPost>();
            }
        }

        private void PostSearch(IElement item, List<string> dict)
        {
            try
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

                Cenzor cenzor = new Cenzor();
                tweet.Text = cenzor.Cenz(tweet.Text, dict);

                tweet.Social = SocialMedia.Twitter;
                tweet.AuthorName = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("tweet")).First().Attributes["data-name"].Value;
                string linkname = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("tweet")).First().Attributes["data-screen-name"].Value;
                tweet.PostLink = "https://twitter.com/" + linkname + "/status/" + id;
                tweet.AuthorLink = "https://twitter.com/" + linkname;
                tweet.AuthorAvatar = item.QuerySelectorAll("img").Where(y => y.ClassName.Contains("avatar")).First().Attributes["src"].Value;
                try
                {
                    var elemwithdate = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("content")).First().QuerySelectorAll("div").Where(o => o.ClassName.Contains("stream-item-header")).First().QuerySelectorAll("small").Where(u => u.ClassName.Contains("time")).First().QuerySelectorAll("a").Where(f => f.ClassName.Contains("tweet-timestamp")).First().Attributes["title"].Value;
                    var massivstrdate = elemwithdate.Split('-');
                    var massivyearmohtn = massivstrdate[1].Split(' ');
                    var h1 = massivstrdate[0].TrimEnd(' ');
                    var h2 = massivyearmohtn[1];
                    var h3 = massivyearmohtn[2];
                    var h4 = massivyearmohtn[3];
                    var d = item.QuerySelectorAll("div").Where(k => k.ClassName.Contains("content")).First().QuerySelectorAll("div").Where(o => o.ClassName.Contains("stream-item-header")).First().QuerySelectorAll("small").Where(u => u.ClassName.Contains("time")).First().QuerySelectorAll("a").Where(f => f.ClassName.Contains("tweet-timestamp")).First().QuerySelectorAll("span").Where(p => p.ClassName.Contains("_timestamp")).First().Attributes["data-time-ms"].Value;
                    var s1 = h1.Split(':');
                    tweet.Date = (new DateTime(Int32.Parse(h4), getMonth(h3), Int32.Parse(h2), Int32.Parse(s1[0]), Int32.Parse(s1[1]), 0));
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
            } catch { }
        }

        public int getMonth(string value)
        {
            switch (value)
            {
                case "июн.":
                    return 6;
                case "мая":
                    return 5;
                case "апр.":
                    return 4;
                case "мар.":
                    return 3;
                case "июл.":
                    return 7;
                case "авг.":
                    return 8;
                case "сент.":
                    return 9;
                case "окт.":
                    return 10;
                case "нояб.":
                    return 11;
                case "дек.":
                    return 12;
                case "янв.":
                    return 1;
                case "февр.":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
