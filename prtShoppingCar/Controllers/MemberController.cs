using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prtShoppingCar.Models;

namespace prtShoppingCar.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {

        dbShoppingCarEntities db = new dbShoppingCarEntities();
        // GET: Member/Index
        public ActionResult Index()
        {
            var products =
                db.tProduct.OrderByDescending(m => m.fId).ToList();

            return View("../Home/Index", "_LayoutMember",products);
        }

        //GET:Member/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        //GET:Member/ShoppingCar
        public ActionResult ShoppingCar()
        {
            string fUserId = User.Identity.Name;
            var orderDetails = db.tOrderDetail.Where(m => m.fUserId == fUserId 
            && m.fIsApproved == "否").ToList();
            return View(orderDetails);
        }

        //GET:Member/AddCar
        public ActionResult AddCar(string fPId)
        {
            string fUserId = User.Identity.Name;
            var currentCar = db.tOrderDetail
                .Where(m => m.fPId == fPId && m.fIsApproved == "否" && m.fUserId == fUserId).FirstOrDefault();

            if(currentCar == null)
            {
                var product = db.tProduct
                    .Where(m => m.fPId == fPId ).FirstOrDefault();
                tOrderDetail orderDetail = new tOrderDetail();
                orderDetail.fUserId = fUserId;
                orderDetail.fPId = fPId;
                orderDetail.fName = product.fName;
                orderDetail.fPrice = product.fPrice;
                orderDetail.fQty = 1;
                orderDetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetail);
            }
            else
            {
                currentCar.fQty += 1;
            }
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }

        //GET:Member/DeleteCar
        public ActionResult DeleteCar(int fId)
        {
            var orderDetail = db.tOrderDetail.Where(m => m.fId == fId).FirstOrDefault();
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }

        //POST:Member/shoppingCar
        [HttpPost]
        public ActionResult ShoppingCar(string fReceiver, string fEmail, string fAddress)
        {
            string fUseId = User.Identity.Name;
            string guid = Guid.NewGuid().ToString();

            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUseId;
            order.fAddress = fAddress;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fDate = DateTime.Now;
            db.tOrder.Add(order);

            var carlist = db.tOrderDetail
                .Where(m => m.fIsApproved == "否" && m.fUserId == fUseId).ToList();

            foreach (var item in carlist)
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        //GET:Member/OrderList
        public ActionResult OrderList()
        {
            string fUserId = User.Identity.Name;
            var orders = db.tOrder.Where(m => m.fUserId == fUserId).OrderByDescending(m => m.fDate)
                .ToList();
            return View(orders);
        }

        //GET:Member/OrderDetail
        public ActionResult OrderDetail(string fOrderGuid)
        {
            var orderDetails = db.tOrderDetail
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            return View(orderDetails);
        }
    }
}