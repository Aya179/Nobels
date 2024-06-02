using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nobels.Models;
using System.Data;
using System.Globalization;
using System.Security.Claims;

namespace Nobels.Controllers
{
    [Authorize] 

    public class trkebat : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;


        public trkebat(db_a8d242_exadecor2Context context, UserManager<Employee> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        public IActionResult add_schedule_trekb_request(int instReqId)
        {
            var instalReq = _context.InstallationRequests.Find(instReqId);
            var quot = _context.Quotations.Where(c => c.QuotationId == instalReq.QutationId).First();
            var address = _context.Addresses.Find(quot.addressId);
            var Customer = _context.Customers.Find(quot.CustomerId);
            var Branch = _context.Branches.Find(quot.BranchId);
            var city = _context.Cities.Find(address.cityid);
            //oldComparein$('body').on('click'):
            ViewBag.productionSchedDate = _context.ProductionSchedules.Where(c => c.IntallationRequestId == instalReq.Id && c.IsConfirmed == 1).OrderByDescending(c => c.Id).Select(p => p.ProductionDate.Value.ToShortDateString()).First();
            var productionSched = DateTime.Now;
            DateTime date = (DateTime)_context.WarehouseUpdates.Where(w => w.RequestId == instReqId).Select(w => w.PalletsDate.Value.Date).FirstOrDefault();
            string formattedDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ViewBag.palletsdate = formattedDate;
            ViewBag.ShipmentDate = _context.WarehouseUpdates.Where(w => w.RequestId == instReqId && w.ShipmentDate != null).Select(w => w.ShipmentDate.Value.ToShortDateString()).FirstOrDefault();
            ViewBag.TeamReceiveDate = _context.WarehouseUpdates.Where(w => w.RequestId == instReqId && w.TeamReceiveDate != null).Select(w => w.TeamReceiveDate.Value.ToShortDateString()).FirstOrDefault();
            //**********
            //•	 معالجة صلاحية التاريخ المقترح للتركيب
            //var allteamsInCity = _context.Teams.Where(t => t.CityId == city.CityId);
            //var instSChed = _context.InstallationSchedules.Where(c => c.InstallationDate != instalReq.DesiredDate || c.InstallationDate.Value.AddDays((double)c.Duration) < instalReq.DesiredDate&&allteamsInCity.Where(t=>t.Id==c.TeamId).Any()).ToList();
            //var selectquery = (from instSch in _context.InstallationSchedules
            //                   join instreq in _context.InstallationRequests on instSch.InstallationRequestId equals instreq.Id
            //                   join team in _context.Teams on city.CityId equals team.CityId
            //                   where instSch.InstallationDate == instreq.DesiredDate || instSch.InstallationDate.Value.AddDays((double)instSch.Duration) >= instreq.DesiredDate && team.Id == instSch.TeamId
            //                   select new
            //                   {
            //                       team=team.Name
            //                   }

            //                   ).ToList();
            //if (instalReq.DesiredDate <= productionSched.ProductionDate)
            //{
            //    ViewBag.status = "غير صالح";
            //}
            //else if ((instalReq.DesiredDate > productionSched.ProductionDate) && selectquery.Count == 0)
            //{
            //    ViewBag.status = "متاح";
            //} 
            //else if ((instalReq.DesiredDate > productionSched.ProductionDate) && selectquery.Count != 0)
            //{
            //    ViewBag.status = "الرجاء اختيار تاريخ بديل";
            //}
            //************
            //ViewBag.numberofweek = (productionSched.ProductionDate.Value.Day - 1) / 7 + 1;
            ViewBag.numberofweek = (productionSched.Date.Day - 1) / 7 + 1;
            ViewBag.DesiredDate = instalReq.DesiredDate.Value.ToShortDateString();
            ViewBag.QuotionDate = quot.QuotationDate.Value.ToShortDateString();
            ViewBag.customer = Customer.ArabicName;
            ViewBag.Branch = Branch.BranchName;
            ViewBag.quoID = quot.QuotationId;
            ViewBag.code = quot.Code;
            ViewBag.city = city.CityName;
            ViewBag.neigh = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.house = address.houseNumber;

            // string monthnubmer = productionSched.ProductionDate.Value.Month.ToString("00");
            string monthnubmer = productionSched.Date.Month.ToString("00");
            ViewBag.monthnubmer = monthnubmer;
            ViewBag.currentdate = DateTime.UtcNow.Date.ToShortDateString();








            return View();
        }

        public IActionResult CheckingOverlap(DateTime selecteddate, int duration, int teamId)
        {


            DateTime startDate = selecteddate; // September 8, 2023

            DateTime endDate = selecteddate.AddDays(duration - 1);

            bool isFriday = false;
            //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            //{
            //    if (date.DayOfWeek == DayOfWeek.Friday)
            //    {
            //        isFriday = true;
            //        //Console.WriteLine("Friday found. Exiting loop.");
            //        break;
            //    }


            //}
            var nextschedForFriday = _context.InstallationSchedules.Where(n => n.TeamId == teamId && endDate.DayOfWeek == DayOfWeek.Friday && ((n.InstallationDate <= selecteddate.AddDays(duration)) && (n.InstallationDate.Value.AddDays((double)n.Duration) >= selecteddate))).ToList();
            var nextSched = _context.InstallationSchedules.Where(n => n.TeamId == teamId && ((n.InstallationDate <= selecteddate.AddDays(duration - 1)) && (n.InstallationDate.Value.AddDays((double)n.Duration - 1) >= selecteddate))).ToList();

            if (startDate.DayOfWeek == DayOfWeek.Friday || /*endDate.DayOfWeek == DayOfWeek.Friday*/ nextschedForFriday.Count > 0)
            {
                isFriday = true;


            }
            if (isFriday)
            {
                return Json("isfriday");
            }
            else if (nextSched.Count == 0 && !isFriday)
            {
                return Json("avalible");
            }

            else
            {
                return Json("overlap");
            }

        }
        public IActionResult CheckingOverlapForEditPage(DateTime selecteddate, int duration, int teamId, int reqId)
        {


            DateTime startDate = selecteddate;

            DateTime endDate = selecteddate.AddDays(duration - 1);

            bool isFriday = false;
            var nextSched = _context.InstallationSchedules.Where(n => n.TeamId == teamId && ((n.InstallationDate <= selecteddate.AddDays(duration - 1)) && (n.InstallationDate.Value.AddDays((double)n.Duration - 1) >= selecteddate))).ToList();
            // var lastInstalltion = nextSched.Where(task => task.InstallationRequestId == reqId&&task.TeamId==teamId).OrderByDescending(task => task.Id).FirstOrDefault();
            var lastInstalltion = nextSched.Where(task => task.InstallationRequestId == reqId).ToList();

            var nextschedForFriday = _context.InstallationSchedules.Where(n => n.TeamId == teamId && endDate.DayOfWeek == DayOfWeek.Friday && ((n.InstallationDate <= selecteddate.AddDays(duration)) && (n.InstallationDate.Value.AddDays((double)n.Duration) >= selecteddate))).ToList();
            if (lastInstalltion != null && nextschedForFriday.Count /*==*/>= 1 && nextSched.Count/*==*/>= 1)
            {
                //nextSched.Remove(lastInstalltion);
                nextSched.RemoveRange(0, lastInstalltion.Count());
                // nextschedForFriday.Remove(lastInstalltion);
                nextschedForFriday.RemoveRange(0, lastInstalltion.Count());

            }
            else if (lastInstalltion != null)
            {
                // nextSched.Remove(lastInstalltion);
                nextSched.RemoveRange(0, lastInstalltion.Count());
            }

            if (startDate.DayOfWeek == DayOfWeek.Friday || nextschedForFriday.Count > 0)
            {
                isFriday = true;


            }



            if (isFriday)
            {
                return Json("isfriday");
            }
            else if (nextSched.Count == 0 && !isFriday)
            {
                return Json("avalible");
            }

            else
            {
                return Json("overlap");
            }

        }
        public IActionResult add_schedule_trekb_requestApi(int reqId)
        {
            var requists = _context.InstallationRequests.Find(reqId);
            var quu = _context.Quotations.Find(requists.QutationId);
            var address1 = _context.Addresses.Find(quu.addressId);
            var teams = _context.Teams.Where(t => t.CityId == address1.cityid);
            var query = (from installSched in _context.InstallationSchedules
                         join team in _context.Teams on installSched.TeamId equals team.Id
                         join installReq in _context.InstallationRequests on installSched.InstallationRequestId equals installReq.Id
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId
                         where team.CityId == city.CityId && city.CityId == address1.cityid /*installReq.Id==reqId*/



                         select new
                         {

                             // city = city.CityName,
                             // quotionNumberId = quot.QuotationId,
                             teamName = team.Name,
                             //IntallationRequestId = installReq.Id,
                             InstallationDate = installSched.InstallationDate,
                             Duration = installSched.Duration,
                             teamid = team.Id,

                         });


            var teamstasks = query.ToList();



            return Json(new { teamstasks, teams });



        }
        public IActionResult add_schedule_trekb_requestApiFrom(DateTime selecteddate, int duration, int reqId, int teamid)
        {
            InstallationSchedule installationSchedule = new InstallationSchedule();
            installationSchedule.IsConfirmed = 1;
            installationSchedule.InstallationDate = selecteddate;
            installationSchedule.Duration = duration;
            installationSchedule.InstallationRequestId = reqId;
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            installationSchedule.EmployeeId = Int32.Parse(userId);
            installationSchedule.TeamId = teamid;
            installationSchedule.AddedDate = DateTime.Now;
            _context.InstallationSchedules.Add(installationSchedule);
            var instreq = _context.InstallationRequests.Find(reqId);
            instreq.Progress = 1;
            _context.Update(instreq);
            _context.SaveChanges();
            return Json("");

        }
        public IActionResult confirm_trkebat_request_schedule()
        {
            return View();
        }
        public IActionResult edit_schedule_trekb_request(int instReqId)
        {
            var instalReq = _context.InstallationRequests.Find(instReqId);
            var quot = _context.Quotations.Where(c => c.QuotationId == instalReq.QutationId).First();
            var address = _context.Addresses.Find(quot.addressId);
            var Customer = _context.Customers.Find(quot.CustomerId);
            var Branch = _context.Branches.Find(quot.BranchId);
            var city = _context.Cities.Find(address.cityid);
            ViewBag.productionSchedDate = _context.ProductionSchedules.Where(c => c.IntallationRequestId == instalReq.Id && c.IsConfirmed == 1).OrderByDescending(c => c.Id).Select(p => p.ProductionDate.Value.ToShortDateString()).First();
            var productionSched = DateTime.Now;
            DateTime date = (DateTime)_context.WarehouseUpdates.Where(w => w.RequestId == instReqId).Select(w => w.PalletsDate.Value.Date).FirstOrDefault();

            string formattedDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ViewBag.palletsdate = formattedDate;
            // ViewBag.palletsdate = _context.WarehouseUpdates.Where(w => w.RequestId == instReqId).Select(w => w.PalletsDate.Value.ToShortDateString()).FirstOrDefault();
            ViewBag.ShipmentDate = _context.WarehouseUpdates.Where(w => w.RequestId == instReqId && w.ShipmentDate != null).Select(w => w.ShipmentDate.Value.ToShortDateString()).FirstOrDefault();
            ViewBag.TeamReceiveDate = _context.WarehouseUpdates.Where(w => w.RequestId == instReqId && w.TeamReceiveDate != null).Select(w => w.TeamReceiveDate.Value.ToShortDateString()).FirstOrDefault();

            ViewBag.numberofweek = (productionSched.Date.Day - 1) / 7 + 1;
            ViewBag.DesiredDate = instalReq.DesiredDate.Value.ToShortDateString();
            ViewBag.QuotionDate = quot.QuotationDate.Value.ToShortDateString();
            ViewBag.customer = Customer.ArabicName;
            ViewBag.Branch = Branch.BranchName;
            ViewBag.quoID = quot.QuotationId;
            ViewBag.code = quot.Code;
            ViewBag.city = city.CityName;
            ViewBag.neigh = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.house = address.houseNumber;


            string monthnubmer = productionSched.Date.Month.ToString("00");
            ViewBag.monthnubmer = monthnubmer;
            ViewBag.currentdate = DateTime.UtcNow.Date.ToShortDateString();

            var LastInstallationSchedules = _context.InstallationSchedules.Where(i => i.InstallationRequestId == instReqId/*&&i.InstallationDate > DateTime.Now*/).OrderByDescending(i => i.Id).FirstOrDefault();
            ViewBag.LastInstallationSchedulesDate = LastInstallationSchedules.InstallationDate.Value.ToShortDateString();
            ViewBag.team = _context.Teams.Where(i => i.Id == LastInstallationSchedules.TeamId).Select(t => t.Name).FirstOrDefault();






            return View();
        }
        public IActionResult edit_schedule_trekb_requestApi(int reqId)
        {
            var requists = _context.InstallationRequests.Find(reqId);
            var quu = _context.Quotations.Find(requists.QutationId);
            var address1 = _context.Addresses.Find(quu.addressId);
            var teams = _context.Teams.Where(t => t.CityId == address1.cityid);
            var query = (from installSched in _context.InstallationSchedules
                         join team in _context.Teams on installSched.TeamId equals team.Id
                         join installReq in _context.InstallationRequests on installSched.InstallationRequestId equals installReq.Id
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId
                         where team.CityId == city.CityId && city.CityId == address1.cityid /*installReq.Id==reqId*/



                         select new
                         {

                             // city = city.CityName,
                             // quotionNumberId = quot.QuotationId,

                             teamName = team.Name,
                             IntallationRequestId = installReq.Id,
                             InstallationDate = installSched.InstallationDate,
                             Duration = installSched.Duration,
                             teamid = team.Id,
                             installtionId = installSched.Id,
                             addedDate = installSched.AddedDate,

                         });


            var teamstasks = query.ToList();

            //var lastInstalltion =  from queryResult in teamstasks
            //                       where queryResult.IntallationRequestId== reqId
            //                       group queryResult by new { queryResult.IntallationRequestId, queryResult.InstallationDate, queryResult.teamid, queryResult.Duration } into a

            //                                 select a.OrderByDescending(g => g.installtionId).FirstOrDefault();

            var lastInstalltions = teamstasks.Where(task => task.IntallationRequestId == reqId)/*.OrderByDescending(task => task.installtionId).FirstOrDefault()*/.ToList();

            // var allDublicatedRow = teamstasks.Where(t => t.IntallationRequestId == reqId && t.InstallationDate == lastInstalltion.InstallationDate && t.Duration == lastInstalltion.Duration && t.teamid == lastInstalltion.teamid).ToList();

            if (lastInstalltions != null)
            {
                foreach (var i in /*allDublicatedRow*/lastInstalltions)
                    // teamstasks.Remove(lastInstalltion);
                    teamstasks.Remove(i);
            }

            return Json(new { teamstasks, teams, lastInstalltions });



        }
        public IActionResult schedule_trkeb_requestall()
        {
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");

            return View();
        }

        public IActionResult schedule_trkeb_requestallApi1(int radioValue = 0)
        {
            //all
            if (radioValue == 0)
            {

                var query1 = (from installReq in _context.InstallationRequests
                              join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId

                              //  join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                              where (!_context.InstallationSchedules.Any(c => c.InstallationRequestId == installReq.Id)) && warehouse.PalletsDate != null



                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  neighborhood = address.neighborhood,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = installReq.Id,
                                  //ProductionDate = productionSched.ProductionDate,
                                  InstallationDate = null,
                                  status = "unscheduled",
                                  PalletsDate = warehouse.PalletsDate,
                                  Phone = customer.Phone,
                                  QuotationSimpleDate = quot.QuotationSimpleDate

                              }).ToList();




                //var res1 = from queryResult in query1

                //           group queryResult by queryResult.IntallationRequestId into a
                //           select a.OrderByDescending(g => g.ProductionDate).First();
                var query2 = (from installReq in _context.InstallationRequests
                              join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId

                              join instsch in _context.InstallationSchedules on installReq.Id equals instsch.InstallationRequestId

                              //  join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                              where /*instsch.IsConfirmed==0 &&*/ warehouse.PalletsDate != null && (_context.InstallationSchedules.Where(c => c.InstallationRequestId == installReq.Id).Count() >= 1)



                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  neighborhood = address.neighborhood,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = installReq.Id,
                                  //ProductionDate = productionSched.ProductionDate,
                                  InstallationDate = instsch.InstallationDate,
                                  status = "temporarilyscheduled",
                                  PalletsDate = warehouse.PalletsDate,
                                  Phone = customer.Phone,
                                  QuotationSimpleDate = quot.QuotationSimpleDate,
                                  IntallationScheduleId = instsch.Id


                              }).ToList();




                var res2 = from queryResult in query2
                               // where queryResult.InstallationDate > DateTime.Now
                           group queryResult by queryResult.IntallationRequestId into a

                           // select a.OrderByDescending(g => g.InstallationDate).First();
                           select a.OrderByDescending(g => g.IntallationScheduleId).First();
                var datasource = query1.Union(res2);
                return Json(datasource.ToList());

            }
            //unscheduled
            if (radioValue == 1)
            {
                var query1 = (from installReq in _context.InstallationRequests
                              join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId

                              // join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                              where (!_context.InstallationSchedules.Any(c => c.InstallationRequestId == installReq.Id)) && warehouse.PalletsDate != null



                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  neighborhood = address.neighborhood,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = installReq.Id,
                                  //ProductionDate = productionSched.ProductionDate,
                                  InstallationDate = null,
                                  status = "unscheduled",
                                  PalletsDate = warehouse.PalletsDate,
                                  Phone = customer.Phone,
                                  QuotationSimpleDate = quot.QuotationSimpleDate


                              }).ToList();




                //var res1 = from queryResult in query1

                //           group queryResult by queryResult.IntallationRequestId into a
                //           select a.OrderByDescending(g => g.ProductionDate).First();


                //query1.ToList();
                return Json(query1);

            }
            //temporarily scheduled
            if (radioValue == 2)
            {
                var query2 = (from installReq in _context.InstallationRequests
                              join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId

                              join instsch in _context.InstallationSchedules on installReq.Id equals instsch.InstallationRequestId

                              // join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                              join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                              join branch in _context.Branches on quot.BranchId equals branch.BranchId
                              join customer in _context.Customers on quot.CustomerId equals customer.Id

                              join address in _context.Addresses on quot.addressId equals address.id
                              join city in _context.Cities on address.cityid equals city.CityId

                              where /*instsch.IsConfirmed==0 &&*/ warehouse.PalletsDate != null && (_context.InstallationSchedules.Where(c => c.InstallationRequestId == installReq.Id).Count() >= 1)



                              select new productionClass
                              {
                                  branch = branch.BranchName,
                                  city = city.CityName,
                                  neighborhood = address.neighborhood,
                                  CustomerArabicName = customer.ArabicName,
                                  quotionCode = quot.Code,
                                  quotionNumberId = quot.QuotationId,
                                  IntallationRequestId = installReq.Id,
                                  //ProductionDate = productionSched.ProductionDate,
                                  InstallationDate = instsch.InstallationDate,
                                  status = "temporarilyscheduled",
                                  PalletsDate = warehouse.PalletsDate,
                                  Phone = customer.Phone,
                                  QuotationSimpleDate = quot.QuotationSimpleDate,
                                  IntallationScheduleId = instsch.Id



                              }).ToList();




                var res2 = from queryResult in query2
                               // where queryResult.InstallationDate>DateTime.Now
                           group queryResult by queryResult.IntallationRequestId into a

                           // select a.OrderByDescending(g => g.InstallationDate).First();
                           select a.OrderByDescending(g => g.IntallationScheduleId).First();

                return Json(res2);
            }
            else
            {
                return Json("empty");
            }





        }


        public IActionResult scheduled_succesfully(int reqId)
        {
            var req = _context.InstallationRequests.Find(reqId);
            var qu = _context.Quotations.Find(req.QutationId);
            var insSch = _context.InstallationSchedules.Where(i => i.InstallationRequestId == reqId).OrderByDescending(i => i.Id).FirstOrDefault();
            int duration = (int)insSch.Duration;
            //DateTime endDate = insSch.InstallationDate.Value.AddDays(duration - 1);
            DateTime startdate = insSch.InstallationDate.Value;
            DateTime enddate2 = insSch.InstallationDate.Value.AddDays(duration - 1);
            // if(insSch.Duration==1)
            //{
            //    enddate2 = insSch.InstallationDate.Value.AddDays(duration-1);
            //}
            //else
            //{
            for (DateTime date = startdate; date <= enddate2; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    // enddate2.Date.AddDays((double)1);
                    ViewBag.addOneToDuration = 1;
                    ViewBag.newendDate = insSch.InstallationDate.Value.AddDays(duration).ToShortDateString();
                    break;
                }


            }
            //}





            ViewBag.endDate = enddate2.Date.ToShortDateString();
            //var branch = _context.Branches.Find(qu.BranchId);
            //var add = _context.Addresses.Find(qu.addressId);
            //var city = _context.Cities.Find(add.cityid);
            var customer = _context.Customers.Find(qu.CustomerId);
            ViewBag.customer = customer.ArabicName;
            ViewBag.qId = qu.QuotationId;
            ViewBag.code = qu.Code;
            ViewBag.insDate = insSch.InstallationDate.Value.ToShortDateString();

            ViewBag.duration = insSch.Duration;

            return View();
        }
        [HttpPost]
        public IActionResult scheduled_succesfullyform(int qid, string notes)
        {

            if (notes == null || notes == String.Empty)
            {
                return RedirectToAction("schedule_trkeb_requestall");
            }

            else
            {
                QoutationUpdate qoutationUpdate = new QoutationUpdate();

                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


                qoutationUpdate.EmployeeId = Int32.Parse(userId);
                qoutationUpdate.ChangeDate = DateTime.Now;
                qoutationUpdate.QoutationId = qid;
                qoutationUpdate.Updates = notes + " " + qid;
                _context.Add(qoutationUpdate);
                _context.SaveChanges();
                return RedirectToAction("schedule_trkeb_requestall");


            }


        }

        public IActionResult trkebat_calender3inputs()
        {





            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");

            return View();
        }
        public IActionResult trkebat_calender3inputsApi(DateTime selecteddate, int cityId)
        {
            if (cityId == 0)
            {
                if (selecteddate.DayOfWeek == DayOfWeek.Saturday)
                {
                    var query = (from team in _context.Teams
                                 join installSched in _context.InstallationSchedules on team.Id equals installSched.TeamId
                                 join installReq in _context.InstallationRequests on installSched.InstallationRequestId equals installReq.Id


                                 //   join team in _context.Teams on installSched.TeamId equals team.Id


                                 join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                                 join branch in _context.Branches on quot.BranchId equals branch.BranchId
                                 join customer in _context.Customers on quot.CustomerId equals customer.Id

                                 join address in _context.Addresses on quot.addressId equals address.id
                                 join city in _context.Cities on team.CityId equals city.CityId

                                 where ((installSched.InstallationDate.Value.Date <= selecteddate.Date && installSched.InstallationDate.Value.AddDays((double)installSched.Duration).Date >= selecteddate.Date)/*|| installSched.InstallationDate.Value.Date.AddDays((double)installSched.Duration).DayOfWeek == DayOfWeek.Friday*/)


                                 select new productionClass
                                 {

                                     branch = branch.BranchName,
                                     city = city.CityName,
                                     CustomerArabicName = customer.ArabicName,
                                     quotionCode = quot.Code,
                                     quotionNumberId = quot.QuotationId,
                                     neighborhood = address.neighborhood,
                                     teamName = team.Name,
                                     Duration = installSched.Duration,
                                     IntallationRequestId = installReq.Id,
                                     InstallationDate = installSched.InstallationDate,
                                     quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                     IntallationScheduleId = installSched.Id,
                                     teamid = team.Id,
                                     status = installSched.InstallationDate == selecteddate ? "بدء" : installSched.InstallationDate.Value.AddDays((double)installSched.Duration) >= selecteddate /*|| installSched.InstallationDate.Value.AddDays((double)installSched.Duration).Date.DayOfWeek == DayOfWeek.Friday*/ /*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && installSched.Duration == 2)*/ ? "استكمال" : "متاح"

                                 }).ToList();
                    var res = from queryResult in query
                                  // where queryResult.InstallationDate > DateTime.Now
                              group queryResult by queryResult.IntallationRequestId into a

                              // select a.OrderByDescending(g => g.InstallationDate).First();
                              select a.OrderByDescending(g => g.IntallationScheduleId).First();
                    var finalres = res.ToList();




                    var query1 = (from team in _context.Teams




                                  join city in _context.Cities on team.CityId equals city.CityId

                                  // where (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id)) || (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id && (i.InstallationDate.Value.AddDays((double)i.Duration) >= selecteddate && i.InstallationDate <= selecteddate /*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && i.Duration == 2)*/)))

                                  select new productionClass
                                  {

                                      branch = string.Empty,
                                      city = city.CityName,
                                      CustomerArabicName = string.Empty,
                                      quotionCode = String.Empty,
                                      quotionNumberId = 0,
                                      neighborhood = string.Empty,
                                      teamName = team.Name,
                                      teamid = team.Id,
                                      IntallationRequestId = 0,
                                      InstallationDate = DateTime.Now,
                                      quotation_date = string.Empty,
                                      status = "nonSchedulesTeams"

                                  }).ToList();
                    var allTeams = query1.ToList();
                    var query1Teams = finalres;
                    var restTeams = allTeams.ExceptBy(query1Teams.Select(s => s.teamid), s => s.teamid).ToList();




                    var datasource = restTeams.Union(finalres);
                    return Json(datasource.OrderByDescending(i => i.quotionNumberId).ToList());
                    // return Json(new {restTeams, query1Teams });


                }
                else
                {
                    var query = (from team in _context.Teams
                                 join installSched in _context.InstallationSchedules on team.Id equals installSched.TeamId
                                 join installReq in _context.InstallationRequests on installSched.InstallationRequestId equals installReq.Id





                                 join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                                 join branch in _context.Branches on quot.BranchId equals branch.BranchId
                                 join customer in _context.Customers on quot.CustomerId equals customer.Id

                                 join address in _context.Addresses on quot.addressId equals address.id
                                 join city in _context.Cities on team.CityId equals city.CityId

                                 where ((installSched.InstallationDate.Value.Date <= selecteddate.Date && installSched.InstallationDate.Value.AddDays((double)installSched.Duration - 1).Date >= selecteddate.Date))


                                 select new productionClass
                                 {

                                     branch = branch.BranchName,
                                     city = city.CityName,
                                     CustomerArabicName = customer.ArabicName,
                                     quotionCode = quot.Code,
                                     quotionNumberId = quot.QuotationId,
                                     neighborhood = address.neighborhood,
                                     teamName = team.Name,
                                     Duration = installSched.Duration,
                                     IntallationRequestId = installReq.Id,
                                     InstallationDate = installSched.InstallationDate,
                                     IntallationScheduleId = installSched.Id,
                                     teamid = team.Id,

                                     quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                                     status = installSched.InstallationDate == selecteddate ? "بدء" : installSched.InstallationDate.Value.AddDays((double)installSched.Duration - 1) >= selecteddate /*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && installSched.Duration == 2)*/ ? "استكمال" : "متاح"

                                 }).ToList();
                    var res = from queryResult in query

                              group queryResult by queryResult.IntallationRequestId into a


                              select a.OrderByDescending(g => g.IntallationScheduleId).First();
                    var finalres = res.ToList();

                    var query1 = (from team in _context.Teams





                                  join city in _context.Cities on team.CityId equals city.CityId

                                  // where (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id)) || (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id && (i.InstallationDate.Value.AddDays((double)i.Duration-1) >= selecteddate && i.InstallationDate <= selecteddate /*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && i.Duration == 2)*/)))

                                  select new productionClass
                                  {

                                      branch = string.Empty,
                                      city = city.CityName,
                                      CustomerArabicName = string.Empty,
                                      quotionCode = String.Empty,
                                      quotionNumberId = 0,
                                      neighborhood = string.Empty,
                                      teamName = team.Name,
                                      teamid = team.Id,
                                      IntallationRequestId = 0,
                                      InstallationDate = DateTime.Now,
                                      quotation_date = string.Empty,
                                      status = "nonSchedulesTeams"

                                  }).ToList();

                    var allTeams = query1.ToList();
                    var query1Teams = finalres;
                    var restTeams = allTeams.ExceptBy(query1Teams.Select(s => s.teamid), s => s.teamid).ToList();


                    var datasource = restTeams.Union(finalres);
                    return Json(datasource.OrderByDescending(i => i.quotionNumberId).ToList());
                    // return Json(new { restTeams, query1Teams });


                }








            }


            //not called in view
            else
            {
                var query = (from installSched in _context.InstallationSchedules
                             join installReq in _context.InstallationRequests on installSched.InstallationRequestId equals installReq.Id

                             join team in _context.Teams on installSched.TeamId equals team.Id
                             join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                             join branch in _context.Branches on quot.BranchId equals branch.BranchId
                             join customer in _context.Customers on quot.CustomerId equals customer.Id

                             join address in _context.Addresses on quot.addressId equals address.id
                             join city in _context.Cities on team.CityId equals city.CityId

                             where team.CityId == cityId && (installSched.InstallationDate.Value.Date <= selecteddate.Date && installSched.InstallationDate.Value.AddDays((double)installSched.Duration - 1).Date >= selecteddate.Date)



                             select new productionClass
                             {

                                 branch = branch.BranchName,
                                 city = city.CityName,
                                 CustomerArabicName = customer.ArabicName,
                                 quotionCode = quot.Code,
                                 quotionNumberId = quot.QuotationId,
                                 neighborhood = address.neighborhood,
                                 teamName = team.Name,
                                 Duration = installSched.Duration,

                                 IntallationRequestId = installReq.Id,
                                 InstallationDate = installSched.InstallationDate,
                                 quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),

                                 status = installSched.InstallationDate == selecteddate ? "بدء" : installSched.InstallationDate.Value.AddDays((double)installSched.Duration - 1) >= selecteddate /*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && installSched.Duration == 2)*/  ? "استكمال" : "متاح"

                             }).ToList();





                //var res1 = from queryResult in query

                //           group queryResult by queryResult.IntallationRequestId into a
                //           select a.OrderByDescending(g => g.InstallationDate).First();


                var query1 = (from team in _context.Teams




                              join city in _context.Cities on team.CityId equals city.CityId

                              where (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id)) || (!_context.InstallationSchedules.Any(i => i.TeamId == team.Id && (i.InstallationDate.Value.AddDays((double)i.Duration) >= selecteddate && i.InstallationDate <= selecteddate/*|| (selecteddate.DayOfWeek == DayOfWeek.Thursday && i.Duration == 2)*/))) && team.CityId == cityId

                              select new productionClass
                              {

                                  branch = string.Empty,
                                  city = city.CityName,
                                  CustomerArabicName = string.Empty,
                                  quotionCode = String.Empty,
                                  quotionNumberId = 0,
                                  neighborhood = string.Empty,
                                  teamName = team.Name,
                                  IntallationRequestId = 0,
                                  InstallationDate = DateTime.Now,
                                  quotation_date = string.Empty,
                                  status = "nonSchedulesTeams"

                              }).ToList();
                var datasource = query1.Union(query);
                return Json(datasource.ToList());

                // return Json(res1.ToList());
            }

        }
        public IActionResult trkebat_offer(int id)
        {
            var qu = _context.Quotations.Find(id);
            ViewBag.quid = qu.QuotationId;
            ViewBag.code = qu.Code;
            ViewBag.qudate = qu.QuotationDate.Value.ToShortDateString();
            var customer = _context.Customers.Find(qu.CustomerId);
            ViewBag.customer = customer.ArabicName;
            var address = _context.Addresses.Find(qu.addressId);
            ViewBag.neig = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.housenumber = address.houseNumber;
            var city = _context.Cities.Find(address.cityid);
            ViewBag.city = city.CityName;
            var branch = _context.Branches.Find(qu.BranchId);
            ViewBag.branch = branch.BranchName;
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.userId = Int32.Parse(userId);
            var techOffice = _context.QoutationUpdates.Where(q => q.QoutationId == id && q.Updates.Contains("تم قبول العرض فنياً")).FirstOrDefault();
            if (techOffice != null)
            {
                ViewBag.tech = techOffice.Updates;
            }
            else { ViewBag.tech = "-"; }
            var FinanOffice = _context.QoutationUpdates.Where(q => q.QoutationId == id && q.Updates.Contains("تم قبول العرض مالياً")).FirstOrDefault();
            if (FinanOffice != null)
            {
                ViewBag.finan = FinanOffice.Updates;
            }
            else { ViewBag.finan = "-"; }
            return View();
        }
        public IActionResult trkebat_offerCreate([Bind("Id,QutationId,CreationDate,DesiredDate,EmployeeId")] InstallationRequest installationRequest, string notes)
        {
            installationRequest.Progress = 10;
            _context.Add(installationRequest);

            _context.SaveChanges();
            QoutationUpdate qoutationUpdate = new QoutationUpdate();
            qoutationUpdate.EmployeeId = installationRequest.EmployeeId;
            qoutationUpdate.ChangeDate = DateTime.Now;
            if (notes != null)
            {
                qoutationUpdate.Updates = notes + "-" + "تم رفع طلب تركيب للطلب" + " " + installationRequest.QutationId;
            }

            else
            {
                qoutationUpdate.Updates = "تم رفع طلب تركيب للطلب" + " " + installationRequest.QutationId;
            }
            qoutationUpdate.QoutationId = installationRequest.QutationId;
            _context.Add(qoutationUpdate);
            _context.SaveChanges();
            return RedirectToAction("schedule_the_production1", "Production");





        }
        public IActionResult track_trkebat()
        {
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");
            var query = (from qu in _context.Quotations
                         join installReq in _context.InstallationRequests on qu.QuotationId equals installReq.QutationId
                         //join quStatus in _context.QutationStatuses on qu.QuotationId equals quStatus.QutationId
                         join branch in _context.Branches on qu.BranchId equals branch.BranchId
                         join customer in _context.Customers on qu.CustomerId equals customer.Id

                         join address in _context.Addresses on qu.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId
                         where /*!_context.QutationStatuses.Any(c => c.QutationId == qu.QuotationId&&c.status== "closed")*//*&&*/(installReq.Progress <= 100 || installReq.Progress == 100) && (_context.InstallationSchedules.Any(c => c.InstallationRequestId == installReq.Id))

                         select new productionClass

                         {
                             quotionNumberId = qu.QuotationId,
                             quotionCode = qu.Code,
                             CustomerArabicName = customer.ArabicName,
                             branch = branch.BranchName,
                             city = city.CityName,
                             Progress = installReq.Progress,
                             quotation_date = qu.QuotationSimpleDate.Value.ToShortDateString(),
                             InstallationDate = _context.InstallationSchedules.Where(w => w.InstallationRequestId == installReq.Id).OrderByDescending(i => i.Id).Select(i => i.InstallationDate).FirstOrDefault()
                             //status = quStatus.status,
                             //Statusdate = quStatus.Statusdate
                         }).ToList();




            var res = from queryResult in query

                      group queryResult by queryResult.quotionNumberId into a
                      select a.OrderByDescending(g => g.Statusdate).First();


            return View(res.ToList());

        }

        public IActionResult track_trkebat_form(int id)
        {
            var qu = _context.Quotations.Find(id);
            ViewBag.code = qu.Code;
            var insreq = _context.InstallationRequests.Where(i => i.QutationId == id).First();
            ViewBag.progress = insreq.Progress;
            var insSch = _context.InstallationSchedules.Where(sc => sc.InstallationRequestId == insreq.Id).OrderByDescending(i => i.Id).FirstOrDefault();
            if (insSch != null)
            {
                ViewBag.date = insSch.InstallationDate.Value.ToShortDateString();
            }
            else
            {
                ViewBag.date = "-";
            }
            var select = (from quUpdate in _context.QoutationUpdates
                          join em in _context.Users on quUpdate.EmployeeId equals em.Id
                          where quUpdate.QoutationId == insreq.QutationId
                          select new productionClass
                          {
                              name = em.UserName,
                              note = quUpdate.Updates
                          }

                        ).ToList();

            ViewBag.quid = qu.QuotationId;
            ViewBag.reqid = insreq.Id;
            ViewBag.code = qu.Code;


            var customer = _context.Customers.Find(qu.CustomerId);
            ViewBag.customer = customer.ArabicName;
            var address = _context.Addresses.Find(qu.addressId);
            ViewBag.neig = address.neighborhood;
            var city = _context.Cities.Find(address.cityid);
            ViewBag.city = city.CityName;
            var branch = _context.Branches.Find(qu.BranchId);
            ViewBag.branch = branch.BranchName;
            ViewBag.Qdate = qu.QuotationDate.Value.ToShortDateString();
            ViewBag.street = address.street;
            ViewBag.house = address.houseNumber;

            var instUpdate = _context.InstallationUpdates.Where(i => i.RequestId == insreq.Id).ToList();
            ViewData["MyData"] = instUpdate;
            var instUpdatetrack = _context.InstallationUpdates.Where(up => up.RequestId == insreq.Id).OrderByDescending(up => up.Id).FirstOrDefault();
            if (instUpdatetrack != null && (instUpdatetrack.UpdateString.Contains("إرسال المواد الخام إلى الموقع")))
            { ViewBag.maxProgressValue = 99; }
            if (instUpdatetrack != null && (instUpdatetrack.UpdateString.Contains("تسليم الموقع")))
            { ViewBag.maxProgressValue = 100; }
            if (instUpdatetrack != null && (instUpdatetrack.UpdateString.Contains("إيقاف التركيب")))
            { ViewBag.maxProgressValue = 0; }
            return View(select);
        }
        public IActionResult test(int id)
        {
            var select = (from quUpdate in _context.QoutationUpdates
                          join em in _context.Users on quUpdate.EmployeeId equals em.Id
                          where quUpdate.QoutationId == id
                          select new
                          {
                              name = em.UserName,
                              note = quUpdate.Updates
                          }

                          ).ToList();
            return Json(select);
        }

        public IActionResult track_trkebat_formAction(int reqId, int progress, int status, string notes)
        {
            var inst = _context.InstallationRequests.Find(reqId);
            string statusValue = "";
            if (status == 1)
            {
                statusValue = "إرسال المواد الخام إلى الموقع";

            }
            if (status == 3)
            {
                statusValue = "الانتهاء من الـتركيب";

            }
            if (status == 4)
            {
                statusValue = "تسليم الموقع";
                progress = 100;

            }
            if (status == 2)
            {
                statusValue = "إيقاف التركيب";

            }
            if (status == 2)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                InstallationUpdate installationUpdate = new InstallationUpdate();
                installationUpdate.RequestId = reqId;
                installationUpdate.UpdatedDate = DateTime.Now;
                installationUpdate.UpdateString = statusValue;
                installationUpdate.EmployeeId = Int32.Parse(userId);
                _context.Add(installationUpdate);
                _context.SaveChanges();
                //ProgressUpdate progressUpdate = new ProgressUpdate();
                //progressUpdate.CurrentProgress = progress;
                //progressUpdate.UpdateDate = DateTime.Now;

                //progressUpdate.EmployeeId = Int32.Parse(userId);
                //progressUpdate.RequestId = reqId;

                //_context.Add(progressUpdate);
                //var instreq = _context.InstallationRequests.Find(reqId);
                //instreq.Progress = progress;
                //_context.Update(instreq);
                QoutationUpdate qoutationUpdate = new QoutationUpdate();
                qoutationUpdate.QoutationId = inst.QutationId;
                if (notes == null)
                {
                    qoutationUpdate.Updates = "إيقاف طلب التركيب" + " " + inst.QutationId;
                }
                else
                {
                    qoutationUpdate.Updates = notes;
                }


                qoutationUpdate.EmployeeId = Int32.Parse(userId);
                qoutationUpdate.ChangeDate = DateTime.Now;
                _context.Add(qoutationUpdate);
                _context.SaveChanges();
                return RedirectToAction("track_trkebat");
            }
            if (status != 2)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                InstallationUpdate installationUpdate = new InstallationUpdate();
                installationUpdate.RequestId = reqId;
                installationUpdate.UpdatedDate = DateTime.Now;
                installationUpdate.UpdateString = statusValue;
                installationUpdate.EmployeeId = Int32.Parse(userId);
                _context.Add(installationUpdate);
                _context.SaveChanges();
                ProgressUpdate progressUpdate = new ProgressUpdate();
                progressUpdate.CurrentProgress = progress;
                progressUpdate.UpdateDate = DateTime.Now;

                progressUpdate.EmployeeId = Int32.Parse(userId);
                progressUpdate.RequestId = reqId;

                _context.Add(progressUpdate);
                var instreq = _context.InstallationRequests.Find(reqId);
                instreq.Progress = progress;
                _context.Update(instreq);
                QoutationUpdate qoutationUpdate = new QoutationUpdate();
                qoutationUpdate.QoutationId = inst.QutationId;
                if (notes == null)
                {
                    qoutationUpdate.Updates = "تم" + " " + statusValue + " " + "للطلب " + inst.QutationId + " " + "بنسبة إنجاز" + progressUpdate.CurrentProgress + "%";
                }
                else
                {
                    qoutationUpdate.Updates = notes;
                }


                qoutationUpdate.EmployeeId = Int32.Parse(userId);
                qoutationUpdate.ChangeDate = DateTime.Now;
                _context.Add(qoutationUpdate);
                _context.SaveChanges();
                return RedirectToAction("track_trkebat");
            }

            return RedirectToAction("track_trkebat_form", new { id = inst.QutationId });


        }


        public IActionResult getCityBranches(int id)
        {
            var branches = _context.Branches.Where(b => b.CityId == id);
            return Json(new { branches });
        }


        public IActionResult NonInstallationRequestQuotations()
        {
            var query1 = (
                          from quot in _context.Quotations
                          join customer in _context.Customers on quot.CustomerId equals customer.Id

                          join address in _context.Addresses on quot.addressId equals address.id
                          join city in _context.Cities on address.cityid equals city.CityId

                          where (!_context.InstallationRequests.Any(c => c.QutationId == quot.QuotationId)) && quot.QuotationStatus.Contains("مقبول مالياً") && _context.OrderDetails.Where(o => o.QoutationId == quot.QuotationId).Any()



                          select new productionClass
                          {

                              city = city.CityName,
                              //neighborhood = address.neighborhood,
                              CustomerArabicName = customer.ArabicName,
                              quotionNumberId = quot.QuotationId,
                              status = quot.QuotationStatus,

                              QuotationSimpleDate = quot.QuotationDate,
                              FinancialDate = _context.QoutationUpdates.Where(i => i.QoutationId == quot.QuotationId && i.Updates.Contains("قبول العرض مالياً")).FirstOrDefault().ChangeDate.Value.ToShortDateString(),
                              TechnicalofficeDate = _context.QoutationUpdates.Where(i => i.QoutationId == quot.QuotationId && i.Updates.Contains("قبول العرض فنياً")).FirstOrDefault().ChangeDate.Value.ToShortDateString(),


                          }).ToList();
            return View(query1);
        }

        //استلام Pallets
        public IActionResult warehouse()
        {
            var warehouse1 = _context.WarehouseUpdates;
            var query = (from installReq in _context.InstallationRequests
                             //  join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                             // join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId

                         where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 0).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 1) /*&& (_context.ProductionSchedules.Any(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1 &&DateTime.Now>=c.ProductionDate))*/ /*&& (!_context.WarehouseUpdates.Where(i => i.RequestId == installReq.Id).Any())*/



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             neighborhood = address.neighborhood,
                             ProductionDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == installReq.Id && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault(),
                             // ProductionSheduleId = productionSched.Id,
                             IntallationRequestId = installReq.Id,

                             PalletsDate = warehouse1.Where(i => i.RequestId == installReq.Id).Any() ? warehouse1.Where(i => i.RequestId == installReq.Id).Select(i => i.PalletsDate).First() : null,
                             quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),


                         });




            var res = query.ToList();
            return View(res);
        }
        public IActionResult updatePalletsDate(int installReq)
        {
            WarehouseUpdate warehouse = new WarehouseUpdate();
            warehouse.RequestId = installReq;
            warehouse.WarehouseId = 1;
            warehouse.PalletsDate = DateTime.Now;
            _context.Add(warehouse);
            _context.SaveChanges();
            return Json(warehouse);


        }
        public IActionResult updatePalletsDateFromProductionDate(int installReq)
        {
            WarehouseUpdate warehouse = new WarehouseUpdate();
            warehouse.RequestId = installReq;
            warehouse.WarehouseId = 1;
            //warehouse.PalletsDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == installReq && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault();
            warehouse.PalletsDate = DateTime.Now.Date;
            _context.Add(warehouse);
            _context.SaveChanges();
            return Json(warehouse.PalletsDate);


        }
        public IActionResult updateShipmentDate(int WarehouseUpdateId)
        {
            var warehouse = _context.WarehouseUpdates.Find(WarehouseUpdateId);
            warehouse.ShipmentDate = DateTime.Now;
            _context.Update(warehouse);
            _context.SaveChanges();
            return Json(warehouse);


        }
        public IActionResult updateShipmentDateFromProductionDate(int WarehouseUpdateId)
        {
            var warehouse = _context.WarehouseUpdates.Find(WarehouseUpdateId);
            warehouse.ShipmentDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == warehouse.RequestId && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault();
            _context.Update(warehouse);
            _context.SaveChanges();
            return Json(warehouse);


        }
        public IActionResult updateTeamReceiveDate(int WarehouseUpdateId)
        {
            var warehouse = _context.WarehouseUpdates.Find(WarehouseUpdateId);
            warehouse.TeamReceiveDate = DateTime.Now;
            _context.Update(warehouse);
            _context.SaveChanges();
            return Json(warehouse);


        }
        //public IActionResult updateTeamReceiveDateFromProductionDate(int WarehouseUpdateId)
        //{
        //    var warehouse = _context.WarehouseUpdates.Find(WarehouseUpdateId);
        //    warehouse.TeamReceiveDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == warehouse.RequestId && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault(); 
        //    _context.Update(warehouse);
        //    _context.SaveChanges();
        //    return Json(warehouse);


        //}

        //شحن
        public IActionResult warehouse1()
        {
            var query = (from installReq in _context.InstallationRequests
                             //  join productionSched in _context.ProductionSchedules on installReq.Id equals productionSched.IntallationRequestId
                         join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId
                         where warehouse.PalletsDate != null/* && warehouse.ShipmentDate==null*/&& warehouse.TeamReceiveDate == null



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             neighborhood = address.neighborhood,
                             ProductionDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == installReq.Id && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault(),
                             // ProductionSheduleId = productionSched.Id,
                             IntallationRequestId = installReq.Id,
                             WarehouseUpdateId = warehouse.Id,
                             PalletsDate = warehouse.PalletsDate,
                             ShipmentDate = warehouse.ShipmentDate,
                             TeamReceiveDate = warehouse.TeamReceiveDate,
                             quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),

                         });




            var res = query.ToList();
            return View(res);
        }
        //استلام فريق التركيبات
        public IActionResult warehouse2()
        {


            var query = (from installReq in _context.InstallationRequests
                         join InstallationSch in _context.InstallationSchedules on installReq.Id equals InstallationSch.InstallationRequestId
                         join team in _context.Teams on InstallationSch.TeamId equals team.Id

                         join warehouse in _context.WarehouseUpdates on installReq.Id equals warehouse.RequestId
                         join quot in _context.Quotations on installReq.QutationId equals quot.QuotationId
                         join branch in _context.Branches on quot.BranchId equals branch.BranchId
                         join customer in _context.Customers on quot.CustomerId equals customer.Id

                         join address in _context.Addresses on quot.addressId equals address.id
                         join city in _context.Cities on address.cityid equals city.CityId
                         where warehouse.PalletsDate != null && warehouse.ShipmentDate != null && (_context.InstallationSchedules.Where(i => i.InstallationRequestId == installReq.Id).Count() >= 1)

                         // where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == installReq.Id).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == installReq.Id && c.IsConfirmed == 1) == 0) && productionSched.IsConfirmed == 0 && productionSched.ProductionDate == DateTime.Now.Date



                         select new productionClass
                         {
                             branch = branch.BranchName,
                             city = city.CityName,
                             CustomerArabicName = customer.ArabicName,
                             quotionCode = quot.Code,
                             quotionNumberId = quot.QuotationId,
                             neighborhood = address.neighborhood,
                             ProductionDate = _context.ProductionSchedules.Where(p => p.IntallationRequestId == installReq.Id && p.IsConfirmed == 1).Select(p => p.ProductionDate).FirstOrDefault(),
                             // ProductionSheduleId = productionSched.Id,
                             IntallationRequestId = installReq.Id,
                             WarehouseUpdateId = warehouse.Id,
                             PalletsDate = warehouse.PalletsDate,
                             ShipmentDate = warehouse.ShipmentDate,
                             TeamReceiveDate = warehouse.TeamReceiveDate,
                             quotation_date = quot.QuotationSimpleDate.Value.ToShortDateString(),
                             teamName = team.Name,
                             InstallationDate = InstallationSch.InstallationDate,
                             IntallationScheduleId = InstallationSch.Id

                         }).ToList();

            var res2 = from queryResult in query

                       group queryResult by queryResult.IntallationRequestId into a

                       select a.OrderByDescending(g => g.IntallationScheduleId).First();

            return View(res2);
        }



    }
}
