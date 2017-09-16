using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ElectriTrackerService.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public string[] Get()
        {
            return new string[] { "value1", "value4" };

        }

        // GET api/values
        public string GetHello(string name)
        {
            return "Hello, " + name + "!";
        }
    }
}
