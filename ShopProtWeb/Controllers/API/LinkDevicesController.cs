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
        public async Task<ApiMessage> Post(LinkDeviceRegisterModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                //scenario 1: provided with Device id and User id
                if (model.user.id != null && model.device.id != null && model.user.id != Guid.Empty && model.device.id != Guid.Empty)
                {
                    Device device = new Device() { id = model.device.id };
                    User user = new User() { id = model.user.id };
                    if (await user.FindByID() && await device.FindByID())
                    {
                        DeviceOwner downer = new DeviceOwner() { user = user, device = device };

                        if (await downer.FindByDeviceAndUserId())
                        {
                            msg.success = false;
                            msg.message = "Device and User had been linked before";
                            msg.data = downer.Return;
                        }
                        else if (await downer.LinkDevice())
                        {
                            msg.success = true;
                            msg.message = "Device and User is linked successfully";
                            msg.data = downer.Return;
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
                    Device device = new Device() { uuid = model.device.uuid, os = model.device.os, model = model.device.model, app_token = model.device.app_token, user_id = model.device.user_id };
                    User user = new User() { facebook_id = model.user.facebook_id, access_token = model.user.access_token };

                    DeviceOwner downer = new DeviceOwner() { user = user, device = device };

                    UserResponseModel response;
                    if (!UniTool.VerifyFacebook(user.facebook_id, user.access_token, out response))
                    {
                        msg.message = "Sorry, Facebook access token is invalid";
                        return msg;
                    }
                    user = new User(response);

                    //try find user and device first
                    bool installed = true;
                    if (!await device.FindByUUID())
                    {
                        installed = await device.Install();
                    }
                    else
                    {
                        await device.FindByID();
                    }
                    if (installed && !await user.FindByFacebookID())
                    {
                        installed = await user.Register();
                    }

                    //try register user and device first
                    if (installed)
                    {
                        if (await downer.FindByDeviceAndUserId())
                        {
                            msg.success = true;
                            msg.message = "Device and User had been linked before";
                            downer.user = user;
                            downer.device = device;
                            msg.data = downer.Return;
                        }
                        else if (await downer.LinkDevice())
                        {
                            msg.success = true;
                            msg.message = "Device and User is linked successfully";
                            downer.user = user;
                            downer.device = device;
                            msg.data = downer.Return;
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
