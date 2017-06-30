using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchLibrary
{
    public class GeneralPost
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string AuthorName { get; set; } = "";

        public string AuthorAvatar { get; set; }

        public string PostLink { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SocialMedia Social { get; set; }

        public string Text { get; set; }

        public string Image { get; set; } = "";
        public string AuthorLink { get; set; } = "";

        [JsonIgnore]
        public string HashTag { get; set; }
    }

    public enum SocialMedia
    {
        Instagram,
        VK,
        Twitter
    }
}
