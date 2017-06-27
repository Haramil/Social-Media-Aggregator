using Newtonsoft.Json;

namespace TwitterSearcher.Classes
{
    internal class TweetResponse
    {
        
            [JsonProperty("has_more_items")]
            internal bool HasMoreItems { get; set; }
            [JsonProperty("items_html")]
            internal string Items_html { get; set; }
            [JsonProperty("new_latent_count")]
            internal int LatentCount { get; set; }
            [JsonProperty("focused_refresh_interval")]
            internal long RefreshInterval { get; set; }
            [JsonProperty("min_position")]
            internal string MinPosition { get; set; }
        
    }
}
