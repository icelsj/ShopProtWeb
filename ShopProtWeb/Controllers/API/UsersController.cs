using ShopProtWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShopProtWeb.Controllers.API
{
    public class UsersController : ApiController
    {
        [HttpPost]
        public async Task<ApiMessage> Post(User model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                
            }
            catch (Exception e)
            {
                msg.message = e.Message;
            }
            return msg;
        }
    }
}
