using System;
using Microsoft.AspNetCore.Mvc;

namespace server
{
    public class JavaScriptResult : ContentResult
    {
        public new String Content { get; set; }
        public new String ContentType { get; set; }

        public JavaScriptResult(string script)
        {
            Content = script;
            ContentType = "application/javascript";
        }
    }
}
