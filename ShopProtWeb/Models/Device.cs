using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

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
        public DateTime created_at { get; set; }
        
        public async Task<bool> Install()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Device (uuid, os, model, installed_at) OUTPUT INSERTED.id VALUES (@uuid,@os,@model,@installed_at)";

            if (db.State != System.Data.ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@os", os);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@installed_at", installed_at);
                object id = await cmd.ExecuteScalarAsync();

                if (id.GetType() == typeof(string))
                {
                    this.id = Guid.Parse(id.ToString());
                    //this.id = id.ToString();
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
    }

    public class User: ShopProtModelBase
    {
        public Guid id { get; set; }
        public string facebook_id { get; set; }
        public DateTime dob { get; set; }
        public Gender gender { get; set; }
        public string email { get; set; }
        public string name { get; set; }

        public bool isAnnoymous { get; set; }
        
        public async Task<bool> Register()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.User (facebook_id, dob, gender, email, name, isAnnoymous) VALUES (@facebook_id, @dob, @gender, @email, @name, @isAnnoymous)";

            if (db.State != System.Data.ConnectionState.Open)
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
                cmd.Parameters.AddWithValue("@isAnnoymous", isAnnoymous);
                success = cmd.ExecuteNonQuery() > 0 ? true : false;

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
    }

    public class DeviceOwner : ShopProtModelBase
    {
        public Guid id { get; set; }
        public Guid device_id { get; set; }
        public Guid user_id { get; set; }
        public DateTime linked_at { get; set; }
    }
}