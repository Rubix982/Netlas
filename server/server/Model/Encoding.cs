using System;

namespace server
{
    public class Encoding
    {
        public String UTF8Encoding { get; set; }
        public String UTF8String { get; set; }

        public Encoding(string utf8Encoding = "", string utf8String = "")
        {
            UTF8Encoding = utf8Encoding;
            UTF8String = utf8String;
        }
    }
}
