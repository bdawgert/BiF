using System.Collections.Generic;
using Newtonsoft.Json;

namespace BiF.Untappd.Models.Search
{

    public class UntappdResponse<T>
    {
        public T Response { get; set; }
    }

    public class BeerSearchResult
    {
        public int Found { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Term { get; set; }
        [JsonProperty("parsed_term")]
        public string ParsedTerm { get; set; }

        public Beers Beers { get; set; }

    }

    public class BeerLookupResult
    {
        public Beer Beer { get; set; }

    }

    public class Beers
    {
        public int Count { get; set; }
        public List<BeerItem> Items { get; set; }

    }

    public class BeerItem
    {
        public Beer Beer { get; set; }
        public Brewery Brewery { get; set; }
    }

    public class Beer
    {
        [JsonProperty("bid")]
        public string Id { get; set; }
        [JsonProperty("beer_name")]
        public string Name { get; set; }
        [JsonProperty("beer_abv")]
        public double ABV { get; set; }
        [JsonProperty("beer_description")]
        public string Description { get; set; }
        [JsonProperty("beer_style")]
        public string Style { get; set; }

        [JsonProperty("rating_score")]
        public float Rating { get; set; }

        public Brewery Brewery { get; set; }
    }

    public class Brewery {
        [JsonProperty("brewery_name")]
        public string Name { get; set; }

    }

}
