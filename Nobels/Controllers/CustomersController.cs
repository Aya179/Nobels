using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class CustomersController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;


        public CustomersController(db_a8d242_exadecor2Context context, UserManager<Employee> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        public IActionResult csvView()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile formFile)
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);

            var branchid = (int)user.BranchId;
            var data = new MemoryStream();
            await formFile.CopyToAsync(data);

            data.Position = 0;
            using (var reader = new StreamReader(data))
            {
                var bad = new List<string>();
                var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    HeaderValidated = null,

                    MissingFieldFound = null,
                    Delimiter = ";",
                    BadDataFound = context =>
                    {
                        bad.Add(context.RawRecord);
                    }
                };
                using (var csvReader = new CsvReader(reader, conf))
                {
                    csvReader.Read();
                    while (csvReader.Read())
                    {
                        var arabicname = csvReader.GetField(0).ToString(); 
                        
                        var englishname = csvReader.GetField(1).ToString(); 
                        var email = csvReader.GetField(2).ToString(); 
                        var phone = csvReader.GetField(3).ToString(); 
                        var addr = csvReader.GetField(4).ToString(); 
                        var city = csvReader.GetField(5).ToString(); 
                        var gender = csvReader.GetField(6).ToString(); 
                        await _context.Customers.AddAsync(new Customer
                        {
                            ArabicName = arabicname,
                            EnglishName = englishname,
                            SecondAddress="-",
                            PhonNumber="-",
                            Email = email,
                            Phone=phone,
                            Address=addr,
                            CityId= int.Parse(city),
                                             
                            Gender = gender
                            ,

                        });;
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction(nameof(Index));
            // return ViewComponent("Index");
        }
        public IActionResult CityCoustmerView()
        {
           
            ViewData["City"] = new SelectList(_context.Cities, "CityId", "CityName");

            return View();

        }


        public IActionResult CityCoustmerAsync(int cityId)
        {
            var cus = _context.Customers.Where(q => q.CityId ==cityId).Include(qu => qu.City);
            return Json(cus.ToList());

        }

        public IActionResult GetBranchName(int bId)
        {
            var branch = _context.Branches.Where(b=>b.BranchId==bId).FirstOrDefault();
            var name = branch.BranchName;
            return Json(name);

        }


        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Customers.Include(c => c.City);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                Employee user = await userManager.GetUserAsync(HttpContext.User);

                customer.branchId = (int)user.BranchId;
                customer.Phone = "+05" + customer.Phone;
                customer.PhonNumber = "+05" + customer.PhonNumber;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'PricingDBContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return _context.Customers.Any(e => e.Id == id);
        }
    }
}
