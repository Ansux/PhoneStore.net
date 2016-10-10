using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsSystem.Controllers
{
    public class SqlHelperController : Controller
    {
        private static string sqlConn = ConfigurationManager.ConnectionStrings["PhoneStoreDBConnection"].ToString();

        public static int ExecInsertUpdateDelete(string sql, SqlParameter[] sp)
        {
            int rows = -1;
            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    if (sp != null)
                        comm.Parameters.AddRange(sp);
                    rows = comm.ExecuteNonQuery();
                }
                catch (Exception e1)
                {
                }
                finally
                {
                    if (conn != null)
                        conn.Close();
                }
            }
            return rows;
        }

     
        public static DataTable GetDataTable(string sql, SqlParameter[] sp)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    if (sp != null)
                        comm.Parameters.AddRange(sp);
                    SqlDataAdapter sda = new SqlDataAdapter(comm);
                    sda.Fill(dt);
                }
                catch (Exception ee)
                {
                    return null;
                }
                finally
                {
                    if (conn != null)
                        conn.Close();
                }
            }
            return dt;
        }

    }
}
