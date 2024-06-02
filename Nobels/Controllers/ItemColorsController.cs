using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class ItemColorsController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;

        public ItemColorsController(db_a8d242_exadecor2Context context)
        {
            _context = context;
        }

        // GET: ItemColors
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.ItemColors.Include(i => i.Color).Include(i => i.Item);
            return View(await pricingDBContext.ToListAsync());
        }

        // GET: ItemColors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemColors == null)
            {
                return NotFound();
            }

            var itemColor = await _context.ItemColors
                .Include(i => i.Color)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.ItemColorId == id);
            if (itemColor == null)
            {
                return NotFound();
            }

            return View(itemColor);
        }

        // GET: ItemColors/Create
        public IActionResult Create()
        {
            var Maxid = _context.Items.Max(X=>X.ItemId);
            var item=_context.Items.FirstOrDefault(X=>X.ItemId==Maxid);
            //var newItem = _context.Items.Find(items.ItemId);
            ViewBag.itemID1 = item.ItemId;
            ViewBag.itemName = item.ItemArName;
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName");
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName");

            return View();
        }
       


        // POST: ItemColors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemColorId,ColorId,ItemId,SpecialPrice")] ItemColor itemColor)
        {
            if (ModelState.IsValid)
            {
                //var colorItem = _context.Items.Where(item => item.ItemId == ItemId);
                
                _context.Add(itemColor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", itemColor.ColorId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName", itemColor.ItemId);
            return View(itemColor);
        }
        //public async Task<IActionResult> createcolor(int id)
        //{
        //    var color=_context.Items.Where(item =>item.ItemId==id);
        //    return View(); 
        //}
        // GET: ItemColors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemColors == null)
            {
                return NotFound();
            }

            var itemColor = await _context.ItemColors.FindAsync(id);
            if (itemColor == null)
            {
                return NotFound();
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", itemColor.ColorId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName", itemColor.ItemId);
            return View(itemColor);
        }

        // POST: ItemColors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemColorId,ColorId,ItemId,SpecialPrice")] ItemColor itemColor)
        {
            if (id != itemColor.ItemColorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemColor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemColorExists(itemColor.ItemColorId))
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
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", itemColor.ColorId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName", itemColor.ItemId);
            return View(itemColor);
        }

        // GET: ItemColors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemColors == null)
            {
                return NotFound();
            }

            var itemColor = await _context.ItemColors
                .Include(i => i.Color)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.ItemColorId == id);
            if (itemColor == null)
            {
                return NotFound();
            }

            return View(itemColor);
        }

        // POST: ItemColors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemColors == null)
            {
                return Problem("Entity set 'PricingDBContext.ItemColors'  is null.");
            }
            var itemColor = await _context.ItemColors.FindAsync(id);
            if (itemColor != null)
            {
                _context.ItemColors.Remove(itemColor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemColorExists(int id)
        {
          return _context.ItemColors.Any(e => e.ItemColorId == id);
        }
    }
}
