using InstagramSearcher;
using SearchLibrary;
using System.Collections.Generic;
using VKSearcher;
using System.Linq;
using System.Threading;

namespace AggregatorServer.Models
{
    public class AggregatorModel
    {
        public List<GeneralPost> Search(string query)
        {
            List<GeneralPost> result = new List<GeneralPost>();
            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();

            Thread instThread = new Thread(() => instagram.Search(query, result));
            Thread vkThread = new Thread(() => vk.Search(query, result));

            instThread.Start();
            vkThread.Start();
            instThread.Join();
            vkThread.Join();

            return result.OrderByDescending(p => p.Date).ToList();
        }
    }
}