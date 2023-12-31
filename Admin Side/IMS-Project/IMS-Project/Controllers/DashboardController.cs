using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS_Project.Models;
using System.Data.Objects;
using System.Data.Entity;

namespace IMS_Project.Controllers
{
    public class DashboardController : Controller
    {
        KahreedoEntities1 db = new KahreedoEntities1();
        public ActionResult Index() 
        {          

            ViewBag.latestOrders = db.Orders.OrderByDescending(x => x.OrderID).Take(10).ToList();
            
            ViewBag.NewOrders = db.Orders.Where(a => a.Notes == null)
    .Count();
            
            ViewBag.DispatchedOrders = db.Orders.Where(a => a.DIspatched == true).Count();
            ViewBag.ShippingOrders = db.Orders.Where(a => a.Shipped == true).Count();
            ViewBag.DeliveredOrders = db.Orders.Where(a => a.Notes.Equals("Completed")).Count();



            return View();
        }
        
        //Area Grap
        public JsonResult GetSalesPerDay()
        {
            var data = (from O in db.Orders 
                            //select new { date = EntityFunctions.TruncateTime(O.OrderDate), O.TotalAmount } into a
                        select new { date = DbFunctions.TruncateTime(O.OrderDate), O.TotalAmount } into a
                        group a by a.date into b
                        select new
                        {
                            period = b.Key,
                            sales = b.Sum(x => x.TotalAmount)
                        });

            List<AreaCharts> aa = new List<AreaCharts>();
            foreach (var item in data)
            {
                string date = item.period.ToString().Split(new[] { (' ') }, StringSplitOptions.None)[0];
                aa.Add(new AreaCharts() { period = date, sales = item.sales.GetValueOrDefault() });
            }
            return Json(aa, JsonRequestBehavior.AllowGet);
        }

        //Circle Graph
        public JsonResult GetTopProductSales()
        {
            var dataforchart = (from OD in db.OrderDetails
                                join P in db.Products
                                on OD.ProductID equals P.ProductID
                                select new { P.Name, OD.Quantity } into row
                                group row by row.Name into g
                                select new
                                {
                                    label = g.Key,
                                    value = g.Sum(x => x.Quantity)
                                })
                    .OrderByDescending(x => x.value)
                    .Take(3);
            return Json(dataforchart, JsonRequestBehavior.AllowGet);
        }
        

        //Line Grap
        public JsonResult GetOrderPerDay()
        {
                var data = from O in db.Orders
                               //select new { Odate = EntityFunctions.TruncateTime(O.OrderDate), O.OrderID } into g
                           select new { Odate = DbFunctions.TruncateTime(O.OrderDate), O.OrderID, O.TotalAmount } into g
                           group g by g.Odate into col
                           select new
                           {
                               Order_Date = col.Key,
                               Count = col.Count(y => y.OrderID != null)

                           };
            List<LineCharts> aa = new List<LineCharts>();
            foreach (var item in data)
            {
                string date = item.Order_Date.ToString().Split(new[] { (' ') }, StringSplitOptions.None)[0];
                aa.Add(new LineCharts() { Date = date, Orders = item.Count });
            }
            return Json(aa, JsonRequestBehavior.AllowGet);

           

        }
        [HttpPost]
        public ActionResult CompleteOrder(int orderId)
        {
         
                var order = db.Orders.Find(orderId);
                order.Notes = "Completed";
                order.DIspatched = false;
                order.Shipped = false;
                order.Deliver = false;

                // Save changes to the database
                db.SaveChanges();
            //send email for customer
            //var strSanPham = "";
            //decimal thanhtien = 0;
            //var tongtien = decimal.Zero;
            //foreach (var sp in order.OrderDetails)
            //{
            //    strSanPham += "<tr>";
            //    strSanPham += "<td>" + sp.Product.Name + "</td>";
            //    strSanPham += "<td>" + sp.Quantity + "</td>";
            //    strSanPham += "<td>" + sp.TotalAmount + "</td>";
            //    strSanPham += "</tr>";
            //    thanhtien = (sp.UnitPrice ?? 0) * (sp.Quantity ?? 0);
            //    //thanhtien = temp ?? 0;



            //}
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));

            contentCustomer = contentCustomer.Replace("{{MaDon}}", order.OrderID.ToString());
            //contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.OrderDate.ToString());
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.Customer.First_Name);
            //contentCustomer = contentCustomer.Replace("{{HoKhachHang}}", order.Customer.Last_Name);
            contentCustomer = contentCustomer.Replace("{{Phone}}", order.Customer.Mobile1);
            contentCustomer = contentCustomer.Replace("{{Email}}", order.Customer.Email);
            contentCustomer = contentCustomer.Replace("{{DiaChi}}", order.Customer.Address1);


            //contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhtien.ToString());
            contentCustomer = contentCustomer.Replace("{{TongTien}}", order.TotalAmount.ToString());
            Khareedo.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.OrderID, contentCustomer, order.Customer.Email);



            //send email for customer
            // Redirect back to the Index action or any other action as needed
            return RedirectToAction("Index");
           
           
        }

        //Bar Grap
        public JsonResult GetSalesPerCountry()
        {
            var dataforBarchart = (from O in db.Orders
                                join C in db.Customers
                                on O.CustomerID equals C.CustomerID
                                select new { C.Country,O.TotalAmount } into row
                                group row by row.Country into g
                                select new
                                {
                                    Country = g.Key,
                                    Sales_Amount = g.Sum(x => x.TotalAmount)
                                })
                              .OrderByDescending(x => x.Sales_Amount)
                              .Take(6);
            return Json(dataforBarchart, JsonRequestBehavior.AllowGet);
        }
        

    }
}