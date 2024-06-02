using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheThing.Models;

namespace TheThing.Controllers
{
    public class SubTypesController : Controller
    {
        private readonly PricingDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;


        public SubTypesController(PricingDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: SubTypes
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.SubTypes.Include(s => s.Type);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: SubTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubTypes == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.SubTypeId == id);
            if (subType == null)
            {
                return NotFound();
            }

            return View(subType);
        }

        // GET: SubTypes/Create
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");
            return View();
        }

        // POST: SubTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubTypeId,TypeId,SubTypeEnName,SubTypeArName,Photopath,Notes")] SubType subType)
        {
            string uploadPath = "SubTypes/img";

            if (ModelState.IsValid)
            {


                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = subType.SubTypeEnName + Path.GetExtension(file.FileName);
                        //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                        var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                        using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            subType.Photopath = uploadPathWithfileName;
                        }
                    }
                }

                _context.Add(subType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName", subType.TypeId);
            return View(subType);
        }

        // GET: SubTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubTypes == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes.FindAsync(id);
            if (subType == null)
            {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName", subType.TypeId);
            return View(subType);
        }

        // POST: SubTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubTypeId,TypeId,SubTypeEnName,SubTypeArName,Photopath,Notes")] SubType subType)
        {
            if (id != subType.SubTypeId)
            {
                return NotFound();
            }
            string uploadPath = "SubTypes/img";

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = subType.SubTypeEnName + Path.GetExtension(file.FileName);
                            //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                            var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                            using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                subType.Photopath = uploadPathWithfileName;
                            }
                        }
                    }

                    _context.Update(subType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubTypeExists(subType.SubTypeId))
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
            ViewData["TypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName", subType.TypeId);
            return View(subType);
        }

        // GET: SubTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubTypes == null)
            {
                return NotFound();
            }

            var subType = await _context.SubTypes
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.SubTypeId == id);
            if (subType == null)
            {
                return NotFound();
            }

            return View(subType);
        }

        // POST: SubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubTypes == null)
            {
                return Problem("Entity set 'PricingDBContext.SubTypes'  is null.");
            }
            var subType = await _context.SubTypes.FindAsync(id);
            if (subType != null)
            {
                _context.SubTypes.Remove(subType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubTypeExists(int id)
        {
          return _context.SubTypes.Any(e => e.SubTypeId == id);
        }
    }
}
