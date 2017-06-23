using AggregatorServer.Models;
using Newtonsoft.Json;
using System.Web.Mvc;
using AggregatorServer.Cash;

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
            string jsonFile = Cashing.Path + @"Cash\" + Cashing.CashQuery + ".json";

            if (Cashing.CashQuery == query && System.IO.File.Exists(jsonFile))
                lock (Cashing.CashQuery)
                {
                    return System.IO.File.ReadAllText(jsonFile);
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