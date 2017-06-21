using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
                searchResult.Add(new GeneralPost
                {
                    Caption = instElem.caption,
                    Image = instElem.display_src
                });
            }
        }
    }
}
