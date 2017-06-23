using Censure;
using Newtonsoft.Json;
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
        Dictionary<int, Author> authors = new Dictionary<int, Author>();

        private void PostSearch(dynamic vkPost, List<GeneralPost> searchResult, List<string> dict)
        {
            GeneralPost newPost = new GeneralPost();

            newPost.Text = vkPost.text;
            try
            {
                newPost.Text += Environment.NewLine + vkPost.copy_history.text;
            }
            catch { }

            Cenzor cenzor = new Cenzor();
            newPost.Text = cenzor.Cenz(newPost.Text, dict);

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
                    string source;
                    if (type == "video")
                    {
                        source = att.video.photo_800;
                        if (source != null) { newPost.Image = source; break; }
                        source = att.video.photo_640;
                        if (source != null) { newPost.Image = source; break; }
                    }
                    else if (type == "photo")
                    {
                        source = att.photo.photo_807;
                        if (source != null) { newPost.Image = source; break; }
                        source = att.photo.photo_604;
                        if (source != null) { newPost.Image = source; break; }
                        break;
                    }
                }
            }
            catch { }

            int fromID = vkPost.from_id;

            string screenName = authors[fromID].ScreenName;
            newPost.AuthorName = authors[fromID].Name;
            newPost.AuthorAvatar = authors[fromID].Photo;

            string ownerID = vkPost.owner_id;
            string ID = vkPost.id;
            newPost.PostLink = "https://vk.com/" + screenName + "?w=wall" + ownerID + "_" + ID;

            lock (searchResult)
            {
                searchResult.Add(newPost);
            }
        }

        public string Search(string query, List<GeneralPost> searchResult, string pageInfo, List<string> dict)
        {
            if (pageInfo != "") pageInfo = "&start_from=" + pageInfo;

            var request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/newsfeed.search?q=%23" + query + "&count=20&extended=1&access_token=9123309e9123309e9123309ecb917ffb61991239123309ec86b359fe8d192ce5c598a50&v=5.65" + pageInfo);

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException) { return ""; }

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic vkData = JsonConvert.DeserializeObject(responseString);
            var vk = vkData.response;

            try
            {
                foreach (var profile in vk.profiles)
                {
                    int id = profile.id;
                    authors.Add(id, new Author
                    {
                        ScreenName = profile.screen_name,
                        Name = profile.first_name + " " + profile.last_name,
                        Photo = profile.photo_100
                    });
                }
            } catch { }
            try
            {
                foreach (var group in vk.groups)
                {
                    int id = group.id;
                    id = -id;
                    authors.Add(id, new Author
                    {
                        ScreenName = group.screen_name,
                        Name = group.name,
                        Photo = group.photo_200
                    });
                }
            }
            catch { }

            List<Thread> postThreads = new List<Thread>();
            foreach (var item in vk.items)
            {
                Thread postThread = new Thread(() => PostSearch(item, searchResult, dict));
                postThreads.Add(postThread);
                postThread.Start();
            }

            postThreads.ForEach(t => t.Join());

            if (postThreads.Count == 0)
                return "retry";
            else
                return vk.next_from;
        }

        private class Author
        {
            public string ScreenName { get; set; }
            public string Name { get; set; }
            public string Photo { get; set; }
        }
    }
}
