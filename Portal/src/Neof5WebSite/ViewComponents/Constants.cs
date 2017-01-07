using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace Neof5WebSite.ViewComponents
{
    public class MasterSiteHostPortViewComponent : ViewComponent
    {
        public string Invoke() =>
#if DEBUG
            HttpContext.Request.Host.Host + ":49368";
#else
            "neof5master.azurewebsites.net:80";
#endif
    }

    public class SiteTopUrlViewComponent : ViewComponent
    {
        public string Invoke() { 
            var url = HttpContext.Request.GetEncodedUrl();
            var mr = Regex.Match(url, @"^\w+://[^/]+/");
            if (!mr.Success) return null;
            return mr.Groups[0].Value;
        }
    }
}
