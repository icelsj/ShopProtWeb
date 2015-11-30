using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ShopProtWeb.Models;
using System.Threading.Tasks;

namespace ShopProtWeb.Controllers.API
{
    public class ShowAllController : ApiController
    {
        [HttpGet]
        public async Task<ApiMessage> Get()
        {
            DeviceOwner downer;
            Guid id = Guid.Empty;

            ApiMessage msg = new ApiMessage() { success = false };
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                downer = new DeviceOwner() { device = new Device() { id = device.id } };
                authorized = await downer.FindByDeviceId();

                id = downer.user.id;
            }

            if (hasKey && authorized)
            {
                ShowAll show = new ShowAll();
                msg.data = await show.ListAll(id);
                msg.success = true;
            }
            else
            {
                msg.message = "Unauthorized";
            }
            return msg;
        }
    }
}
