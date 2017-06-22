using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SearchLibrary
{
    public class GeneralPost
    {
        public DateTime Date { get; set; }

        public string AuthorName { get; set; } = "";

        public string AuthorAvatar { get; set; }

        public string PostLink { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SocialMedia Social { get; set; }

        public string Text { get; set; }

        public string Image { get; set; } = "";
    }

    public enum SocialMedia
    {
        Instagram,
        VK,
        Twitter
    }
}
