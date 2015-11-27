using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ShopProtWeb.Models
{
    public enum ItemStatus { Deleted = 0, Active = 1, Done = 2 }

    public class ItemList : ShopProtModelBase
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public ItemStatus status { get; set; }
        public DateTime created_at { get; set; }
        public Guid created_by { get; set; }
        public Guid group_id { get; set; }
        public User creator { get; set; }
        
        public ItemList()
        {

        }

        public ItemList(ItemListCreateModel model)
        {
            this.name = model.name;
            this.description = model.description;
            this.status = model.status;
            this.category = model.category;
            this.category_id = model.category_id;
        }
        
        public ItemListResponseModel Return
        {
            get
            {
                return new ItemListResponseModel() { id = this.id, name = this.name, description = this.description, status = this.status, created_at = this.created_at, category = this.category, category_id = this.category_id, created_by = this.creator.Return };
            }
        }

        public async Task<bool> Create()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.ItemLists (name, description, status, created_by, category, category_id, group_id) OUTPUT INSERTED.id VALUES (@name, @description, @status, @created_by, @category, @category_id, @group_id)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@created_by", created_by);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@category_id", category_id);
                cmd.Parameters.AddWithValue("@group_id", group_id);
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
            string sql = "UPDATE dbo.ItemLists SET name = @name, description = @description, status = @status, category = @category, category_id = @category_id WHERE id = @id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@id", this.id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@category_id", category_id);
                success = await cmd.ExecuteNonQueryAsync() > 0 ? true : false;

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
            string sql = "SELECT name, description, status, created_at, created_by, category, category_id FROM dbo.ItemLists WITH (NOLOCK) WHERE id LIKE @id";

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
                    this.status = (ItemStatus)dt.Rows[0]["status"];
                    this.created_at = (DateTime)dt.Rows[0]["created_at"];
                    this.created_by = (Guid)dt.Rows[0]["created_by"];
                    this.category = dt.Rows[0]["category"].ToString();
                    this.category_id = (Guid)dt.Rows[0]["category_id"];

                    User aUser = new User() { id = this.created_by };
                    await aUser.FindByID();
                    this.creator = aUser;

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

        public async Task<List<ItemListResponseModel>> ListByGroupId(Guid group_id)
        {
            List<ItemListResponseModel> items = new List<ItemListResponseModel>();
            Exception err = null;
            string sql = "SELECT id, name, description, status, created_at, created_by, category, category_id FROM dbo.ItemLists WITH (NOLOCK) WHERE group_id = @group_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@group_id", group_id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ItemList item = new ItemList();
                        item.id = (Guid)dr["id"];
                        item.name = dr["name"].ToString();
                        item.description = dr["description"].ToString();
                        item.status = (ItemStatus)dr["status"];
                        item.created_at = (DateTime)dr["created_at"];
                        item.created_by = (Guid)dr["created_by"];
                        item.category = dr["category"].ToString();
                        item.category_id = (Guid)dr["category_id"];

                        User aUser = new User() { id = item.created_by };
                        await aUser.FindByID();
                        item.creator = aUser;

                        items.Add(item.Return);
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

            return items;
        }

    }

    public class ItemListCreateModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public ItemStatus status { get; set; }
    }

    public class ItemListResponseModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public Guid category_id { get; set; }
        public ItemStatus status { get; set; }
        public DateTime created_at { get; set; }
        public UserResponseModel created_by { get; set; }
    }
}