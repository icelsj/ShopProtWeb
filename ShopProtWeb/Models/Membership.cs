﻿using System;
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
        public Guid device_id { get; set; }
        public Guid group_id { get; set; }
        public MembershipStatus status { get; set; }
        public DateTime joined_at { get; set; }


        public async Task<bool> Create()
        {
            bool success = false;
            Exception err = null;
            string sql = "INSERT INTO dbo.Memberships (device_id, group_id, status) OUTPUT INSERTED.id VALUES (@device_id, @group_id, @status)";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();
            SqlTransaction trans = db.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, db, trans);
                cmd.Parameters.AddWithValue("@device_id", device_id);
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
            string sql = "SELECT id, status, joined_at FROM dbo.Memberships WITH (NOLOCK) WHERE device_id LIKE @device_id AND group_id LIKE @group_id";

            if (db.State != ConnectionState.Open)
                await db.OpenAsync();

            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(sql, db);
                cmd.Parameters.AddWithValue("@device_id", device_id);
                cmd.Parameters.AddWithValue("@group_id", group_id);
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = cmd;
                adp.Fill(dt);

                if (dt != null && !dt.HasErrors && dt.Rows.Count > 0)
                {
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

    }
}