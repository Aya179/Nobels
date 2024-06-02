using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;
        private RoleManager<AppRole> _roleManager;
        public EmployeesController(db_a8d242_exadecor2Context context, UserManager<Employee> usrMgr, IPasswordHasher<Employee> passwordHasher, RoleManager<AppRole> roleManager)
        {
            userManager = usrMgr;
            _context = context;
            _passwordHasher = passwordHasher;
            _roleManager = roleManager;


            // _passwordHasher = passwordHasher;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Employees.Include(e => e.Branch).Include(e => e.Role);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            var branch = _context.Branches.Where(i => i.BranchId == employee.BranchId).FirstOrDefault();
            var Role = _context.EmployeeRoles.Where(i => i.RoleId == employee.RoleId).FirstOrDefault();

            ViewData["BranchId"] = branch.BranchName;
            ViewData["RoleId"] = Role.RoleName;
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branches, "BranchId", "BranchName");
            ViewData["RoleId1"] = new SelectList(_context.EmployeeRoles.Where(r => r.RoleId == 4 || r.RoleId == 3), "RoleId", "RoleName");

            ViewData["RoleId"] = new SelectList(_context.EmployeeRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeNumber,BranchId,RoleId,FirstName,LastName,Phone,Email,RegisterDate,UserName,Password")] Employee employee)
        {
            if (ModelState.IsValid)
            {


                IdentityResult result = await userManager.CreateAsync(employee, employee.Password);
                employee.RegisterDate = DateTime.Now;
                employee.EmailConfirmed = true;
                employee.NormalizedEmail = employee.Email.ToUpper();
                employee.NormalizedUserName = employee.UserName.ToUpper();
                employee.PasswordHash = _passwordHasher.HashPassword(employee, employee.Password);
                employee.SecurityStamp = Guid.NewGuid().ToString();
                employee.Phone = "+05" + employee.Phone;
                _context.Add(employee);
                await _context.SaveChangesAsync();
                var user = _context.Employees.FirstOrDefault(x => x.EmployeeNumber == employee.EmployeeNumber);
                var dbrole = _context.EmployeeRoles.Where(rr => rr.RoleId == user.RoleId).FirstOrDefault();
                var role = _context.Roles.FirstOrDefault(r => r.Name == dbrole.RoleName);
                

               
                IdentityUserRole<int> userRole = new IdentityUserRole<int>();
                
                userRole.RoleId = role.Id;
                userRole.UserId = user.Id;
                
                _context.UserRoles.Add(userRole);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "BranchId", "BranchName", employee.BranchId);
            ViewData["RoleId"] = new SelectList(_context.EmployeeRoles, "RoleId", "RoleName", employee.RoleId);
            ViewData["RoleId1"] = new SelectList(_context.EmployeeRoles.Where(r=>r.RoleId==4||r.RoleId==3), "RoleId", "RoleName", employee.RoleId);

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
            string phone = employee.Phone;
            
            string str = "+05";
            if (phone.StartsWith(str))
            {
                phone = phone.Substring(3);
                ViewBag.phone=phone;
            }


            if (employee == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.EmployeeRoles, "RoleId", "RoleName");

            //var branch = _context.Branches.Where(i => i.BranchId == employee.BranchId).FirstOrDefault();
            //var Role = _context.EmployeeRoles.Where(i => i.RoleId == employee.RoleId).FirstOrDefault();

            //ViewBag.BranchId = branch.BranchName;
            //ViewBag.RoleId = Role.RoleName;
            return View(employee);

        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeNumber,BranchId,RoleId,FirstName,LastName,Phone,Email,UserName,Password")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Employees.FirstOrDefault(x => x.EmployeeNumber == employee.EmployeeNumber);
                    user.Password = employee.Password;
                    //string str = "+05";
                    //bool result= employee.Phone.StartsWith(str);
                    user.FirstName=employee.FirstName;
                    user.LastName=employee.LastName;
                    user.Phone = "+05" + employee.Phone; 
                    user.UserName = employee.UserName;
                    user.NormalizedUserName=employee.UserName.ToUpper();
                    user.Email = employee.Email;
                    user.NormalizedEmail=employee.Email.ToUpper();
                    user.PasswordHash = _passwordHasher.HashPassword(user, employee.Password);
                    user.RoleId=employee.RoleId;


                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    var dbrole = _context.EmployeeRoles.Where(rr => rr.RoleId == user.RoleId).FirstOrDefault();
                    var role = _context.Roles.FirstOrDefault(r => r.Name == dbrole.RoleName);

                    var updatedRole=_context.UserRoles.Where(rr => rr.UserId == user.Id).FirstOrDefault();
                    _context.UserRoles.Remove(updatedRole);
                    _context.SaveChanges();

                   



                    IdentityUserRole<int> userRole = new IdentityUserRole<int>();

                    userRole.RoleId = role.Id;
                    userRole.UserId = user.Id;

                    _context.UserRoles.Add(userRole);
                    _context.SaveChanges();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            //var branch = _context.Branches.Where(i => i.BranchId == employee.BranchId).FirstOrDefault();
            //var Role = _context.EmployeeRoles.Where(i => i.RoleId == employee.RoleId).FirstOrDefault();

            //ViewData["BranchId"] = branch.BranchName;
            //ViewData["RoleId"] = Role.RoleName;
            ViewData["RoleId"] = new SelectList(_context.EmployeeRoles, "RoleId", "RoleName", employee.RoleId);

            return View(employee);
        }
        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'PricingDBContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employees.Any(e => e.Id == id);
        }
    }
}
