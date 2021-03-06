﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace Neof5WebSite.ViewComponents
{
    public class MasterSiteHostPortViewComponent : ViewComponent
    {
        public string Invoke()
            => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"
                ? "neof5master.azurewebsites.net:80"
                : HttpContext.Request.Host.Host + ":49368";
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
