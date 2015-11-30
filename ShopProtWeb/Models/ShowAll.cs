using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ShopProtWeb.Models
{
    public class ShowAll : ShopProtModelBase
    {
        public async Task<All_UserModel> ListAll(Guid userid)
        {
            All_UserModel userModel = new All_UserModel();

            //get user
            User user = new User() { id = userid };
            if (await user.FindByID())
            {
                userModel.id = user.id;
                userModel.facebook_id = user.facebook_id;
                userModel.gender = user.gender;
                userModel.email = user.email;
                userModel.name = user.name;
                userModel.first_name = user.first_name;
                userModel.last_name = user.last_name;
            }

            //get group
            GroupList group = new GroupList();
            List<GroupListResponseModel> groups = await group.ListByUserId(user.id);
            userModel.groups = new List<All_GroupModel>();
            foreach (GroupListResponseModel g in groups)
            {
                All_GroupModel groupModel = new All_GroupModel();
                groupModel.id = g.id;
                groupModel.name = g.name;
                groupModel.description = g.description;
                groupModel.status = g.status;
                groupModel.created_at = g.created_at;

                //get group members
                Membership member = new Membership();
                groupModel.members = await member.ListGroupMember(g.id);

                //get items in each group
                ItemList item = new ItemList();
                List<ItemListResponseModel> items = await item.ListByGroupId(g.id);
                groupModel.items = new List<All_ItemModel>();
                foreach (ItemListResponseModel i in items)
                {
                    All_ItemModel itemModel = new All_ItemModel();
                    itemModel.id = i.id;
                    itemModel.name = i.name;
                    itemModel.description = i.description;
                    itemModel.status = i.status;
                    itemModel.created_at = i.created_at;
                    itemModel.category = i.category;
                    itemModel.category_id = i.category_id;
                    itemModel.created_by = i.created_by;

                    groupModel.items.Add(itemModel);
                }

                userModel.groups.Add(groupModel);
            }

            return userModel;
        }
    }

    public class All_UserModel
    {
        //basic user details
        public Guid id { get; set; }
        public string facebook_id { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        //show all groups -> items
        public List<All_GroupModel> groups { get; set; }
    }

    public class All_GroupModel
    {
        //group detail
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public GroupStatus status { get; set; }
        public DateTime created_at { get; set; }

        //items
        public List<All_ItemModel> items { get; set; }
        public List<UserResponseModel> members { get; set; }
    }

    public class All_ItemModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public ItemStatus status { get; set; }
        public DateTime created_at { get; set; }
        public UserResponseModel created_by { get; set; }
    }
}