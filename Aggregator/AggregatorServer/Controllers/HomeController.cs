using AggregatorServer.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace AggregatorServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Search(string query)
        {
            AggregatorModel aggregator = new AggregatorModel();
            return JsonConvert.SerializeObject(aggregator.Search(query));
        }
    }
}