using AggregatorServer.Models;
using Newtonsoft.Json;
using SearchLibrary;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AggregatorServer.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(string query)
        {
            ViewBag.Query = query;
            return View();
        }

        [HttpPost]
        public string Search(string query)
        {

            DBWorker dbworker = new DBWorker();
            List<GeneralPost> posts = dbworker.GetAllPostsByHashTag(query);
            if (posts.Count != 0)
            {
                Pagination pag = dbworker.GetPaginations(query);
                SearchResult res = new SearchResult();
                res.Posts = posts;
                res.InstPagination = pag.InstagrammPagination;
                res.VKPagination = pag.VKPagination;
                res.TwitterPagination = pag.TwitterPagination;
                res.Query = query;
                return JsonConvert.SerializeObject(res);
            }

            AggregatorModel aggregator = new AggregatorModel();
            return JsonConvert.SerializeObject(aggregator.Search(query));
        }

        [HttpPost]
        public string More(string query, string vkPage, string instPage, string twPage)
        {
            AggregatorModel aggregator = new AggregatorModel();
            return JsonConvert.SerializeObject(aggregator.More(query, vkPage, instPage, twPage));
        }
    }
}