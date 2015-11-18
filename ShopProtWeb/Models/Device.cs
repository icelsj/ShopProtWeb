using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Serialization;

namespace ShopProtWeb.Models
{
    public enum Gender { Unspecified = 0, Male = 1, Female = 2 }
    
    public class Device : ShopProtModelBase
    {
        public Guid id { get; set; }
        [Required]
        public string uuid { get; set; }
        [Required]
        public string os { get; set; }
        [Required]
        public string model { get; set; }
        public DateTime installed_at { get; set; }
        public DateTime created_at { get; set; }
        
        public async Task<bool> Install()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Devices (uuid, os, model, installed_at) OUTPUT INSERTED.id VALUES (@uuid,@os,@model,@installed_at)";

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
                cmd.Parameters.AddWithValue("@installed_at", installed_at);
                object id = await cmd.ExecuteScalarAsync();

                if (id != null && id.GetType() == typeof(Guid))
                {
                    this.id = ((Guid)id);
                    success = true;
                    this.created_at = DateTime.Now;
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
            string sql = "SELECT id, uuid, os, model, installed_at, created_at FROM dbo.Devices WITH (NOLOCK) WHERE id LIKE @id";

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
                    this.installed_at = (DateTime)dt.Rows[0]["installed_at"];
                    this.created_at = (DateTime)dt.Rows[0]["created_at"];

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

    public class User: ShopProtModelBase
    {
        public Guid id { get; set; }
        [Required]
        public string facebook_id { get; set; }
        public DateTime dob { get; set; }
        [Required]
        public Gender gender { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string name { get; set; }

        public bool isAnonymous { get; set; }
        
        public async Task<bool> Register()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Users (facebook_id, dob, gender, email, name, isAnonymous) OUTPUT INSERTED.id VALUES (@facebook_id, @dob, @gender, @email, @name, @isAnonymous)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@facebook_id", facebook_id);
                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@isAnonymous", isAnonymous);
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

        public async Task<bool> FindByFacebookID()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id FROM dbo.Users WITH (NOLOCK) WHERE facebook_id LIKE @facebook_id AND email like @email";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@facebook_id", facebook_id);
                cmd.Parameters.AddWithValue("@email", email);
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
            string sql = "SELECT id, facebook_id, dob, gender, email, name, isAnonymous FROM dbo.Users WITH (NOLOCK) WHERE id LIKE @id";

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
                    this.facebook_id = dt.Rows[0]["facebook_id"].ToString();
                    this.dob = (DateTime)dt.Rows[0]["dob"];
                    this.gender = (Gender)dt.Rows[0]["gender"];
                    this.email = dt.Rows[0]["email"].ToString();
                    this.name = dt.Rows[0]["name"].ToString();
                    this.isAnonymous = (bool)dt.Rows[0]["isAnonymous"];

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

    public class DeviceOwner : ShopProtModelBase
    {
        public Guid id { get; set; }
        public Device device { get; set; }
        public User user { get; set; }
        public DateTime linked_at { get; set; }

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
}