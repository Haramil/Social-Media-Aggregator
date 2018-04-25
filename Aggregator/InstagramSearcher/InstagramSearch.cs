﻿using Censure;
using Newtonsoft.Json;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace InstagramSearcher
{
    public class InstagramSearch : ISearcher
    {
        private void PostSearch(dynamic instagramPost, List<GeneralPost> searchResult, List<string> dict)
        {
            GeneralPost newPost = new GeneralPost();

            var caption = instagramPost.edge_media_to_caption.edges;

            newPost.Text = caption[0].node.text;

            Cenzor cenzor = new Cenzor();
            newPost.Text = cenzor.Cenz(newPost.Text, dict);

            newPost.Image = instagramPost.display_url;
            double sec = instagramPost.taken_at_timestamp;
            newPost.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(sec).ToLocalTime();
            newPost.PostLink = "https://www.instagram.com/p/" + instagramPost.shortcode;
            newPost.Social = SocialMedia.Instagram;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(newPost.PostLink + "/?__a=1");
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                dynamic instPostData = JsonConvert.DeserializeObject(responseString);
                var instAuthor = instPostData.graphql.shortcode_media.owner;

                newPost.AuthorName = instAuthor.username;
                newPost.AuthorLink = "https://www.instagram.com/" + newPost.AuthorName;
                newPost.AuthorAvatar = instAuthor.profile_pic_url;
            }
            catch { }
            
            lock (searchResult)
            {
                searchResult.Add(newPost);
            }
        }

        public string Search(string query, List<GeneralPost> searchResult, string pageInfo, List<string> dict)
        {
            if (pageInfo != "") pageInfo = "&max_id=" + pageInfo;

            string responseString;
          
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/explore/tags/" + query + "/?__a=1" + pageInfo);
                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch { return pageInfo; }

            dynamic instData = JsonConvert.DeserializeObject(responseString);
            var inst = instData.graphql.hashtag.edge_hashtag_to_media;
            try
            {
                List<Thread> postThreads = new List<Thread>();
                foreach (var instElem in inst.edges)
                {
                    Thread postThread = new Thread(() => PostSearch(instElem.node, searchResult, dict));
                    postThreads.Add(postThread);
                    postThread.Start();
                }

                postThreads.ForEach(t => t.Join());
                return inst.page_info.end_cursor;
            }
            catch
            {
                return pageInfo;
            }
        }
    }
}
