using NewsSystem.Controllers;
using PhoneStoreSystem.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhoneStoreSystem.Controllers
{
    [VaildateLoginRole(Role = ("Administrator"))]
    public class BackupRestoreController : Controller
    {
        //
        // GET: /BackupRestore/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("index")]
        public ActionResult Indexs()
        {

            string dataRadio = Request["dataRadio"];
            string restoreDbFileName = Request["fileTxt"];
            string backupDbFileName = PublicController.TimeStamp().ToString() + ".bak";
            string dbName = "PhoneStoreDB";      //数据文件名
            string path = Server.MapPath("~\\App_Data\\") + "backup_restore"; //文件夹路径

            string backPath = "D:\\PhoneStoreDB_bak";

            //string connsql = "data source=122.10.70.150,1433;User ID=a1203195110;pwd=540323699;Initial Catalog=master";
            string connsql = "Data Source=.;Initial Catalog=master;Integrated Security=True";



            if (dataRadio.Equals("backup"))//备份数据库
            {
                SqlConnection connection = new SqlConnection(connsql);

                if (!Directory.Exists(backPath))            //判断一个文件夹是否存在    
                {
                    Directory.CreateDirectory(backPath);       //创建文件夹
                }
                backPath += "\\" + backupDbFileName;        //备份文件保存的路径及文件名
                SqlCommand command = new SqlCommand("use master backup database @name to disk=@path;", connection);    //备份代码
                connection.Open();
                command.Parameters.AddWithValue("@name", dbName);   //将值替换
                command.Parameters.AddWithValue("@path", backPath);
                command.ExecuteNonQuery();
                connection.Close();
                connection.Dispose();
                ViewBag.dataMessage = "备份成功啦！";

                return View();
            }
            else//恢复数据库
            {
                if (restoreDbFileName == null)
                {
                    ViewBag.dataMessage = "请选择需要还原的文件！！";
                    return View();
                }
                SqlConnection connection = new SqlConnection(connsql);
                connection.Open();
                //-------------------杀掉所有连接 db_CSManage 数据库的进程--------------
                string strSQL = "select spid from master..sysprocesses where dbid=db_id( 'PhoneStoreDB') ";
                SqlDataAdapter Da = new SqlDataAdapter(strSQL, connection);

                DataTable spidTable = new DataTable();
                Da.Fill(spidTable);

                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.Connection = connection;

                for (int iRow = 0; iRow <= spidTable.Rows.Count - 1; iRow++)
                {
                    Cmd.CommandText = "kill " + spidTable.Rows[iRow][0].ToString();   //强行关闭用户进程 
                    Cmd.ExecuteNonQuery();
                }
                connection.Close();
                connection.Dispose();

                SqlConnection sqlcon = new SqlConnection(connsql);
                int index = restoreDbFileName.LastIndexOf("\\");
                if (index != -1)
                {
                    restoreDbFileName = restoreDbFileName.Substring(index);
                }
                backPath += "\\" + restoreDbFileName;
                SqlCommand command = new SqlCommand("use master  restore database @name from disk=@path with replace;", sqlcon);//还原代码
                sqlcon.Open();
                command.Parameters.AddWithValue("@name", dbName);
                command.Parameters.AddWithValue("@path", backPath);
                command.ExecuteNonQuery();
                sqlcon.Close();
                sqlcon.Dispose();

                Response.Write("<script>alert('还原成功');</script>");

                return RedirectToAction("Login", "Account");
            }


        }
    }
}
