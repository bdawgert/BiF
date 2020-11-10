using System.Linq;
using System.Web.Mvc;
using BiF.Untappd;
using BiF.Untappd.Models.Search;
using BiF.Web.Utilities;

namespace BiF.Web.Controllers
{
    [Authorize]
    public class UntappdController : Controller
    {
        // GET: Untappd
        public ActionResult Index()
        {



            return View();
        }


        // GET: Untappd
        public ActionResult Search(string q) {
            return View();
        }

        public JsonResult SearchAsync(string q) { 

            string clientId = KeyVault.GetSecret("untappd-clientid").Result;
            string clientSecret = KeyVault.GetSecret("untappd-clientsecret").Result;

            UntappdClient client = UntappdClient.Create(clientId, clientSecret);

            BeerSearchResult searchResult = client.Search(q);

            var results = searchResult.Beers.Items.Select(x => new {
                x.Beer.Id,
                Name = x.Beer.Name,
                x.Beer.Rating,
                Brewery = x.Brewery.Name
            });

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LookUp(int beerId) {

            string clientId = KeyVault.GetSecret("untappd-clientid").Result;
            string clientSecret = KeyVault.GetSecret("untappd-clientsecret").Result;

            UntappdClient client = UntappdClient.Create(clientId, clientSecret);

            ViewBag.Message = client.Lookup(beerId);

            return View("Message");
        }

    }
}