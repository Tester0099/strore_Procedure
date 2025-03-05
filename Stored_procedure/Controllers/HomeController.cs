using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;
using Stored_procedure.Models;

namespace Stored_procedure.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connection = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User_data data ,string email)
        {
            string retrive_email = Session["email"] as string;

            if (email == retrive_email)
            {



                try
                {
                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        using (SqlCommand cmd = new SqlCommand("insertdta", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@name", data.name);
                            cmd.Parameters.AddWithValue("@email", data.email);
                            cmd.Parameters.AddWithValue("@password", data.password);
                            cmd.Parameters.AddWithValue("@number", data.number);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["msg"] = "Data Insert successfully";
                    return RedirectToAction("Register");

                }
                
            catch (Exception ex)
            {
                ViewBag.Exception = ex; 
                return RedirectToAction("Register");
            }
            }
            else
            {
                TempData["msg"] = "All ready register this email";
                return RedirectToAction("Ragister");
               
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        
        public ActionResult Login_process(User_data data)
        {
            try
            {
                using(SqlConnection conn=new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("login_pro",conn))
                    {
                        cmd.CommandType= CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@email", data.email);
                        cmd.Parameters.AddWithValue ("Password", data.password);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Session["Userlogin"] = true;
                                Session["email"] = data.email;
                            }
                        }
                    }
                }

                TempData["msg"] = "successfully login";
                return RedirectToAction("Userdashboard");

            }
            catch (Exception ex)
            {
                ViewBag.error=ex;
                return RedirectToAction("Login");
            }
           
        }

        [HttpGet]
        public ActionResult Userdashboard()
        {
            if (Session["UserLogin"]==null)
            {
                return RedirectToAction("Login");
            }

            List<User_data> user_Datas = new List<User_data>();

            using(SqlConnection conn=new SqlConnection(connection))
            {
                using( SqlCommand cmd = new SqlCommand("show_dta", conn))
                {
                    cmd.CommandType=CommandType.StoredProcedure;
                    conn.Open();
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user_Datas.Add(new User_data()
                            {
                                Id=Convert.ToInt32(reader["ID"]),
                                name=reader["name"].ToString(),
                                email=reader["email"].ToString(),
                                password=reader["password"].ToString(),
                                number=reader["number"].ToString(),
                            });
                        }
                    }
                }
            }


            return View(user_Datas);
        }

        public ActionResult Logout()
        {
            Session.Clear();    
            Session.Abandon();
            return RedirectToAction("Login");
        }


        public ActionResult delete(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("delete_pro", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["message"] = "Data delete successfully";
                    return RedirectToAction("Userdashboard");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                return RedirectToAction("Userdashboard");
            }
            
        }
        public ActionResult edit(int id)
        {
            User_data user_data = new User_data();
           
              using(SqlConnection conn = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("edit_dta", conn))
                    {
                        cmd.CommandType= CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id",id);
                        conn.Open();
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                           if (reader.Read())
                           {
                            user_data.Id=Convert.ToInt32(reader["id"]);
                            user_data.name = reader["name"].ToString();
                            user_data.email = reader["email"].ToString();
                            user_data.password= reader["password"].ToString();
                            user_data.number=reader["number"].ToString();
                           }
                        }
                    }
                }
                return View(user_data);
        }

        [HttpPost]
        public ActionResult edit_data(User_data update_data)
        {
            using(SqlConnection conn = new SqlConnection(connection))
            {
                using(SqlCommand cmd = new SqlCommand("update_user_data", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", update_data.name);
                    cmd.Parameters.AddWithValue ("@email", update_data.email);
                    cmd.Parameters.AddWithValue("@password", update_data.password);
                    cmd.Parameters.AddWithValue("@number",update_data.number);
                    cmd.Parameters.AddWithValue("@id",update_data.Id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Userdashboard");
            }

        }

    }
}