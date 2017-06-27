using SearchLibrary;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AggregatorServer.Models
{
    public class DBWorker
    {
        public List<GeneralPost> GetAllPostsByHashTag(string hashtag)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Posts.Where(x => x.HashTag == hashtag).ToList();
                }

            }
        }
        public Pagination GetPaginations(string hashtag)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Paginations.Where(x => x.HashTag == hashtag).First();
                }

            }
        }
        public void AddAllPosts(List<GeneralPost> posts, string cashquery)
        {
            foreach (GeneralPost post in posts)
            {
                post.HashTag = cashquery;
            }
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Posts.AddRange(posts);
                    db.SaveChanges();
                }

            }
        }
        public void AddPagination(Pagination pagination)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Paginations.Add(pagination);
                    db.SaveChanges();
                }

            }
        }

        public void DeleteAllPostsByHashTag(string hashtag)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    IEnumerable<GeneralPost> postsdelete = db.Posts.Where(x => x.HashTag == hashtag);
                    db.Posts.RemoveRange(postsdelete);
                    db.SaveChanges();
                }

            }

        }
        public void DeletePagination(string hashtag)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    IEnumerable<Pagination> pagedelete = db.Paginations.Where(x => x.HashTag == hashtag);
                    db.Paginations.RemoveRange(pagedelete);
                    db.SaveChanges();
                }

            }
        }
       

    }
}
