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
    public class LinkDevicesController : ApiController
    {
        /// <summary>
        ///     Register user and device in 2 scenario 
        ///     1. Client registered device and user and link them seperately, then client can provide id only for device and user
        ///     2. Client will register device and user at the same time, then client will need to pass all information without id
        /// </summary>
        /// <returns>device_id</returns>
        [HttpPost]
        public async Task<ApiMessage> Post(DeviceOwner model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                //scenario 1: provided with Device id and User id
                if (model.user.id != null && model.device.id != null && model.user.id != Guid.Empty && model.device.id != Guid.Empty)
                {
                    if (await model.user.FindByID() && await model.device.FindByID())
                    {
                        if (await model.FindByDeviceAndUserId())
                        {
                            msg.success = true;
                            msg.message = "Device and User had been linked before";
                        }
                        else if (await model.LinkDevice())
                        {
                            msg.success = true;
                            msg.message = "Device and User is linked successfully";
                        }
                        else
                        {
                            msg.message = "Device and User is failed to link";
                        }
                    }
                    else
                    {
                        msg.message = "Device or User is not registered yet.";
                    }
                }
                else //scenario 2: register user and device
                {
                    //try find user and device first
                    bool installed = true;
                    if (!await model.device.FindByUUID())
                    {
                        installed = await model.device.Install();
                    }
                    if (installed && !await model.user.FindByFacebookID())
                    {
                        installed = await model.user.Register();
                    }

                    //try register user and device first
                    if (installed)
                    {
                        if (await model.FindByDeviceAndUserId())
                        {
                            msg.success = true;
                            msg.message = "Device and User had been linked before";
                        }
                        else if (await model.LinkDevice())
                        {
                            msg.success = true;
                            msg.message = "Device and User is linked successfully";
                        }
                        else
                        {
                            msg.message = "Device and User is failed to link";
                        }
                    }
                    else
                    {
                        msg.message = "Device and User is failed to be created";
                    }
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
