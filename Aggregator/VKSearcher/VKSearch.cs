using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace VKSearcher
{
    public class VKSearch : ISearcher
    {
        public void Search(string query, List<GeneralPost> searchResult)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/newsfeed.search?q=%23" + query + "&count=20&extended=1&access_token=9123309e9123309e9123309ecb917ffb61991239123309ec86b359fe8d192ce5c598a50");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic vkData = JsonConvert.DeserializeObject(responseString);
            var vkList = vkData.response;
            foreach (var vkElem in vkList)
            {
                if (vkElem is JValue) continue;
                GeneralPost newPost = new GeneralPost();

                newPost.Text = vkElem.text;
                try
                {
                    newPost.Text += Environment.NewLine + vkElem.copy_text;
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
                double sec = vkElem.date;
                newPost.Date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(sec);

                try
                {
                    foreach (var att in vkElem.attachments)
                    {
                        string type = att.type;
                        if (type == "video")
                        {
                            newPost.Image = att.video.image;
                            break;
                        }
                        else if (type == "photo")
                        {
                            newPost.Image = att.photo.src;
                            break;
                        }
                    }
                }
                catch { }

                string screenName;

                try
                {
                    screenName = vkElem.user.screen_name;
                    newPost.AuthorName = vkElem.user.first_name + " " + vkElem.user.last_name;
                    newPost.AuthorAvatar = vkElem.user.photo;
                }
                catch
                {
                    try
                    {
                        screenName = vkElem.group.screen_name;
                        newPost.AuthorName = vkElem.group.name;
                        newPost.AuthorAvatar = vkElem.group.photo;
                    }
                    catch
                    {
                        screenName = "about";
                    }
                }

                string ownerID = vkElem.owner_id;
                string ID = vkElem.id;
                newPost.PostLink = "https://vk.com/" + screenName + "?w=wall" + ownerID + "_" + ID;

                searchResult.Add(newPost);
            }
        }
    }
}
