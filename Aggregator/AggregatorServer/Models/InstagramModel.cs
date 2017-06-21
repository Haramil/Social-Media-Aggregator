using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace AggregatorServer.Models
{
    public class Block
    {
        public string Caption { get; set; }
        public string Image { get; set; }
    }

    public class InstagramModel
    {
        public List<Block> Search(string query)
        {
            var instBlocks = new List<Block>();

            var request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/explore/tags/" + query + "/?__a=1");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic instData = JsonConvert.DeserializeObject(responseString);
            var instList = instData.tag.media.nodes;
            foreach (var instElem in instList)
            {
                instBlocks.Add(new Block
                {
                    Caption = instElem.caption,
                    Image = instElem.display_src
                });
            }
            
            return instBlocks;
        }
    }
}