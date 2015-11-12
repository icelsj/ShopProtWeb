using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopProtWeb.Models
{
    public enum ItemStatus { Deleted = 0, Active = 1, Done = 2 }

    public class ItemList
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public ItemStatus status { get; set; }
        public DateTime created_at { get; set; }
        public Guid created_by { get; set; }
    }
}