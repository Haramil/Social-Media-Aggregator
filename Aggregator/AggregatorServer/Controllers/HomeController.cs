using AggregatorServer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
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
            InstagramModel instModel = new InstagramModel();
            VKModel vKModel = new VKModel();
            List<Block> list = instModel.Search(query);
            list.AddRange(vKModel.Search(query));
            return JsonConvert.SerializeObject(list);
        }
    }
}