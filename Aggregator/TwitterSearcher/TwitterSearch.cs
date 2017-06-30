using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TwitterSearcher.Classes;
using TwitterSearcher.Services;

namespace TwitterSearcher
{
    public class TwitterSearch : ISearcher
    {
        private string searchurl = "https://twitter.com/i/search/timeline%23";
        internal HtmlService htmlService { get; set; }

        public TwitterSearch()
        {
            htmlService = new HtmlService();
        }

        public string Search(string query, List<GeneralPost> posts, string info, List<string> dict)
        {
            try
            {
                var url = "https://twitter.com/i/search/timeline?f=realtime&q=%23" + query + "&src=typd&max_position=" + info;
                var twitterresponse = GetTwitterResponse(url);

                if (twitterresponse == null) return "";

                lock (posts)
                {
                    posts.AddRange(htmlService.GetTweets(twitterresponse.Items_html, dict));
                }
                if (twitterresponse.MinPosition != "" && twitterresponse.MinPosition != info)
                    return twitterresponse.MinPosition;
                else return "";
            }
            catch
            {
                return "";
            }
        }

        #region privatesection
        private TweetResponse GetTwitterResponse(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            request.Method = "GET";
            request.ContentType = "application/json";

            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException) { return null; }

            string temp = "";
            StringBuilder sb = new StringBuilder();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                // use whatever method you want to save the data to the file...
                temp = reader.ReadToEnd();
                sb.Append(temp);
            }
            string y = JsonConvert.DeserializeObject(sb.ToString()).ToString();
            JSONWorker jsonworker = new JSONWorker();
            return jsonworker.getReponse(sb.ToString());
        }
        #endregion

    }
}
