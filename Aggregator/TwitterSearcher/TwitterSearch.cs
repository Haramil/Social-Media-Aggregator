using Newtonsoft.Json;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using TwitterSearcher.Classes;
using TwitterSearcher.Services;

namespace TwitterSearcher
{
    public class TwitterSearch : ISearcher
    {
        private string searchurl = "https://twitter.com/i/search/timeline";
        private string Query = "q";
        private string RealTime = "f";
        private string Src = "src";
        private string IncludeFeatures = "include_available_features";
        private string IncludeEntities = "include_entities";
        private string LastNodeTs = "last_note_ts";
        private string MaxPosition = "max_position";
        internal HtmlService htmlService { get; set; }

        public TwitterSearch()
        {
            htmlService = new HtmlService();
        }

        public string Search(string query, List<GeneralPost> posts, string info)
        {

            var url = GetUrl(query, info);
            var twitterresponse = GetTwitterResponse(url);
            lock (posts)
            {
                posts.AddRange(htmlService.GetTweets(twitterresponse.Items_html));
            }
            if (twitterresponse.HasMoreItems)
                return twitterresponse.MinPosition;
            else return "";
        }

        #region privatesection
        private string GetUrl(string query, string info)
        {

            var uriBuilder = new UriBuilder(searchurl);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters[RealTime] = "realtime";
            parameters[Query] = "%23" + query;
            parameters[Src] = "typd";
            parameters[IncludeFeatures] = "1";
            parameters[IncludeEntities] = "1";
            parameters[LastNodeTs] = "85";
            parameters[MaxPosition] = info;
            uriBuilder.Query = parameters.ToString();
            Uri finalUrl = uriBuilder.Uri;
            return finalUrl.AbsoluteUri;

        }

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
