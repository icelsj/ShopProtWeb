using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ShopProtWeb.Models;

namespace ShopProtWeb.Controllers.API
{
    public class DevicesController : ApiController
    {
        /// <summary>
        ///     Register device only and return with the whole device information where it registered
        /// </summary>
        /// <returns>device_id</returns>
        [HttpPost]
        public async Task<ApiMessage> Post(DeviceRegisterModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                if (ModelState.IsValid)
                {
                    Device device = new Device(model);

                    bool success = await device.FindByUUID();
                    if (success)
                    {
                        await device.FindByID();

                        if (model.app_token != null)
                        {
                            device.app_token = model.app_token;
                        }
                        if (model.user_id != null && model.user_id != Guid.Empty)
                        {
                            device.user_id = model.user_id;
                        }

                        await device.UpdateInstall();
                        msg.message = "This device had been registered before";
                        msg.success = true;
                        msg.data = device.Return;
                    }
                    else
                    {
                        if (await device.Install())
                        {
                            msg.message = "This device has been registered successfully";
                            msg.success = true;
                            msg.data = device.Return;
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
