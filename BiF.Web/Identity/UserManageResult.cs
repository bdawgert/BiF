using System.Collections.Generic;

namespace BiF.Web.Identity
{
    public class UserManageResult : IBifResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}