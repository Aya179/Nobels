using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Nobels.Models;

using System.Data;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Nobels.Controllers
{
    [Authorize]

    public class ProductionController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        IConfiguration configuration;
        IWebHostEnvironment hostEnvironment;
        IExcelDataReader reader;


        public ProductionController(db_a8d242_exadecor2Context context, UserManager<Employee> userManager, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult schedule_the_production1()
        {
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            var query = (from installReq in _context.InstallationRequests
                         //join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId

                         where !_context.ProductionSchedules.Any(c => c.IntallationRequestId == installReq.Id) && installReq.DesiredDate>=DateTime.Now



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             IntallationRequestId = installReq.Id,
                             quotation_date=quot.QuotationDate.Value.ToShortDateString()



                         });
      



            var res = query.ToList();

            return View(res);
        }
        public IActionResult getCityBranches(int id)
        {
            var branches=_context.Branches.Where(b=>b.CityId==id);
            return Json(new { branches});
        }
        public IActionResult schedule_the_production2(int instReqId)
        {
            var instalReq = _context.InstallationRequests.Find(instReqId);
            var quot = _context.Quotations.Where(c => c.QuotationId == instalReq.QutationId).First();
            var address = _context.Addresses.Find(quot.addressId);
            var Customer = _context.Customers.Find(quot.CustomerId);
            var Branch = _context.Branches.Find(quot.BranchId);
            var city = _context.Cities.Find(address.cityid);
            var date2 = _context.QoutationUpdates.Where(q=>q.QoutationId==quot.QuotationId&&q.Updates.Contains("تم قبول العرض فنياً")).FirstOrDefault();
            
            ViewBag.customer = Customer.ArabicName;
            ViewBag.Branch = Branch.BranchName;
            ViewBag.quoID = quot.QuotationId;
            ViewBag.city = city.CityName;
            ViewBag.neigh = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.housenumber = address.houseNumber;
            ViewBag.reqid = instReqId;
            ViewBag.code = quot.Code;
            ViewBag.date = instalReq.DesiredDate.Value.ToShortDateString();
            if(date2!=null)
            ViewBag.date2 = date2.ChangeDate.Value.ToShortDateString();
            else
            {
                ViewBag.date2 = "2023-08-02";
            }
            var select = (from quUpdate in _context.QoutationUpdates
                          join em in _context.Users on quUpdate.EmployeeId equals em.Id
                          where quUpdate.QoutationId == instalReq.QutationId
                          select new productionClass
                          {
                              name = em.UserName,
                              note = quUpdate.Updates
                          }

                       ).ToList();
            return View(select);
        }
        public IActionResult Editschedule_the_production(int instReqId)
        {
            var instalReq = _context.InstallationRequests.Find(instReqId);
            var quot = _context.Quotations.Where(c => c.QuotationId == instalReq.QutationId).First();
            var address = _context.Addresses.Find(quot.addressId);
            var Customer = _context.Customers.Find(quot.CustomerId);
            var Branch = _context.Branches.Find(quot.BranchId);
            var city = _context.Cities.Find(address.cityid);
            var production = _context.ProductionSchedules.Where(p => p.IntallationRequestId == instReqId && p.IsConfirmed == 0).OrderByDescending(p => p.ProductionDate).First();
            if (production != null)
            {
                ViewBag.date = production.ProductionDate.Value.ToShortDateString();
            }
            ViewBag.customer = Customer.ArabicName;
            ViewBag.Branch = Branch.BranchName;
            ViewBag.quoID = quot.QuotationId;
            ViewBag.city = city.CityName;
            ViewBag.neigh = address.neighborhood;
            ViewBag.reqid = instReqId;
            ViewBag.code = quot.Code;
            var techOffice = _context.QoutationUpdates.Where(q => q.QoutationId == quot.QuotationId && q.Updates.Contains("تم قبول العرض فنياً")).FirstOrDefault();
            if (techOffice != null)
            {
                ViewBag.tech = techOffice.ChangeDate.Value.ToShortDateString();
            }
            else { ViewBag.tech = "-"; }
            //var FinanOffice = _context.QoutationUpdates.Where(q => q.QoutationId == id && q.Updates.Contains("تم قبول العرض مالياً")).FirstOrDefault();
            //if (FinanOffice != null)
            //{
            //    ViewBag.finan = FinanOffice.Updates;
            //}
            //else { ViewBag.finan = "-"; }
            return View();
        } 
        public IActionResult ConfirmProductionschedule(int instReqId)
        {
            var instalReq = _context.InstallationRequests.Find(instReqId);
            var quot = _context.Quotations.Where(c => c.QuotationId == instalReq.QutationId).First();
            var address = _context.Addresses.Find(quot.addressId);
            var Customer = _context.Customers.Find(quot.CustomerId);
            var Branch = _context.Branches.Find(quot.BranchId);
            var city = _context.Cities.Find(address.cityid);
            var date2 = _context.QoutationUpdates.Where(q => q.QoutationId == quot.QuotationId && q.Updates.Contains("تم قبول العرض فنياً")).FirstOrDefault();

            var production = _context.ProductionSchedules.Where(p=>p.IntallationRequestId==instReqId&&p.IsConfirmed==0).OrderByDescending(p=>p.ProductionDate).First();
            if (production != null)
            {
                ViewBag.date = production.ProductionDate.Value.ToShortDateString();
            }
            ViewBag.neig = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.housenumber = address.houseNumber;
            ViewBag.city = city.CityName;
            ViewBag.customer = Customer.ArabicName;
            ViewBag.Branch = Branch.BranchName;
            ViewBag.quoID = quot.QuotationId;
            ViewBag.city = city.CityName;
            ViewBag.neigh = address.neighborhood;
            ViewBag.reqid = instReqId; 
            ViewBag.code = quot.Code;
            ViewBag.date1 = instalReq.DesiredDate.Value.ToShortDateString();
            if (date2 != null)
                ViewBag.date2 = date2.ChangeDate.Value.ToShortDateString();
            else
            {
                ViewBag.date2 = "2023-08-02";
            }
            return View();
        }
        public IActionResult ConfirmProductionscheduleForm(DateTime selectedDate, int reqId, string? notes)
        {
            if (notes == null || notes == string.Empty)
            {
                ProductionSchedule schedule = new ProductionSchedule();
                schedule.ProductionDate = selectedDate;
                schedule.IsConfirmed = 1;
                schedule.IntallationRequestId = reqId;
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                schedule.EmployeeId = Int32.Parse(userId);
                schedule.AddedDate = DateTime.Now;
                _context.Add(schedule);
                _context.SaveChanges();
            }
            else
            {
                ProductionSchedule schedule = new ProductionSchedule();
                schedule.ProductionDate = selectedDate;
                schedule.IsConfirmed = 1;
                schedule.IntallationRequestId = reqId;
                schedule.AddedDate = DateTime.Now;

                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                schedule.EmployeeId = Int32.Parse(userId);
                _context.Add(schedule);
                _context.SaveChanges();
                var installationRequest = _context.InstallationRequests.Find(reqId);
                QoutationUpdate qoutationUpdate = new QoutationUpdate();
                qoutationUpdate.EmployeeId = Int32.Parse(userId);
                qoutationUpdate.ChangeDate = DateTime.Now;
                qoutationUpdate.Updates = notes + "-";
                qoutationUpdate.QoutationId = installationRequest.QutationId;
                _context.Add(qoutationUpdate);
                _context.SaveChanges();
            }

            return RedirectToAction("schedule_the_production3");
        }
        public IActionResult Editschedule_the_productionForm(DateTime selectedDate, int reqId,string ?notes)
        {
           

            if(notes == null|| notes == string.Empty)
            {
                ProductionSchedule schedule = new ProductionSchedule();
                schedule.ProductionDate = selectedDate;
                schedule.IsConfirmed = 1;
                schedule.IntallationRequestId = reqId;
                schedule.AddedDate = DateTime.Now;

                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                schedule.EmployeeId = Int32.Parse(userId);
                _context.Add(schedule);
                _context.SaveChanges();
            }
           // if (notes != null||notes!=""||notes!=string.Empty)
           else { 
                ProductionSchedule schedule = new ProductionSchedule();
            schedule.ProductionDate = selectedDate;
            schedule.IsConfirmed = 1;
            schedule.IntallationRequestId = reqId;
                schedule.AddedDate = DateTime.Now;

                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            schedule.EmployeeId = Int32.Parse(userId);
            _context.Add(schedule);
            _context.SaveChanges();

            var installationRequest = _context.InstallationRequests.Find(reqId);
                QoutationUpdate qoutationUpdate = new QoutationUpdate();
            qoutationUpdate.EmployeeId = Int32.Parse(userId);
            qoutationUpdate.ChangeDate = DateTime.Now;
                qoutationUpdate.Updates = notes + "-"   ;
                qoutationUpdate.QoutationId = installationRequest.QutationId;
            _context.Add(qoutationUpdate);
            _context.SaveChanges();
            }

           
           

            return RedirectToAction("schedule_the_production3");
        }
        public IActionResult schedule_the_production2Form(DateTime selectedDate,int reqId, string notes)
        {
            var inst = _context.InstallationRequests.Find(reqId);
            ProductionSchedule schedule = new ProductionSchedule();
            schedule.ProductionDate = selectedDate;
            schedule.IsConfirmed = 0;
            schedule.IntallationRequestId = reqId;
            schedule.AddedDate = DateTime.Now;

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            schedule.EmployeeId = Int32.Parse(userId);
            _context.Add(schedule);
            _context.SaveChanges();
            
            if (notes != null)
            {
                QoutationUpdate qoutationUpdate = new QoutationUpdate();
                qoutationUpdate.QoutationId = inst.QutationId;
                qoutationUpdate.Updates = notes;
                qoutationUpdate.EmployeeId = Int32.Parse(userId);
                qoutationUpdate.ChangeDate = DateTime.Now;
                _context.Add(qoutationUpdate);
                _context.SaveChanges();
            }
            


           

            return RedirectToAction("schedule_the_production1");
        } 
        //Not used yet
        public IActionResult schedule_the_production2Api(DateTime selectedDate)
        {
            var query = (from installReq in _context.InstallationRequests
                         join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId

                         where productionSched.ProductionDate==selectedDate


                         select new 
                         {
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             ProductionDate = productionSched.ProductionDate,
                             ProductionSheduleId = productionSched.Id,
                             IntallationRequestId = installReq.Id
                         }).ToList();


            var result = from queryResult in query

                       group queryResult by queryResult.IntallationRequestId into a
                       select a.OrderByDescending(g => g.ProductionSheduleId).First();
            return Json(result.ToList());
        }
        public IActionResult schedule_the_production3()
        {
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            // if (== 1))
           // ViewBag.test = _context.ProductionSchedules.Where(c => c.IntallationRequestId == 12).Count();


;
            return View();
        }
        public IActionResult schedule_the_production4()
        {
            // ViewBag.date = DateTime.Now.DayOfWeek;

            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            var query = (from installReq in _context.InstallationRequests
                         join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId

                         where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched.IsConfirmed == 0 && (productionSched.ProductionDate < DateTime.Now.Date)



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             neighborhood = address.neighborhood,
                             ProductionDate = productionSched.ProductionDate,
                             ProductionSheduleId = productionSched.Id,
                             IntallationRequestId = installReq.Id,
                             quotation_date = quot.QuotationDate.Value.ToShortDateString(),

                         });




            var res = query.ToList();
            var res2 = from queryResult in res

                       group queryResult by queryResult.IntallationRequestId into a
                       select a.OrderByDescending(g => g.ProductionSheduleId).First();


            return View(res2.ToList());
        }
        public IActionResult schedule_the_production3Api(int radioValue=0) 
        {
            //if (radioValue == 0)
            //{


            //    var query1 = (from installReq in _context.InstallationRequests
            //                      //join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
            //                  join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
            //                  join branch in _context.Branches on quot.BranchId equals branch.BranchId
            //                  join customer in _context.Customers on quot.CustomerId equals customer.Id

            //                  join address in _context.Addresses on quot.addressId equals address.id
            //                  join city in _context.Cities on address.cityid equals city.CityId

            //                  where !_context.ProductionSchedules.Any(c => c.IntallationRequestId == installReq.Id)



            //                  select new productionClass
            //                  {
            //                      branch = branch.BranchName,
            //                      city = city.CityName,
            //                      CustomerArabicName = customer.ArabicName,
            //                      quotionCode = quot.Code,
            //                      quotionNumberId = quot.QuotationId,
            //                      ProductionDate = DateTime.Now,
            //                      status="0",
            //                      IntallationRequestId=installReq.Id,
            //                      quotation_date=quot.QuotationSimpleDate.Value.ToShortDateString(),


            //                  }).ToList();
            //    var query2 = (from installReq in _context.InstallationRequests

            //                  join productionSched1 in _context.ProductionSchedules on installReq.Id equals productionSched1.IntallationRequestId

            //                  join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
            //                  join branch in _context.Branches on quot.BranchId equals branch.BranchId
            //                  join customer in _context.Customers on quot.CustomerId equals customer.Id

            //                  join address in _context.Addresses on quot.addressId equals address.id
            //                  join city in _context.Cities on address.cityid equals city.CityId
            //                   where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0)&& productionSched1.IsConfirmed == 0 && productionSched1.ProductionDate >= DateTime.Now
            //                  //where 





            //                  select new productionClass
            //                  {
            //                      branch = branch.BranchName,
            //                      city = city.CityName,
            //                      CustomerArabicName = customer.ArabicName,
            //                      quotionCode = quot.Code,
            //                      quotionNumberId = quot.QuotationId,
            //                      IntallationRequestId = productionSched1.IntallationRequestId,
            //                      ProductionDate = productionSched1.ProductionDate,
            //                      status = "1",
            //                      quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),




            //                  }).ToList();
            //    var datasource = query1.Union(query2);
            //    return Json(datasource.ToList());

            //}
            ////غير مجدول 

            //if (radioValue == 1)
            //{
            //    var query1 = (from installReq in _context.InstallationRequests

            //                 // join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
            //                  join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
            //                  join branch in _context.Branches on quot.BranchId equals branch.BranchId
            //                  join customer in _context.Customers on quot.CustomerId equals customer.Id

            //                  join address in _context.Addresses on quot.addressId equals address.id
            //                  join city in _context.Cities on address.cityid equals city.CityId

            //                   where !_context.ProductionSchedules.Any(c => c.IntallationRequestId == installReq.Id)




            //                  select new productionClass
            //                  {
            //                      branch = branch.BranchName,
            //                      city = city.CityName,
            //                      CustomerArabicName = customer.ArabicName,
            //                      quotionCode = quot.Code,
            //                      quotionNumberId = quot.QuotationId,
            //                      ProductionDate= DateTime.Now,
            //                      IntallationRequestId=installReq.Id,
            //                      quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),



            //                  });




            //    var res1 = query1.ToList();
            //    return Json(res1);

            //}
            ////مجدول غير مؤكد
            //if (radioValue == 2)
            //{
            //    var query2 = (from installReq in _context.InstallationRequests

            //                  join productionSched1 in _context.ProductionSchedules on installReq.Id equals productionSched1.IntallationRequestId

            //                  join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
            //                  join branch in _context.Branches on quot.BranchId equals branch.BranchId
            //                  join customer in _context.Customers on quot.CustomerId equals customer.Id

            //                  join address in _context.Addresses on quot.addressId equals address.id
            //                  join city in _context.Cities on address.cityid equals city.CityId
            //                  where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched1.IsConfirmed == 0&&productionSched1.ProductionDate>=DateTime.Now
            //                  //where productionSched1.IsConfirmed == 0





            //                  select new productionClass
            //                  {
            //                      branch = branch.BranchName,
            //                      city = city.CityName,
            //                      CustomerArabicName = customer.ArabicName,
            //                      quotionCode = quot.Code,
            //                      quotionNumberId = quot.QuotationId,
            //                      IntallationRequestId = productionSched1.IntallationRequestId,
            //                      ProductionDate = productionSched1.ProductionDate,
            //                      quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),





            //                  });




            //    //var res2 = from queryResult in query2

            //    //           group queryResult by queryResult.IntallationRequestId into a
            //    //           where a.Count()==1
            //    //           select a;

            //    return Json(query2.ToList());
            //}
            //الكل
            if (radioValue == 0)
            {


                var query1 = (from installReq in _context.InstallationRequests
                              join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                              where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched.IsConfirmed == 0 && productionSched.ProductionDate.Value.Date >= DateTime.Now.Date



                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  ProductionDate = productionSched.ProductionDate,
                                  status = "0",
                                  IntallationRequestId = installReq.Id,
                                  quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                  ProductionSheduleId=productionSched.Id


                              }).ToList();
                var res1 = from queryResult in query1

                           group queryResult by queryResult.IntallationRequestId into a
                           select a.OrderByDescending(g => g.ProductionSheduleId).First();

                var query2 = (from installReq in _context.InstallationRequests

                              join productionSched1 in _context.ProductionSchedules on installReq.Id equals productionSched1.IntallationRequestId

                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId
                              where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 2) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 0) == 1) && productionSched1.ProductionDate.Value.Date >= DateTime.Now.Date && productionSched1.IsConfirmed==1
                              //where 





                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = productionSched1.IntallationRequestId,
                                 // ProductionDate = _context.ProductionSchedules.Where(i=>i.IntallationRequestId== productionSched1.IntallationRequestId && i.ProductionDate >= DateTime.Now&&i.IsConfirmed==1).OrderByDescending(i=>i.Id).Select(p=>p.ProductionDate).FirstOrDefault(),
                                  ProductionDate = productionSched1.ProductionDate,
                                  status = "1",
                                  quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                  ProductionSheduleId = productionSched1.Id





                              }).ToList();
                var res2 = from queryResult in query2

                           group queryResult by queryResult.IntallationRequestId into a
                           select a.OrderByDescending(g => g.ProductionSheduleId).First();
                //var datasource = query1.Union(query2);
                var datasource = res1.Union(res2);
                return Json(datasource.ToList());

            }

            // مجدول غير مؤكد  

            if (radioValue == 1)
            {
                var query1 = (from installReq in _context.InstallationRequests

                         join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                            where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched.IsConfirmed == 0 && productionSched.ProductionDate.Value.Date >= DateTime.Now.Date




                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  ProductionDate = productionSched.ProductionDate,
                                  IntallationRequestId = installReq.Id,
                                  quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                  ProductionSheduleId = productionSched.Id




                              });




                //var res1 = query1.ToList();
                var res11 = from queryResult in query1

                           group queryResult by queryResult.IntallationRequestId into a
                           select a.OrderByDescending(g => g.ProductionSheduleId).First();
                var res1 = res11.ToList();
                return Json(res1);

            }
            //مجدول 
            if (radioValue == 2)
            {
                var query2 = (from installReq in _context.InstallationRequests

                              join productionSched1 in _context.ProductionSchedules on installReq.Id equals productionSched1.IntallationRequestId

                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId
                              where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 2) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 0) == 1) && productionSched1.ProductionDate.Value.Date >= DateTime.Now.Date && productionSched1.IsConfirmed==1
                              //where productionSched1.IsConfirmed == 0





                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = productionSched1.IntallationRequestId,
                                 // ProductionDate = _context.ProductionSchedules.Where(i => i.IntallationRequestId == productionSched1.IntallationRequestId && i.ProductionDate >= DateTime.Now&& i.IsConfirmed == 1).OrderByDescending(i => i.Id).Select(p => p.ProductionDate).FirstOrDefault(),
                                 ProductionDate = productionSched1.ProductionDate,
                                  quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                  ProductionSheduleId = productionSched1.Id






                              });




                var res1 = from queryResult in query2

                           group queryResult by queryResult.IntallationRequestId into a
                           select a.OrderByDescending(g => g.ProductionSheduleId).First();

                return Json(res1.ToList());
            }
            else
            {
                return Json("empty");
            }





        }

        public IActionResult daily_production_log()
        {
           // ViewBag.date = DateTime.Now.DayOfWeek;
           
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            var query = (from installReq in _context.InstallationRequests
                         join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId

                         where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched.IsConfirmed == 0 && (productionSched.ProductionDate == DateTime.Now.Date|| productionSched.ProductionDate == DateTime.Now.Date.AddDays(1)|| productionSched.ProductionDate == DateTime.Now.Date.AddDays(2))



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             neighborhood=address.neighborhood,
                             ProductionDate=productionSched.ProductionDate,
                             ProductionSheduleId=productionSched.Id,
                             IntallationRequestId=installReq.Id,
                             quotation_date=quot.QuotationDate.Value.ToShortDateString(),
                             
                         });




            var res = query.ToList();
            var res2 = from queryResult in res

                       group queryResult by queryResult.IntallationRequestId into a
                       select a.OrderByDescending(g => g.ProductionSheduleId).First();


            return View(res2.ToList());
        }


        public async Task<IActionResult> arraysFromExcelapi(IFormFile file1)
        {

            // Check the File is received
            //sheet1:مودع
            //sheet2:مودع غير

            if (file1 == null)
                throw new Exception("File is Not Received...");


            // Create the Directory if it is not exist
            string dirPath = Path.Combine(hostEnvironment.WebRootPath, "ReceivedReports");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // MAke sure that only Excel file is used 
            string dataFileName = Path.GetFileName(file1.FileName);

            string extension = Path.GetExtension(dataFileName);

            string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

            if (!allowedExtsnions.Contains(extension))
                throw new Exception("Sorry! This file is not allowed,  make sure that file having extension as either.xls or.xlsx is uploaded.");

            // Make a Copy of the Posted File from the Received HTTP Request
            string saveToPath = Path.Combine(dirPath, dataFileName);

            using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
            {
                file1.CopyTo(stream);
            }

            // USe this to handle Encodeing differences in .NET Core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // read the excel file
            using (var stream = new FileStream(saveToPath, FileMode.Open))
            {
                if (extension == ".xls")
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                //

                DataSet ds = new DataSet();
                ds = reader.AsDataSet();
                reader.Close();


                if (ds != null && ds.Tables.Count > 0)
                {


                    // Read the the Table sheet1 مودع
                    DataTable serviceDetails1 = ds.Tables[1];
                    for (int i = 5; i < 16; i++)
                    {
                        Item item = new Item();
                        item.ItemArName = serviceDetails1.Rows[i][1].ToString();
                        item.ItemEnName = serviceDetails1.Rows[i][1].ToString();
                        item.ItemCode = serviceDetails1.Rows[i][1].ToString();
                        item.Notes = serviceDetails1.Rows[i][2].ToString();
                        item.RMCost = (double?)serviceDetails1.Rows[i][3];
                        item.FCost = (double?)serviceDetails1.Rows[i][4];
                        item.ICost = (double?)serviceDetails1.Rows[i][5];
                        item.COG = (double?)serviceDetails1.Rows[i][6];
                        item.UOM = serviceDetails1.Rows[i][7].ToString();
                        item.ItemCurrentPrice = (double?)serviceDetails1.Rows[i][8];
                        item.ItemSubTypeId = 65;
                        
                       // _context.Add(item);


                       // _context.SaveChanges();
                        //if (item.ItemCode == "SHB-08-150" || item.ItemCode == "SHB-08-205" || item.ItemCode == "SHB-08-277" || item.ItemCode == "SHB-09-150" || item.ItemCode == "SHB-09-205" || item.ItemCode == "SHB-09-277" || item.ItemCode == "SHB-10-01" || item.ItemCode == "SHB-12-01")
                        //{
                        //    RowMaterialItem rowMaterialItem = new RowMaterialItem();
                        //    rowMaterialItem.ItemId = item.ItemId;
                        //    rowMaterialItem.RowMatererialId = 1;
                        //    _context.Add(rowMaterialItem);
                        //    RowMaterialItem rowMaterialItem2 = new RowMaterialItem();
                        //    rowMaterialItem2.ItemId = item.ItemId; 
                        //    rowMaterialItem2.RowMatererialId = 3;
                        //    _context.Add(rowMaterialItem2);
                        //    _context.SaveChanges();
                        //}
                        //else {
                        ////RowMaterialItem rowMaterialItem = new RowMaterialItem();
                        ////rowMaterialItem.ItemId = item.ItemId;
                        ////rowMaterialItem.RowMatererialId = 1;
                        ////_context.Add(rowMaterialItem);
                        ////RowMaterialItem rowMaterialItem2 = new RowMaterialItem();
                        ////    rowMaterialItem2.ItemId = item.ItemId;
                        ////    rowMaterialItem2.RowMatererialId = 3;
                        ////    _context.Add(rowMaterialItem2);
                        ////    _context.SaveChanges();

                        // }
                        //  if (serviceDetails1.Rows[i][9].ToString() == "لون الخشب الأساسي ولون الخشب الفرعي"||serviceDetails1.Rows[i][9].ToString() == "لون الخشب")
                        // {
                        //RowMaterialItem rowMaterialItem = new RowMaterialItem();
                        //rowMaterialItem.ItemId = item.ItemId;
                        //rowMaterialItem.RowMatererialId = 1;
                        //_context.Add(rowMaterialItem);
                        //RowMaterialItem rowMaterialItem2 = new RowMaterialItem();
                        //rowMaterialItem2.ItemId = item.ItemId;
                        //rowMaterialItem2.RowMatererialId = 3;
                        //_context.Add(rowMaterialItem2);
                        //_context.SaveChanges();
                        //  }
                        ////else if (serviceDetails1.Rows[i][9].ToString() == "لون الخشب الأساسي ولون الخشب الفرعي  ولون الزجاج ")
                        //// {
                        //     RowMaterialItem rowMaterialItem = new RowMaterialItem();
                        //     rowMaterialItem.ItemId = item.ItemId;
                        //     rowMaterialItem.RowMatererialId = 1;
                        //     _context.Add(rowMaterialItem);
                        //     RowMaterialItem rowMaterialItem2 = new RowMaterialItem();
                        //     rowMaterialItem2.ItemId = item.ItemId;
                        //     rowMaterialItem2.RowMatererialId = 3;
                        //     _context.Add(rowMaterialItem2);
                        //     _context.SaveChanges();
                        // }


                    }
                }
            }
            return Json("done");

            // return RedirectToAction("arraysFromExcel", "Clients",new{ sheet1new=JsonConvert.SerializeObject(sheet1new.ToList()), sheet1update, sheet1beforupdate, sheet2new.Count, sheet2update, sheet2beforeupdate});



        }



    }
}
