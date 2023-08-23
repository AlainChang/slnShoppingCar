using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prtShoppingCar.Models;

namespace prtShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        public ActionResult Index()
        {
            var product = db.tProduct
                .OrderByDescending(m => m.fId).ToList();
            return View(product);
        }

        //Get:Home/Login
        public ActionResult Login()
        {
            return View();
        }
        //Post:Home/Login
        [HttpPost]
        public ActionResult Login(string fUseId, string fPwd)
        {
            var member = db.tMember
                .Where(m=>m.fUseId==fUseId&&m.fPwd==fPwd)
                .FirstOrDefault();
            
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }

            //session
            Session["WelCome"] = member.fName+ "歡迎光臨";
            FormsAuthentication.RedirectFromLoginPage(fUseId, true);
            return RedirectToAction("Index", "Member");
        }

        //Get:Home/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public  ActionResult Register(tMember pMember)
        {
            if(ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.tMember
                .Where(m => m.fUseId == pMember.fUseId)
                .FirstOrDefault();
                
            if(member==null)
            {
                db.tMember.Add(pMember);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號已有人使用，註冊失敗";
            return View();
        }
    }
}