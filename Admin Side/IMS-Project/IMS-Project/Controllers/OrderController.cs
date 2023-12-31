using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS_Project.Models;

namespace IMS_Project.Controllers
{
    public class OrderController : Controller
    {
        KahreedoEntities1 db = new KahreedoEntities1();
        // GET: Order
        public ActionResult Index()
        {
            return View(db.Orders.OrderByDescending(x => x.OrderID).ToList());
        }
        public ActionResult Details(int id)
        {
            Order ord = db.Orders.Find(id);
            var Ord_details = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            var tuple = new Tuple<Order, IEnumerable<OrderDetail>>(ord, Ord_details);

            double SumAmount = Convert.ToDouble(Ord_details.Sum(x => x.TotalAmount));
            ViewBag.TotalItems = Ord_details.Sum(x => x.Quantity);
            ViewBag.Discount = ord.Discount;
            ViewBag.TAmount = SumAmount - 0;
            ViewBag.Amount = SumAmount;
            return View(tuple);
        }
        public ActionResult Edit(int id)
        {
            // Retrieve the order from the database using the provided id
            Order order = db.Orders.Find(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Pass the order to the view for editing
            return View("Edit",order);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                // Update the order in the database
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Redirect to the Index action after editing
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return to the Edit view with the current order
            return View(order);






        }
    }
}