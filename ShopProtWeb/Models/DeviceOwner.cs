using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ShopProtWeb.Models
{

    public class DeviceOwner : ShopProtModelBase
    {
        public Guid id { get; set; }
        public Device device { get; set; }
        public User user { get; set; }
        public DateTime linked_at { get; set; }

        public LinkDeviceResponseModel Return
        {
            get
            {
                UserResponseModel user = new UserResponseModel() { id = this.user.id, facebook_id = this.user.facebook_id, email = this.user.email, gender = this.user.gender, name = this.user.name, last_name = this.user.last_name, first_name = this.user.first_name };
                DeviceResponseModel device = new DeviceResponseModel() { id = this.device.id, os = this.device.os, model = this.device.model, access_key = this.device.access_key };
                return new LinkDeviceResponseModel() { id = this.id, user = user, device = device, linked_at = this.linked_at };
            }
        }

        public async Task<bool> LinkDevice()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.DeviceOwners (device_id, user_id) OUTPUT INSERTED.id VALUES (@device_id, @user_id)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@device_id", device.id);
                cmd.Parameters.AddWithValue("@user_id", user.id);
                object id = await cmd.ExecuteScalarAsync();

                if (id != null && id.GetType() == typeof(Guid))
                {
                    this.id = ((Guid)id);
                    success = true;
                    this.linked_at = DateTime.Now;
                }

                trans.Commit();
            }
            catch (Exception e)
            {
                err = e;
                trans.Rollback();
            }
            finally
            {
                db.Close();
            }

            if (err != null)
            {
                throw err;
            }

            return success;
        }

        public async Task<bool> FindByDeviceAndUserId()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id, linked_at FROM dbo.DeviceOwners WITH (NOLOCK) WHERE device_id LIKE @device_id AND user_id like @user_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@device_id", this.device.id);
                cmd.Parameters.AddWithValue("@user_id", this.user.id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    this.id = (Guid)dt.Rows[0]["id"];
                    this.linked_at = (DateTime)dt.Rows[0]["linked_at"];

                    success = true;
                }
            }
            catch (Exception e)
            {
                err = e;
            }
            finally
            {
                db.Close();
            }

            if (err != null)
            {
                throw err;
            }

            return success;
        }
    }

    public class LinkDeviceRegisterModel
    {
        public LDeviceRegisterModel device { get; set; }
        public LUserRegisterModel user { get; set; }
    }

    public class LDeviceRegisterModel
    {
        public Guid id { get; set; }
        public string uuid { get; set; }
        public string os { get; set; }
        public string model { get; set; }
        public string app_token { get; set; }
        public Guid user_id { get; set; }
    }

    public class LUserRegisterModel
    {
        public Guid id { get; set; }
        public string facebook_id { get; set; }
        public string access_token { get; set; }
    }

    public class LinkDeviceResponseModel
    {
        public Guid id { get; set; }
        public UserResponseModel user { get; set; }
        public DeviceResponseModel device { get; set; }
        public DateTime linked_at { get; set; }
    }
}
