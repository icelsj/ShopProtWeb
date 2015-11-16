﻿using System;
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
                bool success = await model.FindByUUID();
                if (success)
                {
                    msg.message = "This device had been registered before";
                    msg.success = true;
                }
                else
                {
                    if (await model.Install())
                    {
                        msg.message = "This device have been registered successfully";
                        msg.success = true;
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
