
namespace BiF.Web.ViewModels.Home
{
    public class ParticipantVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool HasProfile { get; set; }
        public int UserStatus { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSignedUp { get; set; }
    }
}