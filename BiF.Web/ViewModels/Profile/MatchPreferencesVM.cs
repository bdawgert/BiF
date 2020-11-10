using System.Collections.Generic;
using BiF.DAL.Models;

namespace BiF.Web.ViewModels.Profile
{
    public class MatchPreferencesVM {
        public string UserId { get; set; }
        //public string ExchangeName { get; set; }
        public List<UserPublicProfile> AllUsers  { get; set; }
        public List<KeyValuePair<MatchPreferenceType, string>> MatchPreferences { get; set; }
        public int AllowedExclusions { get; set; }
    }

    public class UserPublicProfile
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public Dictionary<int, string> FlavorPreferences { get; set; }
        public double Distance { get; set; }
        public string Zip { get; set; }
    }

}