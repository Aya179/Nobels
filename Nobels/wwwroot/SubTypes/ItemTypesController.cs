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
    public class ItemTypesController : Controller
    {
        private readonly PricingDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment ;
        public ItemTypesController(PricingDBContext context ,IWebHostEnvironment env)
        {
            _appEnvironment = env;
            _context = context;
        }

        // GET: ItemTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.ItemTypes.ToListAsync());
        }

        // GET: ItemTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemTypes == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (itemType == null)
            {
                return NotFound();
            }

            return View(itemType);
        }

        // GET: ItemTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeId,TypeArName,TypeEnName,Photopath")] ItemType itemType)
        {
            string uploadPath = "ItemTypes/img";

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = itemType.TypeEnName + Path.GetExtension(file.FileName);
                            //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                        var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                        using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            itemType.Photopath = uploadPathWithfileName;
                        }
                    }
                }


                _context.Add(itemType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemType);
        }

        // GET: ItemTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemTypes == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes.FindAsync(id);
            if (itemType == null)
            {
                return NotFound();
            }
            return View(itemType);
        }

        // POST: ItemTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeId,TypeArName,TypeEnName,Photopath")] ItemType itemType)
        {
            if (id != itemType.TypeId)
            {
                return NotFound();
            }
            string uploadPath = "ItemTypes/img";
            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = itemType.TypeEnName + Path.GetExtension(file.FileName);
                            //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                            var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                            using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                itemType.Photopath = uploadPathWithfileName;
                            }
                        }
                    }

                    _context.Update(itemType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemTypeExists(itemType.TypeId))
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
            return View(itemType);
        }

        // GET: ItemTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemTypes == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (itemType == null)
            {
                return NotFound();
            }

            return View(itemType);
        }

        // POST: ItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'PricingDBContext.ItemTypes'  is null.");
            }
            var itemType = await _context.ItemTypes.FindAsync(id);
            if (itemType != null)
            {
                _context.ItemTypes.Remove(itemType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemTypeExists(int id)
        {
          return _context.ItemTypes.Any(e => e.TypeId == id);
        }
    }
}
