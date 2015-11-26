using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShopProtWeb.Models;
using System.Threading.Tasks;

namespace ShopProtWeb.Controllers
{
    public class GroupListsController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <header>X-Access-Key</header>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiMessage> Post(GroupListCreateModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false };
            GroupList group = new GroupList(model);
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                group.device_id = device.id;
            }

            if (hasKey && authorized)
            {
                if (ModelState.IsValid)
                {
                    DeviceOwner downer = new DeviceOwner() { device = new Device() { id = group.device_id } };
                    bool founduser = await downer.FindByDeviceId();
                    bool success = await group.Create();
                    if (success)
                    {
                        Membership member = new Membership() { user_id = downer.user.id, group_id = group.id, status = MembershipStatus.Admin };
                        success = await member.Create();
                    }

                    if (success)
                    {
                        msg.message = "Group is created successfully";
                        msg.success = true;
                        msg.data = group.Return;
                    }
                    else
                    {
                        msg.message = "Failed to add group";
                    }
                }
                else
                {
                    msg.message = "Data is not completed";
                }
            }
            else
            {
                msg.message = "Unauthorized";
            }
            return msg;
        }

        [HttpPut]
        [HttpPatch]
        public async Task<ApiMessage> Put(Guid id, GroupListCreateModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false };
            GroupList group = new GroupList(model);
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                group.device_id = device.id;

                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = group.device_id } };
                await downer.FindByDeviceId();
                
                group.id = id;
                bool hasauthorized = await group.FindById();
                Membership member = new Membership() { user_id = downer.user.id, group_id = group.id };
                if (hasauthorized)
                { 
                    authorized = await member.FindByDeviceIdAndGroupId();
                    authorized = member.status == MembershipStatus.Admin ? true : false;
                }
            }

            if (hasKey && authorized)
            {
                if (ModelState.IsValid)
                {
                    group.name = model.name != null ? model.name : group.name;
                    group.description = model.description != null ? model.description : group.description;
                    group.status = model.status;
                    bool success = await group.Update();

                    if (success)
                    {
                        msg.message = "Group is updated successfully";
                        msg.success = true;
                        msg.data = group.Return;
                    }
                    else
                    {
                        msg.message = "Failed to update group";
                    }
                }
                else
                {
                    msg.message = "Data is not completed";
                }
            }
            else
            {
                msg.message = "Unauthorized";
            }
            return msg;
        }

    }
}
