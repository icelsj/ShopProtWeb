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
    public class MembershipsController : ApiController
    {
        [HttpPost]
        public async Task<ApiMessage> Post (Guid id, MembershipCreateModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false };

            GroupList group = new GroupList();
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;
            Device device = new Device();

            if (hasKey)
            {
                device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                group.id = id;
                bool hasauthorized = await group.FindById();

                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                bool founduser = await downer.FindByDeviceId();
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
                    bool success = await group.FindById();
                    if (success)
                    {
                        //Find user id
                        User user = new User() { facebook_id = model.facebook_id };
                        bool foundUser = await user.FindByFacebookID();

                        //if not found then create a temporary
                        if (!foundUser)
                        {
                            user.email = "";
                            user.name = "";
                            user.first_name = "";
                            user.last_name = "";
                            user.gender = "";
                            user.isAnonymous = true;
                            foundUser = await user.Register();
                        }
                        
                        Membership member = new Membership() { user_id = user.id, group_id = group.id, status = MembershipStatus.Joined };
                        bool foundMember = await member.FindByDeviceIdAndGroupId();
                        if (!foundMember)
                        {
                            success = await member.Create();
                            if (success)
                            {
                                msg.message = "Member is invited successfully";
                                msg.success = true;
                                msg.data = user.Return;
                            }
                            else
                            {
                                msg.message = "Failed to add member";
                            }
                        }
                        else
                        {
                            if (member.status != MembershipStatus.Kicked)
                            {
                                msg.message = "Member has joined the group.";
                                msg.data = user.Return;
                            }
                            else
                            {
                                member.status = MembershipStatus.Joined;
                                success = await member.Update();
                                if (success)
                                {
                                    msg.message = "Member is invited successfully";
                                    msg.success = true;
                                    msg.data = user.Return;
                                }
                                else
                                {
                                    msg.message = "Failed to add member";
                                }
                            }
                        }
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

        [HttpDelete]
        public async Task<ApiMessage> Delete (Guid id, MembershipCreateModel model)
        {
            ApiMessage msg = new ApiMessage() { success = false };

            GroupList group = new GroupList();
            IEnumerable<string> xAccessKey;
            bool hasKey = Request.Headers.TryGetValues("X-Access-Key", out xAccessKey);
            bool authorized = false;
            Device device = new Device();

            if (hasKey)
            {
                device = new Device() { access_key = xAccessKey.First() };
                authorized = await device.FindByAccessKey(device.access_key, true);
                group.id = id;
                bool hasauthorized = await group.FindById();

                DeviceOwner downer = new DeviceOwner() { device = new Device() { id = device.id } };
                bool founduser = await downer.FindByDeviceId();
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
                    bool success = await group.FindById();
                    if (success)
                    {
                        //Find user id
                        User user = new User() { facebook_id = model.facebook_id };
                        bool foundUser = await user.FindByFacebookID();

                        //if not found then create a temporary
                        if (!foundUser)
                        {
                            msg.message = "User is not exists";
                        }
                        else
                        {
                            Membership member = new Membership() { user_id = user.id, group_id = group.id };
                            bool foundMember = await member.FindByDeviceIdAndGroupId();
                            if (foundMember && member.status != MembershipStatus.Kicked)
                            {
                                member.status = MembershipStatus.Kicked;
                                success = await member.Update();
                                if (success)
                                {
                                    msg.message = "Member is removed successfully";
                                    msg.success = true;
                                    msg.data = user.Return;
                                }
                                else
                                {
                                    msg.message = "Failed to remove a member";
                                }
                            }
                            else
                            {
                                msg.message = "User is not in the group.";
                                msg.data = user.Return;
                            }
                        }
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
