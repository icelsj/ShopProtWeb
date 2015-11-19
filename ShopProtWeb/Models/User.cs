using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ShopProtWeb.Models
{
    public class User : ShopProtModelBase
    {
        public Guid id { get; set; }
        public string facebook_id { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string access_token { get; set; }
        public bool isAnonymous { get; set; }

        public User()
        {
        }

        public User (UserResponseModel model)
        {
            this.facebook_id = model.facebook_id;
            this.gender = model.gender;
            this.email = model.email;
            this.name = model.name;
            this.first_name = model.first_name;
            this.last_name = model.last_name;
        }

        public UserResponseModel Return
        {
            get
            {
                return new UserResponseModel() { id = this.id, facebook_id = this.facebook_id, email = this.email, gender = this.gender, name = this.name, last_name = this.last_name, first_name = this.first_name };
            }
        }

        public async Task<bool> Register()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Users (facebook_id, gender, email, name, first_name, last_name, isAnonymous) OUTPUT INSERTED.id VALUES (@facebook_id, @gender, @email, @name, @first_name, @last_name, @isAnonymous)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@facebook_id", facebook_id);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@last_name", last_name);
                cmd.Parameters.AddWithValue("@first_name", first_name);
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
            string sql = "SELECT id FROM dbo.Users WITH (NOLOCK) WHERE facebook_id LIKE @facebook_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@facebook_id", facebook_id);
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
            string sql = "SELECT id, facebook_id, gender, email, name, first_name, last_name, isAnonymous FROM dbo.Users WITH (NOLOCK) WHERE id LIKE @id";

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
                    this.gender = dt.Rows[0]["gender"].ToString();
                    this.email = dt.Rows[0]["email"].ToString();
                    this.name = dt.Rows[0]["name"].ToString();
                    this.first_name = dt.Rows[0]["first_name"].ToString();
                    this.last_name = dt.Rows[0]["last_name"].ToString();
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

    public class UserRegisterModel
    {
        [Required]
        public string facebook_id { get; set; }
        [Required]
        public string access_token { get; set; }
    }

    public class UserResponseModel
    {
        public Guid id { get; set; }
        public string facebook_id { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}