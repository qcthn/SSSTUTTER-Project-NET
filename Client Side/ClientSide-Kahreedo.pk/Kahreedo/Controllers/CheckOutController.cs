using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Khareedo.Models;
using System.Data;
using System.Web.UI;

namespace Khareedo.Controllers
{
    public class CheckOutController : Controller
    {
        KahreedoEntities db = new KahreedoEntities();
        
        // GET: CheckOut
        public ActionResult Index()
        {
            ViewBag.PayMethod = new SelectList(db.PaymentTypes, "PayTypeID", "TypeName");
            
               
            var data=this.GetDefaultData();
         
            return View(data);
        }

        
        //PLACE ORDER--LAST STEP
        public ActionResult PlaceOrder(FormCollection getCheckoutDetails)
        {            

                int shpID = 1;
                if (db.ShippingDetails.Count() > 0)
                {
                    shpID = db.ShippingDetails.Max(x => x.ShippingID) + 1;
                }
                int payID = 1;
                if (db.Payments.Count() > 0)
                {
                    payID = db.Payments.Max(x => x.PaymentID) + 1;
                }
                int orderID = 1;
                if (db.Orders.Count() > 0)
                {
                    orderID = db.Orders.Max(x => x.OrderID) + 1;
                }



                ShippingDetail shpDetails = new ShippingDetail();
                shpDetails.ShippingID = shpID;
                shpDetails.FirstName = getCheckoutDetails["FirstName"];
                shpDetails.LastName = getCheckoutDetails["LastName"];
                shpDetails.Email = getCheckoutDetails["Email"];
                shpDetails.Mobile = getCheckoutDetails["Mobile"];
                shpDetails.Address = getCheckoutDetails["Address"];
                shpDetails.Province = getCheckoutDetails["Province"];
                shpDetails.City = getCheckoutDetails["City"];
                shpDetails.PostCode = getCheckoutDetails["PostCode"];
                db.ShippingDetails.Add(shpDetails);
                db.SaveChanges();

                Payment pay = new Payment();
                pay.PaymentID = payID;
                pay.Type = Convert.ToInt32(getCheckoutDetails["PayMethod"]);
                db.Payments.Add(pay);
                db.SaveChanges();

                Order o = new Order();
                o.OrderID = orderID;
                o.CustomerID = TempShpData.UserID;
                o.PaymentID = payID;
                o.ShippingID = shpID;
                o.Discount = Convert.ToInt32( getCheckoutDetails["discount"]);
                o.TotalAmount = Convert.ToInt32( getCheckoutDetails["totalAmount"]);
                o.isCompleted = true;
                o.OrderDate = DateTime.Now;
                db.Orders.Add(o);
                db.SaveChanges();
            // send email
            var strSanPham ="";
            decimal thanhtien = 0;
            var tongtien = decimal.Zero;
            foreach (var sp in TempShpData.items) {
                strSanPham += "<tr>";
                strSanPham += "<td>"+sp.Product.Name+ "</td>";
                strSanPham += "<td>"+sp.Quantity+ "</td>";
                strSanPham += "<td>" + sp.TotalAmount + "</td>";
                strSanPham += "</tr>";
                thanhtien = (sp.UnitPrice ?? 0) * (sp.Quantity ?? 0);
                //thanhtien = temp ?? 0;



            }
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));

            contentCustomer = contentCustomer.Replace("{{MaDon}}", o.OrderID.ToString());
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", o.OrderDate.ToString());
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", shpDetails.FirstName);
            contentCustomer = contentCustomer.Replace("{{Phone}}", shpDetails.Mobile);
            contentCustomer = contentCustomer.Replace("{{Email}}", shpDetails.Email);
            contentCustomer = contentCustomer.Replace("{{DiaChi}}", shpDetails.Address);


            contentCustomer = contentCustomer.Replace("{{ThanhTien}}",thanhtien.ToString());
            contentCustomer = contentCustomer.Replace("{{TongTien}}", o.TotalAmount.ToString());
            Khareedo.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + o.OrderID, contentCustomer, shpDetails.Email);




            // send email
            foreach (var OD in TempShpData.items)
                {
                    OD.OrderID = orderID;
                    OD.Order = db.Orders.Find(orderID);
                    OD.Product = db.Products.Find(OD.ProductID);
                    db.OrderDetails.Add(OD);
                    db.SaveChanges();
                }

               
                return RedirectToAction("Index","ThankYou");
            
        }

    }
}