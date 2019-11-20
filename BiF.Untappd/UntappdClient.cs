using System.Net.Http;
using BiF.Untappd.Models.Search;
using Newtonsoft.Json;

namespace BiF.Untappd
{
    public class UntappdClient
    {

        private static UntappdClient _untappdClient;
        internal static string ClientId { get; private set; }
        internal static string ClientSecret { get; private set; }

        private HttpClient _httpClient;
        private string _url;

        private UntappdClient(string clientId, string clientSecret) {
            ClientId = clientId;
            ClientSecret = clientSecret;

            _httpClient = new HttpClient();
            _url = $"https://api.untappd.com/v4/method_name?client_id={ClientId}&client_secret={ClientSecret}";

        }

        public static UntappdClient Create() {
            return _untappdClient;
        }

        public static UntappdClient Create(string clientId, string clientSecret) {
            if (_untappdClient != null)
                return _untappdClient;

            return _untappdClient = new UntappdClient(clientId, clientSecret);

        }

        public BeerSearchResult Search(string q) {

            UntappdResponse<BeerSearchResult> searchResult = null;

            string searchUrl =
                $"https://api.untappd.com/v4/search/beer?client_id={ClientId}&client_secret={ClientSecret}&q={q}";

            HttpResponseMessage httpResponse =   _httpClient.GetAsync(searchUrl).Result;
            string json = httpResponse.Content.ReadAsStringAsync().Result;
            searchResult = JsonConvert.DeserializeObject<UntappdResponse<BeerSearchResult>>(json);

            return searchResult.Response;
        }

        public Beer Lookup(int beerId) {

            UntappdResponse<BeerLookupResult> beerResult = null;

            string lookupUrl =
                $"https://api.untappd.com/v4/beer/info/{beerId}?client_id={ClientId}&client_secret={ClientSecret}&compact=true";
            
            //_httpClient.GetAsync(lookupUrl)
            //    .ContinueWith(t => {
            //        t.Result.Content.ReadAsStringAsync()
            //            .ContinueWith(r => {
            //                beer = JsonConvert.DeserializeObject<UntappdResponse<Beer>>(r.Result);
            //            });
            //    });

            HttpResponseMessage httpResponse = _httpClient.GetAsync(lookupUrl).Result;
            string json = httpResponse.Content.ReadAsStringAsync().Result;
            beerResult = JsonConvert.DeserializeObject<UntappdResponse<BeerLookupResult>>(json);

            return beerResult.Response.Beer;

        }

    }
}