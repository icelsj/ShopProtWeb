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
        ///     Register device
        /// </summary>
        /// <returns>device_id</returns>
        [HttpPost]
        public async Task<ApiMessage> Post(Device model)
        {
            ApiMessage msg = new ApiMessage() { success = false, data = model };
            try
            {
                if (ModelState.IsValid)
                { 
                    //preset default value
                    if (model.installed_at < DateTime.Parse("1/1/1690"))
                    {
                        model.installed_at = DateTime.Parse("1/1/1690");
                    }

                    bool success = await model.FindByUUID();
                    if (success)
                    {
                        await model.FindByID();
                        msg.message = "This device had been registered before";
                        msg.success = true;
                    }
                    else
                    {
                        if (await model.Install())
                        {
                            msg.message = "This device has been registered successfully";
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
