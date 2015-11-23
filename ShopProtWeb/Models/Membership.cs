using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopProtWeb.Models
{
    public class Membership
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public MembershipStatus status { get; set; }
        public DateTime joined_at { get; set; }
    }
}