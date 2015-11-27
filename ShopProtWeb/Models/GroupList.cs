using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ShopProtWeb.Models
{
    public enum MembershipStatus { Kicked = 0, Joined = 1, Admin = 9 }
    public enum GroupStatus { Off = 0, On = 1 }

    public class GroupList : ShopProtModelBase
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public GroupStatus status { get; set; }
        public DateTime created_at { get; set; }
        public Guid device_id { get; set; }

        public GroupList()
        {

        }
        
        public GroupList(GroupListCreateModel model)
        {
            this.name = model.name;
            this.description = model.description;
            this.status = model.status;
        }

        public GroupListResponseModel Return
        {
            get
            {
                return new GroupListResponseModel() { id = this.id, name = this.name, description = this.description, status = this.status, created_at = this.created_at };
            }
        }

        public async Task<bool> Create()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.GroupLists (name, description, status, created_by) OUTPUT INSERTED.id VALUES (@name, @description, @status, @created_by)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@created_by", device_id);
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
            this.created_at = DateTime.Now;
            return success;
        }

        public async Task<bool> Update()
        {
            bool success = false;
            Exception err = null;
            string sql = "UPDATE dbo.GroupLists SET name = @name, description = @description, status = @status WHERE id = @id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);
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
        
        public async Task<bool> FindById()
        {
            bool success = false;
            Exception err = null;
            string sql = "SELECT name, description, status, created_at FROM dbo.GroupLists WITH (NOLOCK) WHERE id LIKE @id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    this.name = dt.Rows[0]["name"].ToString();
                    this.description = dt.Rows[0]["description"].ToString();
                    this.status = (GroupStatus)dt.Rows[0]["status"];
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

        public async Task<List<GroupListResponseModel>> ListByUserId(Guid user_id)
        {
            List<GroupListResponseModel> groups = new List<GroupListResponseModel>();
            Exception err = null;
            string sql = "SELECT grouplists.id, grouplists.name, grouplists.description, grouplists.status, grouplists.created_at FROM dbo.GroupLists WITH (NOLOCK), dbo.Memberships WITH (NOLOCK) WHERE grouplists.id = memberships.group_id AND memberships.user_id = @user_id AND grouplists.status = 1 AND memberships.status != 0";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        GroupList group = new GroupList();
                        group.id = (Guid)dr["id"];
                        group.name = dr["name"].ToString();
                        group.description = dr["description"].ToString();
                        group.status = (GroupStatus)dr["status"];
                        group.created_at = (DateTime)dr["created_at"];

                        groups.Add(group.Return);
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

            return groups;
        }

    }

    public class GroupListCreateModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public GroupStatus status { get; set; }
    }

    public class GroupListResponseModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public GroupStatus status { get; set; }
        public DateTime created_at { get; set; }
    }
}