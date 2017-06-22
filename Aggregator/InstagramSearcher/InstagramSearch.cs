using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramSearcher
{
    public class InstagramSearch : ISearcher
    {
        private void PostSearch(dynamic instagramPost, List<GeneralPost> searchResult)
        {
            GeneralPost newPost = new GeneralPost();

            newPost.Text = instagramPost.caption;
            newPost.Image = instagramPost.display_src;
            double sec = instagramPost.date;
            newPost.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(sec);
            newPost.PostLink = "https://www.instagram.com/p/" + instagramPost.code;

            var request = (HttpWebRequest)WebRequest.Create(newPost.PostLink + "/?__a=1");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic instPostData = JsonConvert.DeserializeObject(responseString);
            var instAuthor = instPostData.graphql.shortcode_media.owner;

            newPost.AuthorName = instAuthor.username;
            newPost.AuthorAvatar = instAuthor.profile_pic_url;
            newPost.Social = SocialMedia.Instagram;

            lock (searchResult)
            {
                searchResult.Add(newPost);
            }
        }

        public string Search(string query, List<GeneralPost> searchResult, string pageInfo)
        {
            if (pageInfo != "") pageInfo = "&max_id=" + pageInfo;

            var request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/explore/tags/" + query + "/?__a=1" + pageInfo);

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException) { return null; }

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic instData = JsonConvert.DeserializeObject(responseString);
            var inst = instData.tag.media;

            List<Thread> postThreads = new List<Thread>();
            foreach (var instElem in inst.nodes)
            {
                Thread postThread = new Thread(() => PostSearch(instElem, searchResult));
                postThreads.Add(postThread);
                postThread.Start();
            }

            postThreads.ForEach(t => t.Join());

            string page = inst.page_info.end_cursor;
            return page;
        }
    }
}
