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
using SearchLibrary;

namespace AggregatorServer.Cash
{
    public class Cashing
    {
        public static string Path = HostingEnvironment.ApplicationPhysicalPath;

        public static SearchResult Cash(string cashquery)
        {
            if (!Directory.Exists((Path + @"Cash\" + cashquery)))
                Directory.CreateDirectory(Path + @"Cash\" + cashquery);
            List<string> files = Directory.EnumerateFiles(Path + @"Cash\" + cashquery).ToList();

            files.ForEach(file =>
            {
                File.Delete(file);
            });

            int imageNum = 0;
            object lockObj = new object();

            AggregatorModel model = new AggregatorModel();
            SearchResult result = model.Search(cashquery);

            List<Thread> imageThreads = new List<Thread>();

            foreach (var post in result.Posts)
            {
                if (post.Image == "")
                    continue;

                Thread thread = new Thread(() =>
                {
                    WebRequest requestPic = WebRequest.Create(post.Image);
                    WebResponse responsePic = requestPic.GetResponse();
                    Image webImage = Image.FromStream(responsePic.GetResponseStream());

                    lock (lockObj) {
                        post.Image = @"\Cash\" + cashquery + @"\" + imageNum + ".jpg";
                        webImage.Save(Path + post.Image);
                        imageNum++;
                    }
                });

                imageThreads.Add(thread);
                thread.Start();
                
            }

            foreach (Thread thread in imageThreads)
            {
                thread.Join();
            }

            DBWorker dbworker = new DBWorker();
            dbworker.DeleteAllPostsByHashTag(null);
            dbworker.DeleteAllPostsByHashTag(cashquery);
            dbworker.DeletePagination(cashquery);
            dbworker.AddAllPosts(result.Posts, cashquery);
            dbworker.AddPagination(new Pagination()
            {
                VKPagination = result.VKPagination,
                InstagrammPagination = result.InstPagination,
                TwitterPagination = result.TwitterPagination,
                HashTag = cashquery
            });

            return result;
        }
    }
}