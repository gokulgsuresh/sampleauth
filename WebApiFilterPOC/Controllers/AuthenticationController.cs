using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApiFilterPOC.Filters;

namespace WebApiFilterPOC.Controllers
{
    public class AuthenticationController : ApiController
    {

        [Route("v1/Authentication")]
        //[TokenAuthentication]
        [HttpPost]
        public IHttpActionResult AuthenticateUser()
        {
            return (IHttpActionResult)this.Ok();
        }
    }
}