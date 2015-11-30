using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Newtonsoft.Json;

namespace ShopProtWeb.Models
{
    public enum Gender { Unspecified = 0, Male = 1, Female = 2 }
    
    public class Device : ShopProtModelBase
    {
        public Guid id { get; set; }
        public string uuid { get; set; }
        public string os { get; set; }
        public string model { get; set; }
        public DateTime installed_at { get; set; }
        public string access_key { get; set; }
        public string app_token { get; set; }
        public Guid user_id { get; set; }
        
        public Device()
        {
                }

        public Device(DeviceRegisterModel model)
                {
            this.uuid = model.uuid;
            this.os = model.os;
            this.model = model.model;
            this.app_token = model.app_token;
            this.user_id = model.user_id;
                }

        public DeviceResponseModel Return
            {
            get
            {
                db.Close();
            }
            }

        private async Task<string> GenerateAccessKey()
        {
            string guidstr = "";
            bool notfound = true;
            do
            {
                Guid g = Guid.NewGuid();
                guidstr = Convert.ToBase64String(g.ToByteArray());
                guidstr = guidstr.Replace("=", "");
                guidstr = guidstr.Replace("+", "");
                guidstr = guidstr.Replace("-", "");
                guidstr = guidstr.Replace("/", "");

                notfound = await this.FindByAccessKey(guidstr, false);

            } while (notfound);

            return guidstr;
        }

        internal async Task<bool> FindByAccessKey(string key, bool replace)
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id, uuid, os, model, installed_at FROM dbo.Devices WITH (NOLOCK) WHERE access_key = @access_key";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@access_key", key);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    if (replace)
                    {
                    this.id = (Guid)dt.Rows[0]["id"];
                    this.uuid = dt.Rows[0]["uuid"].ToString();
                    this.os = dt.Rows[0]["os"].ToString();
                    this.model = dt.Rows[0]["model"].ToString();
                    this.installed_at = (DateTime)dt.Rows[0]["installed_at"];
                    }
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

        public async Task<bool> Install()
    {
            //scope to generate access key first
            this.access_key = await this.GenerateAccessKey();

            //scope to install device
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Devices (uuid, os, model, access_key, app_token, user_id) OUTPUT INSERTED.id VALUES (@uuid, @os, @model, @access_key, @app_token, @user_id)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                if (installed_at == DateTime.MinValue)
                { 
                    installed_at = DateTime.Now;
                }

                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@os", os);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@access_key", access_key);
                if (app_token == null) cmd.Parameters.AddWithValue("@app_token", DBNull.Value); else cmd.Parameters.AddWithValue("@app_token", app_token);
                if (user_id == Guid.Empty) cmd.Parameters.AddWithValue("@user_id", DBNull.Value); else cmd.Parameters.AddWithValue("@user_id", user_id);
                object id = await cmd.ExecuteScalarAsync();

                if (id != null && id.GetType() == typeof(Guid))
                {
                    this.id = ((Guid)id);
                    success = true;
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

        public async Task<bool> UpdateInstall()
        {
            //scope to generate access key first
            this.access_key = await this.GenerateAccessKey();

            //scope to install device
            bool success = false;
            Exception err = null;
            string sql = "UPDATE dbo.Devices SET app_token = @app_token, user_id = @user_id WHERE uuid = @uuid AND os = @os";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                if (installed_at == DateTime.MinValue)
                {
                    installed_at = DateTime.Now;
                }

                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@os", os);
                if (app_token == null) cmd.Parameters.AddWithValue("@app_token", DBNull.Value); else cmd.Parameters.AddWithValue("@app_token", app_token);
                if (user_id == Guid.Empty) cmd.Parameters.AddWithValue("@user_id", DBNull.Value); else cmd.Parameters.AddWithValue("@user_id", user_id);
                object id = await cmd.ExecuteScalarAsync();

                if (id != null && id.GetType() == typeof(Guid))
                {
                    this.id = ((Guid)id);
                    success = true;
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

        public async Task<bool> FindByUUID()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id FROM dbo.Devices WITH (NOLOCK) WHERE uuid LIKE @uuid AND os like @os";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@os", os);
                object id = await cmd.ExecuteScalarAsync();

                if (id != null && id.GetType() == typeof(Guid))
                {
                    this.id = ((Guid)id);
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

        public async Task<bool> FindByID()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id, uuid, os, model, installed_at, access_key FROM dbo.Devices WITH (NOLOCK) WHERE id LIKE @id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@id", this.id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    this.id = (Guid)dt.Rows[0]["id"];
                    this.uuid = dt.Rows[0]["uuid"].ToString();
                    this.os = dt.Rows[0]["os"].ToString();
                    this.model = dt.Rows[0]["model"].ToString();
                    this.access_key = dt.Rows[0]["access_key"].ToString();
                    this.installed_at = (DateTime)dt.Rows[0]["installed_at"];

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

    public class DeviceRegisterModel
    {
        [Required]
        public string uuid { get; set; }
        [Required]
        public string os { get; set; }
        [Required]
        public string model { get; set; }
        public string app_token { get; set; }
        public Guid user_id { get; set; }
    }
        
    public class DeviceResponseModel
    {
        public Guid id { get; set; }
        public string uuid { get; set; }
        public string os { get; set; }
        public string model { get; set; }
        public string access_key { get; set; }
    }
}