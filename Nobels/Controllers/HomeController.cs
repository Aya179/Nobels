using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nobels.Models;
using System.Diagnostics;

namespace Nobels.Controllers
{


    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;
        private readonly IWebHostEnvironment _appEnvironment;
        public HomeController(ILogger<HomeController> logger, db_a8d242_exadecor2Context context, IWebHostEnvironment env, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _logger = logger;
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;

            _appEnvironment = env;

        }

        public async Task<IActionResult> HomeAsync()
        {





            Employee user = await userManager.GetUserAsync(HttpContext.User);

            var roles = await userManager.GetRolesAsync(user);

            //if (user != null && roles != null && (roles.Contains("استشاري مبيعات") || roles.Contains("مدير معرض") || roles.Contains("مدير مشاريع") || roles.Contains("مندوب مشاريع")))
            if (user != null && roles != null && (roles.Contains("مدير مشاريع") || roles.Contains("مدير معرض")))
            {

                // return RedirectToAction("home", "showRoomAndProjectManger");

                return RedirectToAction("home", "RepresntitiveAndAdvisor");

            }
            else if (user != null && roles != null && (roles.Contains("استشاري مبيعات") || roles.Contains("مندوب مشاريع")))
            {
                return RedirectToAction("home", "RepresntitiveAndAdvisor");
            }
            else if (user != null && roles != null && (roles.Contains("مدير منتج")))
            {
                return RedirectToAction("IndexQutation", "ProductManager");
            }
            else if (user != null && roles != null && (roles.Contains("المكتب الفني ")))
            {
                return RedirectToAction("Index", "Technicaloffice");
            }
            else if (user != null && roles != null && (roles.Contains("المكتب المالي")))
            {
                return RedirectToAction("Index", "FinancialOffice");
            }
            else
            {//مدير تنفيذي+مدير تقني
                bager();
                ViewData["customer"] = new SelectList(_context.Customers, "Id", "ArabicName");

                return View();
            }

        }
        public void bager()
        {
            ViewBag.count = _context.Items.Count();
            ViewBag.count1 = _context.ItemTypes.Count();
            ViewBag.count2 = _context.Customers.Count();
            //ViewBag.count3 = _context.Appointments.Count();


        }


        //api:used in home
        public async Task<IActionResult> TopSeller(DateTime startdate, DateTime enddate)
        {


            var sub = from od in _context.OrderDetails
                      join q in _context.Quotations on od.QoutationId equals q.QuotationId
                      where q.QuotationDate >= startdate && q.QuotationDate <= enddate
                      group new { q, od } by new { q.employeeId }
                          into v
                      select new
                      {

                          emId = v.Key.employeeId,

                          value = v.Sum(x => x.od.TotalValue)

                      };
            var y = sub.OrderByDescending(v => v.value).Take(5).ToList();
            var query = from employee in _context.Employees
                        join vs in sub on employee.Id equals vs.emId
                        join branch in _context.Branches on employee.BranchId equals branch.BranchId

                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            branch = branch.BranchName,
                            total = vs.value,
                            name = employee.FirstName + employee.LastName

                        }
                        ;
            var x = query.OrderByDescending(v => v.total).Take(5).ToArray();





            return Json(x);

        }
        //api:used in home

        public async Task<IActionResult> Customersales(int customerId)
        {


            var sub = from od in _context.OrderDetails
                      join q in _context.Quotations on od.QoutationId equals q.QuotationId
                      where q.CustomerId == customerId
                      group new { q, od } by new { q.CustomerId, q.BranchId }
                          into v
                      select new
                      {
                          cId = v.Key.CustomerId,
                          bId = v.Key.BranchId,

                          value = v.Sum(x => x.od.TotalValue)

                      };
            var y = sub.ToList();
            var query = from Customer in _context.Customers
                        join vs in sub on Customer.Id equals vs.cId
                        join branch in _context.Branches on vs.bId equals branch.BranchId
                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            branch = branch.BranchName,
                            total = vs.value,
                            customername = Customer.ArabicName
                        }
                        ;
            var x = query.ToList();






            return Json(query);

        }

        //api:used in home

        public IActionResult TOP5Product()
        {

            var topProductsIds = _context.OrderDetails// table with a row for each view of a product
        .GroupBy(x => x.ItemId) //group all rows with same product id together
        .OrderByDescending(g => g.Count()) // move products with highest views to the top
        .Take(5) // take top 5
        .Select(x => x.Key) // get id of products
        .ToList(); // execute query and convert it to a list

            var topProducts = _context.Items// table with products information
                .Where(x => topProductsIds.Contains(x.ItemId));
            return Json(topProducts);

        }

        //api:used in home


        public IActionResult TOP5Branch()
        {

            var topBaranchIds = _context.Quotations
        .GroupBy(x => x.BranchId)
        .OrderByDescending(g => g.Count())
        .Take(5)
        .Select(x => x.Key)
        .ToList();

            var topBranches = _context.Branches

                .Where(x => topBaranchIds.Contains(x.BranchId));
            return Json(topBranches);

        }



        //api:used in home

        public async Task<IActionResult> chartApi()
        {


            var sub = from od in _context.OrderDetails
                      join q in _context.Quotations on od.QoutationId equals q.QuotationId
                      group new { q, od } by new { q.BranchId }
                          into v
                      select new
                      {

                          bId = v.Key.BranchId,

                          value = v.Sum(x => x.od.TotalValue)

                      };
            var y = sub.ToList();
            var query = from branch in _context.Branches
                        join vs in sub on branch.BranchId equals vs.bId

                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            branch = branch.BranchName,
                            total = vs.value,

                        }
                        ;
            var x = query.ToArray();






            return Json(query);

        }



       
    }
}