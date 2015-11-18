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
        /// <summary>
        ///     Register user only and return with the whole user information where it registered
        /// </summary>
        /// <returns>device_id</returns>
        [HttpPost]
        public async Task<ApiMessage> Post(User model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                if (ModelState.IsValid)
                {
                    //preset default value
                    if (model.dob < DateTime.Parse("1/1/1690"))
                    {
                        model.dob = DateTime.Parse("1/1/1690");
                    }

                    bool success = await model.FindByFacebookID();
                    if (success)
                    {
                        await model.FindByID();
                        msg.message = "This user had been registered before";
                        msg.success = true;
                    }
                    else
                    {
                        model.isAnonymous = false;
                        if (await model.Register())
                        {
                            msg.message = "User has been registered successfully";
                            msg.success = true;
                        }
                    }
                }
                else
                {
                    msg.message = "data is not completed";
                }
            }
            catch (Exception e)
            {
                msg.message = e.Message;
            }
            return msg;
        }
    }
}
