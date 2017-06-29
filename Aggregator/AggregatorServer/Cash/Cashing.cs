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

        public static List<Pagination> Cash(string cashquery, int countofpages)
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
            foreach (var post in result.Posts)
            {
                if (string.IsNullOrEmpty(post.Image))
                    continue;

                Thread imageThread = new Thread(() =>
                {
                    try
                    {
                        WebRequest requestPic = WebRequest.Create(post.Image);
                        WebResponse responsePic = requestPic.GetResponse();
                        Image webImage = Image.FromStream(responsePic.GetResponseStream());

                        lock (lockObj)
                        {
                            post.Image = @"\Cash\" + cashquery + @"\" + imageNum + ".jpg";
                            webImage.Save(Path + post.Image);
                            imageNum++;
                        }
                    }
                    catch { }
                });

                Thread avatarThread = new Thread(() =>
                {
                    try
                    {
                        WebRequest requestPic = WebRequest.Create(post.AuthorAvatar);
                        WebResponse responsePic = requestPic.GetResponse();
                        Image webImage = Image.FromStream(responsePic.GetResponseStream());

                        lock (lockObj)
                        {
                            post.AuthorAvatar = @"\Cash\" + cashquery + @"\" + imageNum + ".jpg";
                            webImage.Save(Path + post.AuthorAvatar);
                            imageNum++;
                        }
                    }
                    catch { }
                });

                imageThreads.Add(imageThread);
                imageThreads.Add(avatarThread);
                imageThread.Start();
                avatarThread.Start();

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