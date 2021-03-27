using System;

namespace server
{
    public class ForbiddenResponse
    {
        public String StatusCode { get; set; }
        public String ResponseMessage { get; set; }

        public FilterJSON HydratedContent { get; set; }

    }
}
