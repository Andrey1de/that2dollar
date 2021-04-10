using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace that2dollar.Controllers
{
    [Route("/")]
      public class DefaultController : ControllerBase
    {
        /// <summary>
        /// Default controller for render swagger UI 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public IActionResult Index()
        {
            var path = Request.GetEncodedUrl();// +
           
            // var requestUri = new Uri(Request);
            return File(path, "text/html", "index.html"); 
        }
    }

   
}
