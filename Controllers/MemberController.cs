using Exam.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace Exam.Controllers
{
    public class MemberController : Controller
    {
        [HttpGet]
        public ActionResult Search()
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            ViewData["Num"] = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Search(Search search)
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            DBManager dBManager = new DBManager();
            DataTable dt = new DataTable();
            int i = 1;
            dt = dBManager.GetMemberData(search);
            if (dt.Rows.Count > 0) 
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewData["Age" + i.ToString()] = Convert.ToString(dr["Age"]);
                    ViewData["Name" + i.ToString()] = Convert.ToString(dr["Name"]);
                    ViewData["phoneNumber" + i.ToString()] = Convert.ToString(dr["phoneNumber"]);
                    ViewData["Email" + i.ToString()] = Convert.ToString(dr["Email"]);
                    ViewData["Num"] = i++;
                }
            }
            
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            var email = Request.QueryString["value"];
            if (email != null)
            {
                DataTable dt = new DataTable();
                Member member = new Member();
                DBManager dBManager = new DBManager();
                dt = dBManager.GetAccountDataByEmail(email);
                if (dt.Rows.Count > 0)
                {
                    //儲存原Email
                    Session["OriEmail"] = Convert.ToString(dt.Rows[0]["Email"]);
                    member.Email = Convert.ToString(dt.Rows[0]["Email"]);

                    member.Gender = Convert.ToString(dt.Rows[0]["Gender"]);
                    member.Name = Convert.ToString(dt.Rows[0]["Name"]);

                    //儲存原身分證字號
                    Session["OriID"] = Convert.ToString(dt.Rows[0]["ID"]);
                    member.ID = Convert.ToString(dt.Rows[0]["ID"]);

                    //儲存原手機號碼
                    Session["OriPhone"] = Convert.ToString(dt.Rows[0]["phoneNumber"]);
                    member.phoneNumber = Convert.ToString(dt.Rows[0]["phoneNumber"]);

                    member.School = Convert.ToString(dt.Rows[0]["School"]);
                    member.birthday = Convert.ToString(dt.Rows[0]["birthday"]);
                    member.Address = Convert.ToString(dt.Rows[0]["Address"]);
                    member.Department = Convert.ToString(dt.Rows[0]["Department"]);
                }

                ViewData["approach"] = "Update";
                ViewData["btnName"] = "修改";
                return View(member);
            }
            else 
            {
                ViewData["btnName"] = "新增";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(Models.Member member)
        {
            try
            {
                DBManager dBManager = new DBManager();
                string oriPhone = Session["OriPhone"] as string;
                string oriEmail = Session["OriEmail"] as string;
                string oriID = Session["OriID"] as string;

                if (!ModelState.IsValid)
                {
                    return View();
                }
                else if (dBManager.ExistPhone(member.phoneNumber) && member.phoneNumber != oriPhone)
                {
                    ViewBag.Error = "手機不可與其他已註冊帳號的相同!";
                    return View();
                }
                else if (dBManager.ExistEmail(member.Email))
                {
                    ViewBag.Error = "Email不可與其他已註冊的相同!";
                    return View();
                }
                else if (dBManager.ExistID(member.ID) && member.ID != oriID)
                {
                    ViewBag.Error = "身分證字號不可與其他已註冊的相同!";
                    return View();
                }

                //計算年紀
                string[] strArr = member.birthday.Split('/');
                string strTime = strArr[1] + "/" + strArr[2] + "/" + strArr[0];
                DateTime birthDay = Convert.ToDateTime(strTime);
                int age = DateTime.Now.Year - birthDay.Year;
                if (DateTime.Now.Month < birthDay.Month || (DateTime.Now.Month == birthDay.Month && DateTime.Now.Date < birthDay.Date)) 
                {
                    age--;
                }
                member.Age = age < 0 ? 0 : age;

                var check = Request.Form["hidCheck"];
                int count = 0;
                if (check == "Update")
                {
                    count = dBManager.UpdateMember(member, oriEmail);
                    string js = "<script language=javascript>alert('{0}');</script>";
                    HttpContext.Response.Write(string.Format(js, "修改成功!"));
                }
                else 
                {
                    count = dBManager.AddNewMember(member);
                    string js = "<script language=javascript>alert('{0}');</script>";
                    HttpContext.Response.Write(string.Format(js, "新增成功!"));
                }

                return RedirectToAction("Index", "Home", new { area = ""});
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

    }
}
