using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MOJ.Shared.Application;

namespace MOJ.Modules.UserManagments.API.Controllers
{
    [Route("api/roles")]
    public class UserManagmentController : ApiControllerBase
    {
        [HttpGet(Name = "asd")]
        public string Get()
        {
            return "asd";
        }
    }
}
