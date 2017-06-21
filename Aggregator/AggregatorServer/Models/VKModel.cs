using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace AggregatorServer.Models
{
    public class VKModel
    {
        public List<Block> Search(string query)
        {
            var vkBlocks = new List<Block>();

            var request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/newsfeed.search?q=" + query + "&count=20&access_token=9123309e9123309e9123309ecb917ffb61991239123309ec86b359fe8d192ce5c598a50");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic vkData = JsonConvert.DeserializeObject(responseString);
            var vkList = vkData.response;
            foreach (var vkElem in vkList)
            {
                try
                {
                    vkBlocks.Add(new Block
                    {
                        Caption = vkElem.text,
                        Image = vkElem.attachment.photo.src
                    });
                }
                catch
                {
                    try
                    {
                        vkBlocks.Add(new Block
                        {
                            Image = string.Empty,
                            Caption = vkElem.text,
                        });
                    }
                    catch { }
                }
            }
            
            return vkBlocks;
        }
    }
}