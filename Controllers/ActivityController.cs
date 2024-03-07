using Exam.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exam.Controllers
{
    public class ActivityController : Controller
    {
        [HttpGet]
        public ActionResult ActivityRead()
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
        public ActionResult ActivityRead(Models.Activity activity)
        {
            if (Session["Name"] != null)
                ViewBag.Name = Session["Name"];
            else
            {
                Session["Name"] = TempData["Name"] as string;
                ViewBag.Name = TempData["Name"] as string;
            }

            DBManager dBManager = new DBManager();

            string errMsg = dBManager.CheckTime(activity.ActivityDateStart, activity.ActivityDateEnd);
            if (!string.IsNullOrEmpty(errMsg))
            {
                ViewData["Num"] = 0;
                ViewBag.Error = errMsg;
                return View();
            }

            DataTable dt = new DataTable();
            int i = 0;
            dt = dBManager.GetActivity(activity);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewData["Name" + i.ToString()] = Convert.ToString(dr["Name"]);
                    ViewData["Start" + i.ToString()] = Convert.ToString(dr["ActivityDateStart"]);
                    ViewData["End" + i.ToString()] = Convert.ToString(dr["ActivityDateEnd"]);
                    ViewData["Fee" + i.ToString()] = Convert.ToString(dr["Fee"]);
                    string[] strArr = Convert.ToString(dr["attendance"]).Split(',');
                    ViewData["Amount" + i.ToString()] = strArr.Length;
                    i++;
                }
            }

            ViewData["Num"] = dt.Rows.Count;
            return View();
        }

        [HttpGet]
        public ActionResult ActivityCreate()
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
            dt = dBManager.GetAllMemberName();
            string optionStr = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                optionStr += "<option value='" + Convert.ToString(dr["Name"]) + "' >" + Convert.ToString(dr["Name"]) + "</option>";
            }

            ViewBag.items = optionStr;
            return View();
        }

        [HttpPost]
        public ActionResult ActivityCreate(Models.Activity activity)
        {
            try
            {
                DBManager dBManager = new DBManager();
                string errMsg = string.Empty;

                if (!ModelState.IsValid)
                {
                    return View();
                }

                errMsg = dBManager.CheckTime(activity.ActivityDateStart, activity.ActivityDateEnd);
                if (!string.IsNullOrEmpty(errMsg))
                {
                    ViewBag.Error = errMsg;
                    return View();
                }

                string[] strDateS = activity.ActivityDateStart.Split('/');
                string[] strDateE = activity.ActivityDateEnd.Split('/');
                DateTime dateTimeS = Convert.ToDateTime(strDateS[1] + "/" + strDateS[2] + "/" + strDateS[0]);
                DateTime dateTimeE = Convert.ToDateTime(strDateE[1] + "/" + strDateE[2] + "/" + strDateE[0]);

                if (dBManager.ExistPhone(activity.Name))
                {
                    ViewBag.Error = "此活動名稱已註冊!";
                    return View();
                }
                else if (dateTimeS > dateTimeE) 
                {
                    ViewBag.Error = "活動日期起日需小於訖日!";
                    return View();
                }

                var selValue = Request.Form["selMember"];
                int count = dBManager.AddNewActivity(activity, selValue);
                string js = "<script language=javascript>alert('{0}');</script>";
                HttpContext.Response.Write(string.Format(js, "活動新增成功!"));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

    }
}
