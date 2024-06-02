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
    public class ItemsController : Controller
    {
        private readonly PricingDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ItemsController(PricingDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Items.Include(i => i.ItemSubType);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemSubType)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeId");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemArName,ItemEnName,ItemCode,ItemCurrentPrice,ItemSubTypeId,ItemPhotoPath,Notes")] Item item)
        {
            string uploadPath = "Items/img";

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = item.ItemEnName + Path.GetExtension(file.FileName);
                        //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                        var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                        using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            item.ItemPhotoPath = uploadPathWithfileName;
                        }
                    }
                }


                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeId", item.ItemSubTypeId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeId", item.ItemSubTypeId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemArName,ItemEnName,ItemCode,ItemCurrentPrice,ItemSubTypeId,ItemPhotoPath,Notes")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
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
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeId", item.ItemSubTypeId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemSubType)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'PricingDBContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
          return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
