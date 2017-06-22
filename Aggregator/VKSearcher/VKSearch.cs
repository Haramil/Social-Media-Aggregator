using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace VKSearcher
{
    public class VKSearch : ISearcher
    {
        public void Search(string query, List<GeneralPost> searchResult)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/newsfeed.search?q=" + query + "&count=20&access_token=9123309e9123309e9123309ecb917ffb61991239123309ec86b359fe8d192ce5c598a50");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic vkData = JsonConvert.DeserializeObject(responseString);
            var vkList = vkData.response;
            foreach (var vkElem in vkList)
            {
                try
                {
                    searchResult.Add(new GeneralPost
                    {
                        Text = vkElem.text,
                        Image = vkElem.attachment.photo.src
                    });
                }
                catch
                {
                    try
                    {
                        searchResult.Add(new GeneralPost
                        {
                            Image = string.Empty,
                            Text = vkElem.text,
                        });
                    }
                    catch { }
                }
            }
        }
    }
}
