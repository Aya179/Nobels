using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Nobels.Models;
using System.Threading.Tasks;


namespace Nobels.Controllers
{
    public class AccountController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        




        private UserManager<Employee> userManager1 { get; }
        private RoleManager<AppRole> _roleManager{get;}

       

        private SignInManager<Employee> signInManager1 { get; }
        public AccountController(RoleManager<AppRole> roleManager, UserManager<Employee> userManager, SignInManager<Employee> signInManager, db_a8d242_exadecor2Context context)
        {
            _roleManager = roleManager;

            userManager1 = userManager;
            signInManager1 = signInManager;
            _context = context;
           

        }
       

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login userModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

           // var user = await userManager1.FindByEmailAsync(userModel.Email);
            var user1 = _context.Employees.FirstOrDefault(x => x.EmployeeNumber == userModel.number);

            if (user1 != null &&
                await userManager1.CheckPasswordAsync(user1, userModel.Password))
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user1.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user1.UserName));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                //identity.AddClaim(new Claim(ClaimTypes, user.UserName));


                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(identity));
                
                //HttpContext.Items.Add
                 return RedirectToLocal(returnUrl);
               
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        public IActionResult getObject(int id)
        {
            return View(); 
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Home", "Home");

        }
        public async Task<Employee> GetUserData()
        {
            return await userManager1.GetUserAsync(HttpContext.User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager1.SignOutAsync();
            return RedirectToAction("Home", "Home");
        }

        public async Task CreateRolesandUsers()
        {
            bool x = await _roleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                // first we create Admin rool    
                var role = new AppRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = new Employee();
                user.UserName = "default";
                user.Email = "default@default.com";

                string userPWD = "somepassword";

                IdentityResult chkUser = await userManager1.CreateAsync(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = await userManager1.AddToRoleAsync(user, "Admin");
                }
            }

            // creating Creating Manager role     
            x = await _roleManager.RoleExistsAsync("Manager");
            if (!x)
            {
                var role = new AppRole();
                role.Name = "Manager";
                await _roleManager.CreateAsync(role);
            }

            // creating Creating Employee role     
            x = await _roleManager.RoleExistsAsync("Sales");
            if (!x)
            {
                var role = new AppRole();
                role.Name = "Sales";
                await _roleManager.CreateAsync(role);
            }
        }


        public IActionResult TestRole()
        {
            CreateRolesandUsers();
            return View();
        }






    }
}
