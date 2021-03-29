using System;

namespace server
{
    public class Request
    {
        public String Domain { get; set; }
        public String Content { get; set; }
        public String Title { get; set; }

        // What the client number actually is
        public Int32 ClientId { get; set; }

        // What the request number from the client actually is
        public Int32 RequestId { get; set; }

        // Status code - to indicate the status of the resource returned
        public Int32 StatusCode { get; set; }

        // RequestTimeMeasured - for calculating stop watch time
        public String RequestTimeMeasured { get; set; }

        public Boolean isFromCache { get; set; }
    }
}
