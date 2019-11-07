using System.Collections.Generic;

namespace BiF.Web.Identity
{
    public interface IBifResult
    {
        bool Success { get; }
        IEnumerable<string> Errors { get; }
    }
}