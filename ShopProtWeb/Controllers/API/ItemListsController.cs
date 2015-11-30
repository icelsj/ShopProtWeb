using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ShopProtWeb.Models;
using System.Threading.Tasks;

namespace ShopProtWeb.Controllers.API
{
    public class ItemListsController : ApiController
    {
        [HttpPost]
        public async Task<ApiMessage> Post(Guid id, ItemListCreateModel model)
        {
            ItemList item = new ItemList(model);
            
            ApiMessage msg = new ApiMessage() { success = false };
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                authorized = await downer.FindByDeviceId();

                GroupList group = new GroupList() { id = id };
                bool hasauthorized = await group.FindById();

                Membership member = new Membership() { user_id = downer.user.id, group_id = id };
                if (hasauthorized)
                {
                    authorized = await member.FindByDeviceIdAndGroupId();
                    authorized = member.status == MembershipStatus.Kicked ? false : true;

                    item.group_id = group.id;
                    item.created_by = downer.user.id;
                    item.creator = downer.user;
                }
            }

            if (hasKey && authorized)
            {
                if (ModelState.IsValid)
                {
                    bool success = await item.Create();
                    if (success)
                    {
                        msg.message = "Item is created successfully";
                        msg.success = true;
                        msg.data = item.Return;
                    }
                    else
                    {
                        msg.message = "Failed to add item";
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
        public async Task<ApiMessage> Put(Guid id, Guid iid, ItemListCreateModel model)
        {
            ItemList item = new ItemList() { id = iid };

            ApiMessage msg = new ApiMessage() { success = false };
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                authorized = await downer.FindByDeviceId();

                GroupList group = new GroupList() { id = id };
                bool hasauthorized = await group.FindById();

                Membership member = new Membership() { user_id = downer.user.id, group_id = id };
                if (hasauthorized)
                {
                    authorized = await member.FindByDeviceIdAndGroupId();
                    authorized = member.status == MembershipStatus.Kicked ? false : true;
                }
            }

            if (hasKey && authorized)
            {
                if (ModelState.IsValid)
                {
                    bool success = await item.FindById();
                    item.name = model.name;
                    item.description = model.description;
                    item.category_id = model.category_id;
                    item.category = model.category;
                    item.status = model.status;
                    
                    if (success)
                    {
                        success = await item.Update();
                        if (success)
                        {
                            msg.message = "Item is updated successfully";
                            msg.success = true;
                            msg.data = item.Return;
                        }
                        else
                        {
                            msg.message = "Failed to update item";
                        }
                    }
                    else
                    {
                        msg.message = "Failed to update item";
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

        [HttpGet]
        public async Task<ApiMessage> Get(Guid id)
        {
            ApiMessage msg = new ApiMessage() { success = false };
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                authorized = await downer.FindByDeviceId();

                GroupList group = new GroupList() { id = id };
                bool hasauthorized = await group.FindById();

                Membership member = new Membership() { user_id = downer.user.id, group_id = id };
                if (hasauthorized)
                {
                    authorized = await member.FindByDeviceIdAndGroupId();
                    authorized = member.status == MembershipStatus.Kicked ? false : true;
                }
            }

            if (hasKey && authorized)
            {
                ItemList item = new ItemList();
                msg.data = await item.ListByGroupId(id);
                msg.success = true;
                msg.message = "List items successfully";
            }
            else
            {
                msg.message = "Unauthorized";
            }
            return msg;
        }

        [HttpGet]
        public async Task<ApiMessage> Get(Guid id, Guid iid)
        {
            ApiMessage msg = new ApiMessage() { success = false };
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;

            if (hasKey)
            {
                Device device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                authorized = await downer.FindByDeviceId();

                GroupList group = new GroupList() { id = id };
                bool hasauthorized = await group.FindById();

                Membership member = new Membership() { user_id = downer.user.id, group_id = id };
                if (hasauthorized)
                {
                    authorized = await member.FindByDeviceIdAndGroupId();
                    authorized = member.status == MembershipStatus.Kicked ? false : true;
                }
            }

            if (hasKey && authorized)
            {
                ItemList item = new ItemList() { id = iid };
                bool success = await item.FindById();
                if (success)
                {
                    msg.data = item.Return;
                    msg.success = true;
                    msg.message = "Get item successfully";
                }
                else
                {
                    msg.message = "Failed to get item";
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
