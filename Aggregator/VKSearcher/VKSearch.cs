using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace VKSearcher
{
    public class VKSearch : ISearcher
    {
        private void PostSearch(dynamic vkPost, List<GeneralPost> searchResult)
        {
            GeneralPost newPost = new GeneralPost();

            newPost.Text = vkPost.text;
            try
            {
                newPost.Text += Environment.NewLine + vkPost.copy_text;
            }
            catch { }
            while (true)
            {
                int begin = newPost.Text.IndexOf("[");
                if (begin == -1) break;
                int end = newPost.Text.IndexOf("]");
                if (end == -1 || end < begin) break;
                string nameFull = newPost.Text.Substring(begin, end - begin + 1);
                int vert = nameFull.IndexOf("|");
                if (vert == -1) break;
                string name = nameFull.Substring(vert + 1, nameFull.Length - vert - 2);
                newPost.Text = newPost.Text.Replace(nameFull, name);
            }

            newPost.Social = SocialMedia.VK;
            double sec = vkPost.date;
            newPost.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(sec);

            try
            {
                foreach (var att in vkPost.attachments)
                {
                    string type = att.type;
                    if (type == "video")
                    {
                        newPost.Image = att.video.image_big;
                        break;
                    }
                    else if (type == "photo")
                    {
                        newPost.Image = att.photo.src_big;
                        break;
                    }
                }
            }
            catch { }

            string screenName;

            try
            {
                screenName = vkPost.user.screen_name;
                newPost.AuthorName = vkPost.user.first_name + " " + vkPost.user.last_name;
                newPost.AuthorAvatar = vkPost.user.photo;
            }
            catch
            {
                try
                {
                    screenName = vkPost.group.screen_name;
                    newPost.AuthorName = vkPost.group.name;
                    newPost.AuthorAvatar = vkPost.group.photo;
                }
                catch
                {
                    screenName = "about";
                }
            }

            string ownerID = vkPost.owner_id;
            string ID = vkPost.id;
            newPost.PostLink = "https://vk.com/" + screenName + "?w=wall" + ownerID + "_" + ID;

            lock (searchResult)
            {
                searchResult.Add(newPost);
            }
        }

        public void Search(string query, List<GeneralPost> searchResult)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/newsfeed.search?q=%23" + query + "&count=20&extended=1&access_token=9123309e9123309e9123309ecb917ffb61991239123309ec86b359fe8d192ce5c598a50");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic vkData = JsonConvert.DeserializeObject(responseString);
            var vkList = vkData.response;

            List<Thread> postThreads = new List<Thread>();
            foreach (var vkElem in vkList)
            {
                if (vkElem is JValue) continue;
                Thread postThread = new Thread(() => PostSearch(vkElem, searchResult));
                postThreads.Add(postThread);
                postThread.Start();
            }

            postThreads.ForEach(t => t.Join());
        }
    }
}
