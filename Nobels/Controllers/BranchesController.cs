using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class BranchesController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private readonly IWebHostEnvironment _env;

        public BranchesController(db_a8d242_exadecor2Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Branches.Include(b => b.City);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.City)
                .FirstOrDefaultAsync(m => m.BranchId == id);
            if (branch == null)
            {
                return NotFound();
            }
            string[] components = branch.branchImg.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            var imagName = components[components.Length - 1];
            
            ViewBag.imgurl = imagName;
            return View(branch);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchId,BranchName,Location,Phone,Email,Address,CityId,Notes,BranchType")] Branch branch, IFormFile file1)
        {
            var branchImg = HttpContext.Request.Form.Files[0];
              branch.branchImg = "-";
            if (ModelState.IsValid)
            {
                
                _context.Add(branch);
                    await _context.SaveChangesAsync();

                if (branchImg == null || branchImg.Length == 0)
                {
                    //return BadRequest("No video file uploaded.");
                   
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {



                    string path = Path.Combine(_env.WebRootPath, "Branches").ToLower();
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }





                    string imgPath = Path.Combine(path, branch.BranchId.ToString()+ System.IO.Path.GetExtension(branchImg.FileName));
                    using (var stream = new FileStream(imgPath, FileMode.Create))
                    {
                        branchImg.CopyTo(stream);
                    }



                    updatebranchImgPath(branch.BranchId, imgPath);







                    return RedirectToAction(nameof(Index));
                }

            }


       
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", branch.CityId);
            return View(branch);
        }

        public void updatebranchImgPath(int branchId, string path)
        {
            var branch = _context.Branches.Find(branchId);
            branch.branchImg = path;
            _context.Branches.Update(branch);
            _context.SaveChanges();

        }


        public async Task<JsonResult> AddBranchAsync(string branchName, string location, string phone, string email,string dddress, int cityId, string notes,int branchType, IFormFile file1)

        {

            var branchImg = HttpContext.Request.Form.Files[0];
            Branch branch = new Branch();
            branch.BranchName= branchName;
            branch.Location=location;
            branch.Phone= phone;
            branch.Email= email;
            branch.CityId= cityId;
            branch.Notes= notes;
            branch.BranchType= branchType;
            branch.Address = dddress;
           
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            return Json(branch);





        }



        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", branch.CityId);
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BranchId,BranchName,Location,Phone,Email,Address,CityId,Notes,BranchType")] Branch branch,IFormFile file1)
        {
            var existingbranch = _context.Branches.Find(id);
            existingbranch.BranchName= branch.BranchName;
            existingbranch.Location= branch.Location;   
            existingbranch.CityId= branch.CityId;   
            existingbranch.Notes= branch.Notes;
            existingbranch.BranchType= branch.BranchType;
            existingbranch.Address= branch.Address;
            existingbranch.Phone = branch.Phone;
            existingbranch.Email = branch.Email;

            string pattth = existingbranch.branchImg;
            if (id != branch.BranchId)
            {
                return NotFound();
            }
            var branchImg = HttpContext.Request.Form.Files[0];
            if (ModelState.IsValid)
            {
                try
                {
                   

                    if (branchImg == null || branchImg.Length == 0)
                    {
                       // branch.branchImg = pattth;
                      _context.Update(existingbranch);
                    await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (pattth != "-") { 
                            string[] components = pattth.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        var imagName = components[components.Length - 1];
                        string path1 = Path.Combine(_env.WebRootPath, "Branches", imagName).ToLower();
                        if (System.IO.File.Exists(path1))
                        {
                            System.IO.File.Delete(path1);

                        } 
                        }

                        _context.Update(existingbranch);
                        await _context.SaveChangesAsync();

                        string path = Path.Combine(_env.WebRootPath, "Branches").ToLower();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }





                        string imgPath = Path.Combine(path, branch.BranchId.ToString() + System.IO.Path.GetExtension(branchImg.FileName));
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            branchImg.CopyTo(stream);
                        }



                        updatebranchImgPath(existingbranch.BranchId, imgPath);







                        return RedirectToAction(nameof(Index));



                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.BranchId))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", branch.CityId);
            return View(branch);
        }

        // GET: Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.City)
                .FirstOrDefaultAsync(m => m.BranchId == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Branches == null)
            {
                return Problem("Entity set 'PricingDBContext.Branches'  is null.");
            }
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
            }
            


            string[] components = branch.branchImg.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            var imagName= components[components.Length - 1];
string path = Path.Combine(_env.WebRootPath, "Branches",imagName).ToLower();
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
               
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
          return _context.Branches.Any(e => e.BranchId == id);
        }
    }
}
