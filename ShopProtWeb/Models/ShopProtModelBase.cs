using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace ShopProtWeb.Models
{
    public abstract class ShopProtModelBase
    {
        [ScriptIgnore]
        [JsonIgnore]
        [XmlIgnore]
        public SqlConnection db = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ShopprotContext"].ConnectionString);
    }

    public class ApiMessage
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}