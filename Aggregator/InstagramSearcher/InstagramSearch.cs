using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;

namespace InstagramSearcher
{
    public class InstagramSearch : ISearcher
    {
        public void Search(string query, List<GeneralPost> searchResult)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/explore/tags/" + query + "/?__a=1");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic instData = JsonConvert.DeserializeObject(responseString);
            var instList = instData.tag.media.nodes;
            foreach (var instElem in instList)
            {
                GeneralPost newPost = new GeneralPost();

                newPost.Text = instElem.caption;
                newPost.Image = instElem.display_src;
                double sec = instElem.date;
                newPost.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(sec);
                newPost.PostLink = "https://www.instagram.com/p/" + instElem.code;

                request = (HttpWebRequest)WebRequest.Create(newPost.PostLink + "/?__a=1");
                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                dynamic instPostData = JsonConvert.DeserializeObject(responseString);
                var instAuthor = instPostData.graphql.shortcode_media.owner;

                newPost.AuthorName = instAuthor.username;
                newPost.AuthorAvatar = instAuthor.profile_pic_url;
                newPost.Social = SocialMedia.Instagram;

                searchResult.Add(newPost);
            }
        }
    }
}
