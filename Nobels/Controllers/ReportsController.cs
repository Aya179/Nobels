using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nobels.Models;
using System;
using System.Security.Cryptography;
using Nobels.Models;

namespace Nobels.Controllers
{

    //BranchReportAsync:التوابع بدون 1 خاصين بداشبورد المببع والعروض
    //BranchReport1:يلي فين1 خاصين بالداشبورد الرئيسية
    public class ReportsController : Controller
    {

        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;


        public ReportsController(db_a8d242_exadecor2Context context, UserManager<Employee> userManager)
        {
            _context = context;
            this.userManager = userManager;


        }
        public IActionResult BranchReportView()
        {

            ViewData["Branch"] = new SelectList(_context.Branches, "BranchId", "BranchName");

            return View();

        }

        public async Task<IActionResult> BranchReportAsync( DateTime startdate, DateTime enddate)
        {

            Employee user = await userManager.GetUserAsync(HttpContext.User);

            int id = (int)user.BranchId;


            ViewData["Branch"] = new SelectList(_context.Branches, "BranchId", "BranchName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (qutation.BranchId == id) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId=qutation.QuotationId

            //             });



            //var res = query.ToList();


            //return Json(res);


            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.BranchId == id) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();

            return Json(query);





        }

        public IActionResult BranchReport1(int id,DateTime startdate,DateTime enddate)
        {

            ViewData["Branch"] = new SelectList(_context.Branches, "BranchId", "BranchName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (qutation.BranchId == id)&& (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId = qutation.QuotationId

            //             });



            //var res = query.ToList();

            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where  (qu.BranchId == id) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();



            return Json(query);


        }

        public IActionResult CustomerReportView()
        {

            ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");

            return View();

        }

        public async Task<IActionResult> CustomerReportAsync(int id, DateTime startdate, DateTime enddate)
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);

            int bid = (int)user.BranchId;


            ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (customer.Id== id) &&(qutation.BranchId==bid) &&(qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId = qutation.QuotationId

            //             });



            //var res = query.ToList();
            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.CustomerId == id) && (qu.BranchId == bid) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
  into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date=quo.QuotationDate
                        }
                        ;
            var x = query.ToList();




            return Json(query);


        }
        public IActionResult CustomerReport1(int id,DateTime startdate,DateTime enddate)
        {

            ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where customer.Id == id && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId = qutation.QuotationId

            //             });



            //var res = query.ToList();
            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.CustomerId == id)&& (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();




            return Json(query);


        }
        public async Task<IActionResult> CustomerPhoneReportAsync(string phone, DateTime startdate, DateTime enddate)
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);

            int bid = (int)user.BranchId;


            //  ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where ((customer.Phone == phone || customer.PhonNumber == phone )) && (qutation.BranchId == bid) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId = qutation.QuotationId

            //             });



            //var res = query.ToList();



            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      join custom in _context.Customers on qu.CustomerId equals custom.Id
                      where (custom.Phone == phone || custom.PhonNumber == phone) && (qu.BranchId == bid) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();





            return Json(query);


        }

        public IActionResult CustomerPhoneReport1(string phone,DateTime startdate , DateTime enddate)
        {

            ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (customer.Phone == phone || customer.PhonNumber == phone) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,
            //                 quationId = qutation.QuotationId

            //             });



            //var res = query.ToList();

            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      join custom in _context.Customers on qu.CustomerId equals custom.Id
                      where (custom.Phone == phone || custom.PhonNumber == phone) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();



            return Json(query);


        }

        public IActionResult CityReportView()
        {

            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");

            return View();

        }

        public IActionResult CityReport(int id)
        {

            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            var query = (from qutation in _context.Quotations
                         join branch in _context.Branches on qutation.BranchId equals branch.BranchId
                         join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
                         join item in _context.Items on order.ItemId equals item.ItemId
                         join city in _context.Cities on branch.CityId equals city.CityId

                         join customer in _context.Customers on qutation.CustomerId equals customer.Id

                         where  city.CityId == id 
                         select new
                         BillModel
                         {
                             Customer = customer.ArabicName,
                             itemname = item.ItemArName,
                             quantity =order.Quantity,
                             totalValue = order.TotalValue,
                             
                             CityName=city.CityName
                         });



            var res = query.ToList();


            return Json(res);


        }


        public IActionResult CityReport1(int id,DateTime startdate,DateTime enddate)
        {

            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId
            //             join city in _context.Cities on branch.CityId equals city.CityId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where city.CityId == id && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,

            //                 CityName = city.CityName
            //             });



            //var res = query.ToList();


            //return Json(res);


            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      join branch in _context.Branches on qu.BranchId equals branch.BranchId
                      join city in _context.Cities on branch.CityId equals city.CityId
                      where (city.CityId == id) &&(qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();


            return Json(query);


        }

        public IActionResult ManegerReportView()
        {

            var list = _context.Users.Where(i=>i.RoleId==4) ;



            return View();

        }
        public IActionResult ManegerReport1(int id, DateTime startdate, DateTime enddate)
        {

            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId
            //             join city in _context.Cities on branch.CityId equals city.CityId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (qutation.employeeId == id) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,

            //                 CityName = city.CityName
            //             });



            //var res = query.ToList();



            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.employeeId == id) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
          into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();




            return Json(query);



        }
        public IActionResult ManegerReport(int id, DateTime startdate, DateTime enddate)
        {
            
            var query = (from qutation in _context.Quotations
                         join branch in _context.Branches on qutation.BranchId equals branch.BranchId
                         join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
                         join item in _context.Items on order.ItemId equals item.ItemId
                         join city in _context.Cities on branch.CityId equals city.CityId

                         join customer in _context.Customers on qutation.CustomerId equals customer.Id

                         where (qutation.employeeId == id) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
                         select new
                         BillModel
                         {
                             Customer = customer.ArabicName,
                             itemname = item.ItemArName,
                             quantity = order.Quantity,
                             totalValue = order.TotalValue,

                             CityName = city.CityName
                         });



            var res = query.ToList();


            return Json(res);



        }
        public IActionResult ListOfUsers()
        {


            var list = _context.Users.Where(i => i.RoleId == 3);



            return Json(list);

        }

        public async Task<IActionResult> ListOfUsers1Async()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);

            int bid = (int)user.BranchId;




            var list = _context.Users.Where(i => i.RoleId == 4&&i.BranchId==bid);



            return Json(list);

        }


        public IActionResult AllSelesReportView()
        {
            return View();
        }

        public async Task<IActionResult> AllSelesReportAsync(DateTime startdate, DateTime enddate)
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);

            int uid = (int)user.Id;
            int bid = (int)user.BranchId;


            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId
            //             join city in _context.Cities on branch.CityId equals city.CityId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (qutation.QuotationId == order.QoutationId)&&(qutation.employeeId==uid) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,

            //                 CityName = city.CityName
            //             });



            //var res = query.ToList();


            //return Json(res);


            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.employeeId == uid && qu.BranchId==bid) && (qu.QuotationId==od.QoutationId) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();

            return Json(query);


        }

        public IActionResult AllSelesReport1(DateTime startdate,DateTime enddate)
        {
            //var query = (from qutation in _context.Quotations
            //             join branch in _context.Branches on qutation.BranchId equals branch.BranchId
            //             join order in _context.OrderDetails on qutation.QuotationId equals order.QoutationId
            //             join item in _context.Items on order.ItemId equals item.ItemId
            //             join city in _context.Cities on branch.CityId equals city.CityId

            //             join customer in _context.Customers on qutation.CustomerId equals customer.Id

            //             where (qutation.QuotationId == order.QoutationId) && (qutation.QuotationDate >= startdate && qutation.QuotationDate <= enddate)
            //             select new
            //             BillModel
            //             {
            //                 Customer = customer.ArabicName,
            //                 itemname = item.ItemArName,
            //                 quantity = order.Quantity,
            //                 totalValue = order.TotalValue,

            //                 CityName = city.CityName
            //             });



            //var res = query.ToList();

            var sub = from od in _context.OrderDetails
                      join qu in _context.Quotations on od.QoutationId equals qu.QuotationId
                      where (qu.QuotationId == od.QoutationId) && (qu.QuotationDate >= startdate && qu.QuotationDate <= enddate)
                      group od by od.QoutationId
into v
                      select new
                      {
                          qid = v.Key,
                          value = v.Sum(x => x.TotalValue)


                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join quo in _context.Quotations on Customer.Id equals quo.CustomerId
                        join vs in sub on quo.QuotationId equals vs.qid



                        select new
                        {
                            name = Customer.ArabicName,
                            id = vs.qid,
                            total = vs.value,
                            date = quo.QuotationDate
                        }
                        ;
            var x = query.ToList();

            return Json(query);


        }
    }
}
