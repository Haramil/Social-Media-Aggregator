using Newtonsoft.Json;
using System;
using System.IO;
using TwitterSearcher.Classes;

namespace TwitterSearcher.Services
{
    internal class JSONWorker
    {
        internal TweetResponse getReponse(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StringReader stringjson = new StringReader(path))
            {
                try
                {
                    return (TweetResponse)serializer.Deserialize(stringjson, typeof(TweetResponse));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return default(TweetResponse);
                }
            }

        }
    }
}
