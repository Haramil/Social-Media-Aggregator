using InstagramSearcher;
using SearchLibrary;
using System.Collections.Generic;
using VKSearcher;
using System.Linq;

namespace AggregatorServer.Models
{
    public class AggregatorModel
    {
        public List<GeneralPost> Search(string query)
        {
            List<GeneralPost> result = new List<GeneralPost>();
            InstagramSearch instagram = new InstagramSearch();
            VKSearch vk = new VKSearch();
            instagram.Search(query, result);
            vk.Search(query, result);
            return result.OrderByDescending(p => p.Date).ToList();
        }
    }
}