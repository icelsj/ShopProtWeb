using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopProtWeb.Models
{
    public class ItemDictionary
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; } 
    }

    public class ItemCategory
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
