using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ShopProtWeb.Models
{
    public class Membership : ShopProtModelBase
    {
        public Guid id { get; set; }
        public Guid user_id { get; set; }
        public Guid group_id { get; set; }
        public MembershipStatus status { get; set; }
        public DateTime joined_at { get; set; }


        public async Task<bool> Create()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Memberships (user_id, group_id, status) OUTPUT INSERTED.id VALUES (@user_id, @group_id, @status)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@group_id", group_id);
                cmd.Parameters.AddWithValue("@status", status);
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

        public async Task<bool> Update()
        {
            bool success = false;
            Exception err = null;
            string sql = "UPDATE dbo.Memberships SET status = @status WHERE id = @id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", this.id);
                success = (await cmd.ExecuteNonQueryAsync() > 0) ? true : false;

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

        public async Task<bool> FindByDeviceIdAndGroupId()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT id, status, joined_at FROM dbo.Memberships WITH (NOLOCK) WHERE user_id LIKE @user_id AND group_id LIKE @group_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@group_id", group_id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    this.id = (Guid)dt.Rows[0]["id"];
                    this.status = (MembershipStatus)dt.Rows[0]["status"];
                    this.joined_at = (DateTime)dt.Rows[0]["joined_at"];

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

        public async Task<List<UserResponseModel>> ListGroupMember(Guid groupid)
        {
            List<UserResponseModel> users = new List<UserResponseModel>();
            Exception err = null;
            string sql = "SELECT users.id, users.facebook_id, users.gender, users.email, users.name, users.first_name, users.last_name FROM dbo.Memberships WITH (NOLOCK), dbo.Users WITH (NOLOCK) WHERE user_id LIKE users.id AND group_id LIKE @group_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@group_id", groupid);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UserResponseModel user = new UserResponseModel();
                        user.id = (Guid)dr["id"];
                        user.facebook_id = dr["facebook_id"].ToString();
                        user.gender = dr["gender"].ToString();
                        user.email = dr["email"].ToString();
                        user.name = dr["name"].ToString();
                        user.first_name = dr["first_name"].ToString();
                        user.last_name = dr["last_name"].ToString();

                        users.Add(user);
                    }
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

            return users;
        }
    }

    public class MembershipCreateModel
    {
        public MembershipStatus status { get; set; }
        public string facebook_id { get; set; }
    }

    public class MembershipResponseModel
    {
        public Guid id { get; set; }
        public GroupListResponseModel group { get; set; }
        public UserResponseModel user { get; set; }
        public IEnumerable<DeviceMemberStatusResponseModel> devices { get; set; }
    }

    public class DeviceMemberStatusResponseModel
    {
        public DeviceResponseModel device { get; set; }
        public MembershipStatus status { get; set; }
    }
}