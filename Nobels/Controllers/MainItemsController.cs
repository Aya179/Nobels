using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class MainItemsController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private readonly IWebHostEnvironment _appEnvironment;


        public MainItemsController(db_a8d242_exadecor2Context context, IWebHostEnvironment env)
        {
            _appEnvironment = env;

            _context = context;
        }

        // GET: MainItems
        public async Task<IActionResult> Index()
        {
              return View(await _context.MainItem.ToListAsync());
        }

        // GET: MainItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MainItem == null)
            {
                return NotFound();
            }

            var mainItem = await _context.MainItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mainItem == null)
            {
                return NotFound();
            }

            return View(mainItem);
        }

        // GET: MainItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MainItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MainItemName,MainItemEnName,MainItemPhoto")] MainItem mainItem)
        {

            string uploadPath = "MainItems/img";
            if(mainItem.MainItemPhoto != null)
            {
                if (ModelState.IsValid)
                {


                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = mainItem.MainItemEnName + Path.GetExtension(file.FileName);
                            //Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                            var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                            using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                mainItem.MainItemPhoto = uploadPathWithfileName;
                            }
                        }
                    }

                }
                else
                    mainItem.MainItemPhoto="non";
            


                _context.Add(mainItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mainItem);
        }

        // GET: MainItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MainItem == null)
            {
                return NotFound();
            }

            var mainItem = await _context.MainItem.FindAsync(id);
            if (mainItem == null)
            {
                return NotFound();
            }
            return View(mainItem);
        }

        // POST: MainItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MainItemName,MainItemEnName,MainItemPhoto")] MainItem mainItem)
        {
            if (id != mainItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mainItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainItemExists(mainItem.Id))
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
            return View(mainItem);
        }

        // GET: MainItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MainItem == null)
            {
                return NotFound();
            }

            var mainItem = await _context.MainItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mainItem == null)
            {
                return NotFound();
            }

            return View(mainItem);
        }

        // POST: MainItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MainItem == null)
            {
                return Problem("Entity set 'PricingDBContext.MainItem'  is null.");
            }
            var mainItem = await _context.MainItem.FindAsync(id);
            if (mainItem != null)
            {
                _context.MainItem.Remove(mainItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MainItemExists(int id)
        {
          return _context.MainItem.Any(e => e.Id == id);
        }

        public string getArabicName(int id)
        {
            return _context.MainItem.Find(id).MainItemName;
        }
    }
}
