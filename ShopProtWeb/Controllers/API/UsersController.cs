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
        public async Task<ApiMessage> Post(UserRegisterModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false };
            try
            {
                if (ModelState.IsValid)
                {
                    UserResponseModel response;
                    if (!UniTool.VerifyFacebook(model.facebook_id, model.access_token, out response))
                    {
                        msg.message = "Sorry, Facebook access token is invalid";
                        return msg;
                    }

                    User user = new User(response);
                    bool success = await user.FindByFacebookID();
                    if (success)
                    {
                        await user.FindByID();
                        msg.message = "This user had been registered before";
                        msg.success = true;
                        msg.data = user.Return;
                    }
                    else
                    {
                        user.isAnonymous = false;
                        if (await user.Register())
                        {
                            msg.message = "User has been registered successfully";
                            msg.success = true;
                            msg.data = user.Return;
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
