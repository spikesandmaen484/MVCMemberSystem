using Exam.Models;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace Exam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Request.Cookies["keepLogin"] != null) 
            {
                Models.Member member = new Models.Member();
                if (!string.IsNullOrEmpty(Request.Cookies["keepLogin"].Value)) 
                {
                    member.Email = Request.Cookies["keepLogin"].Value;
                }

                return View(member);
            }
            else
                return View();
        }

        [HttpPost]
        public ActionResult Login(Models.LoginValid login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            DBManager dBManager = new DBManager();
            DataTable dt = dBManager.CheckAccount(login);
            if (dt.Rows.Count > 0)
            {
                string name = Convert.ToString(dt.Rows[0]["Name"]);
                if (string.IsNullOrEmpty(name))
                    TempData["Name"] = string.Empty;
                else
                    TempData["Name"] = name;

                var check = Request.Form["remember"];
                if (check == "on")
                {
                    // 將登入帳號記錄在 Session 內
                    Session["UserID"] = login.Email;

                    HttpCookie cookie = new HttpCookie("keepLogin");
                    cookie.Value = login.Email;
                    cookie.Expires = DateTime.Now.AddDays(1);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                }
                else 
                {
                    Session.Clear();
                    //創一個同名COOKIE 蓋過原COOKIE
                    var keep = new HttpCookie("keepLogin");
                    keep.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(keep);
                }
                return RedirectToAction("Index");
            }
            else 
            {
                ViewBag.Error = "帳號或密碼驗證錯誤!";
                return View();
            }

        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.Member member)
        {
            DBManager dBManager = new DBManager();
            if (!ModelState.IsValid)
            {
                return View();
            }
            else if (dBManager.ExistEmail(member.Email))
            {
                ViewBag.Error = "此Email已註冊!";
                return View();
            }
            else if (dBManager.ExistPhone(member.phoneNumber))
            {
                ViewBag.Error = "此手機已註冊!";
                return View();
            }
            else
            {
                TempData["Member"] = member;
                return RedirectToAction("SignUpSuccess");
            }
        }

        public ActionResult SignUpSuccess()
        {
            try 
            {
                var model = TempData["Member"] as Models.Member;
                DBManager dBManager = new DBManager();
                dBManager.AddNewEmail(model);
                return View();
            } 
            catch (Exception ex) 
            {
                return View(ex.Message);
            }
            
        }

        [HttpGet]
        public ActionResult EditPersonalFile()
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            DataTable dt = new DataTable();
            PersonalInfo personalInfo = new PersonalInfo();
            DBManager dBManager = new DBManager();
            dt = dBManager.GetAccountData(ViewBag.Name);
            if (dt.Rows.Count > 0) 
            {
                personalInfo.Email = Convert.ToString(dt.Rows[0]["Email"]);
                personalInfo.RegisterTime = Convert.ToDateTime(dt.Rows[0]["RegisterTime"]);
                personalInfo.Name = Convert.ToString(dt.Rows[0]["Name"]);
                personalInfo.EnglishName = Convert.ToString(dt.Rows[0]["EnglishName"]);

                //儲存原手機號碼
                Session["OriPhone"] = Convert.ToString(dt.Rows[0]["phoneNumber"]);
                personalInfo.phoneNumber = Convert.ToString(dt.Rows[0]["phoneNumber"]);

                personalInfo.Gender = Convert.ToString(dt.Rows[0]["Gender"]);
                personalInfo.birthday = Convert.ToString(dt.Rows[0]["birthday"]);
                personalInfo.Address = Convert.ToString(dt.Rows[0]["Address"]);
                ViewData["IsSubscribe"] = Convert.ToString(dt.Rows[0]["IsSubscribe"]);

                if (dt.Rows[0]["photo"] != DBNull.Value)
                {
                    string path = System.Text.Encoding.UTF8.GetString((byte[])dt.Rows[0]["photo"]);
                    string[] pathArr = path.Split('\\');
                    personalInfo.photo = pathArr[pathArr.Length - 1];
                    Session["path"] = path.Replace(personalInfo.photo, "");
                }
                else 
                {
                    personalInfo.photo = string.Empty;
                    Session["path"] = ConfigurationManager.AppSettings["imgPath"].ToString();
                }
            }

            return View(personalInfo);
        }

        [HttpPost]
        public ActionResult EditPersonalFile(Models.PersonalInfo personalInfo)
        {
            try
            {
                if (Session["Name"] != null)
                    ViewBag.Name = Session["Name"];
                else
                {
                    Session["Name"] = TempData["Name"] as string;
                    ViewBag.Name = TempData["Name"] as string;
                }

                DBManager dBManager = new DBManager();
                string oriPhone = Session["OriPhone"] as string;

                if (!ModelState.IsValid)
                {
                    return View();
                }
                else if (dBManager.ExistPhone(personalInfo.phoneNumber) && personalInfo.phoneNumber != oriPhone)
                {
                    ViewBag.Error = "手機不可與其他已註冊帳號的相同!";
                    return View();
                }

                string imgLocation = Session["path"] + personalInfo.photo;
                byte[] imgBytes = System.Text.Encoding.UTF8.GetBytes(imgLocation);
                var check = Request.Form["chkSubscribe"];
                string chkSubscribe = "N";
                if (check == "on") 
                {
                    chkSubscribe = "Y";
                }
                
                int count = dBManager.EditPersonalInfo(personalInfo, chkSubscribe, imgBytes);
                if (count > 0)
                {
                    string js = "<script language=javascript>alert('{0}');</script>";
                    HttpContext.Response.Write(string.Format(js, "修改成功!"));
                    if (personalInfo.Name != (string)Session["Name"]) 
                    {
                        Session["Name"] = personalInfo.Name;
                        ViewBag.Name = personalInfo.Name;
                    }

                    
                    return RedirectToAction("Index");
                }
                else 
                {
                    ViewBag.Error = "修改失敗!";
                    return View();
                }
                
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
    }
}