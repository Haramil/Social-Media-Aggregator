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

        private static object lockObj = new object();
        private static int imageNum;

        private static string DownloadImage(string link, string query)
        {
            if (!string.IsNullOrEmpty(link))
            {
                try
                {
                    WebRequest requestPic = WebRequest.Create(link);
                    WebResponse responsePic = requestPic.GetResponse();
                    Image webImage = Image.FromStream(responsePic.GetResponseStream());

                    string path;
                    lock (lockObj)
                    {
                        path = @"\Cash\" + query + @"\" + imageNum + ".jpg";
                        webImage.Save(Path + path);
                        imageNum++;
                    }
                    return path;
                }
                catch { }
            }
            return link;
        }

        public static List<Pagination> Cash(string cashquery, int countofpages)
        {
            if (!Directory.Exists((Path + @"Cash\" + cashquery)))
                Directory.CreateDirectory(Path + @"Cash\" + cashquery);
            List<string> files = Directory.EnumerateFiles(Path + @"Cash\" + cashquery).ToList();

            files.ForEach(file =>
            {
                File.Delete(file);
            });

            AggregatorModel model = new AggregatorModel();
            List<GeneralPost> allposts = new List<GeneralPost>();
            SearchResult result = new SearchResult();
            result = model.Search(cashquery);
            allposts.AddRange(result.Posts);
            for (int i = 0; i < countofpages - 1; i++)
            {
                result = model.More(cashquery, result.VKPagination, result.InstPagination, result.TwitterPagination);
                if (result.Posts.Count != 0)
                    allposts.AddRange(result.Posts);
                else
                {
                    countofpages = i + 2;
                    break;
                }

            }
            result.Posts = allposts;
            List<Thread> imageThreads = new List<Thread>();

            imageNum = 0;

            foreach (var post in result.Posts)
            {
                Thread imageThread = new Thread(() =>
                {
                    post.Image = DownloadImage(post.Image, cashquery);
                });

                Thread avatarThread = new Thread(() =>
                {
                    post.AuthorAvatar = DownloadImage(post.AuthorAvatar, cashquery);
                });
                
                imageThreads.Add(avatarThread);
                imageThreads.Add(imageThread);
                avatarThread.Start();
                imageThread.Start();
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
                DateOfAdd = DateTime.Now,
                CountOfPages = countofpages,
                VKPagination = result.VKPagination,
                InstagrammPagination = result.InstPagination,
                TwitterPagination = result.TwitterPagination,
                HashTag = cashquery
            });

            return dbworker.GetAllPagination();
        }

        public static List<Pagination> DeleteHashTag(string query)
        {
            if (Directory.Exists((Path + @"Cash\" + query)))
            {
                List<string> files = Directory.EnumerateFiles(Path + @"Cash\" + query).ToList();
                files.ForEach(file =>
                {
                    File.Delete(file);
                });
                Directory.Delete((Path + @"Cash\" + query));
            }
            DBWorker dbworker = new DBWorker();
            dbworker.DeleteAllPostsByHashTag(query);
            dbworker.DeletePagination(query);
            return dbworker.GetAllPagination();

        }
    }
}