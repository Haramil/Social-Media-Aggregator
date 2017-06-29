using System;
using System.ComponentModel.DataAnnotations;

namespace AggregatorServer.Models
{
    public class Pagination
    {
        [Key]
        public string HashTag { get; set; }
        public DateTime DateOfAdd { get; set; }
        public int CountOfPages { get; set; }
        public string VKPagination { get; set; }
        public string TwitterPagination { get; set; }
        public string InstagrammPagination { get; set; }
    }
}