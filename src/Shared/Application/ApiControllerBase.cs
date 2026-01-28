using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Shared.Application
{
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender _sender = null!;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    }
}
