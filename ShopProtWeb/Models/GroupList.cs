using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<bool> Create()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.GroupList (name, description, status) OUTPUT INSERTED.id VALUES (@name, @description, @status)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);
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
    }
}