using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace that2dollar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnviController : ControllerBase
    {

        // GET api/<EnvironmentController>/5
        [HttpGet("")]
        public IDictionary GeAllt()
        {
            return Environment.GetEnvironmentVariables();
        }

        [HttpGet("username")]
        public string GetUserName()
        {
            return Environment.UserName;
        }

        [HttpGet("{name}")]
        public string GetVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        [HttpPost("{name}/{value}")]
        public string SetVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);
            return Environment.GetEnvironmentVariable(name);
        }



    }
}
