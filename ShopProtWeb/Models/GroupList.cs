using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopProtWeb.Models
{
    public enum MembershipStatus { Kicked = 0, Joined = 1, Admin = 9 }
    public enum GroupStatus { Off = 0, On = 1 }
    public class GroupList
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public GroupStatus status { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Membership
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public MembershipStatus status { get; set; }
        public DateTime joined_at { get; set; }
    }
}