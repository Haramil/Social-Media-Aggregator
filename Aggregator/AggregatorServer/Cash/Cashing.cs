using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AggregatorServer.Models;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Web.Hosting;
using System.Net;
using System.Drawing;

namespace AggregatorServer.Cash
{
    public class Cashing
    {
        public static string CashQuery = "nastachku";
        public static string Path = HostingEnvironment.ApplicationPhysicalPath;

        public static void Start()
        {
            while (true)
            {
                lock (CashQuery)
                {
                    List<string> files = Directory.EnumerateFiles(Path + @"Cash\Images\").ToList();

                    files.ForEach(file =>
                    {
                        File.Delete(file);
                    });

                    int imageNum = 0;

                    AggregatorModel model = new AggregatorModel();
                    SearchResult result = model.Search(CashQuery);

                    foreach (var post in result.Posts)
                    {
                        if (post.Image == "")
                            continue;
                        WebRequest requestPic = WebRequest.Create(post.Image);
                        WebResponse responsePic = requestPic.GetResponse();
                        Image webImage = Image.FromStream(responsePic.GetResponseStream()); // Error

                        post.Image = @"\Cash\Images\" + imageNum + ".jpg";
                        webImage.Save(Path + post.Image);

                        imageNum++;
                    }

                    File.WriteAllText(Path + @"Cash\" + CashQuery + ".json", JsonConvert.SerializeObject(result));
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
    }
}