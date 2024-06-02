using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;
using Color = Nobels.Models.Color;

namespace Nobels.Controllers
{
    public class ColorsController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;

        public ColorsController(db_a8d242_exadecor2Context context)
        {
            _context = context;
        }

        // GET: Colors
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Colors.Include(s => s.ItemTypeId);

            return View(await pricingDBContext.ToListAsync());
        }

        // GET: Colors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.ColorId == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        public IActionResult addColorApi(string ColorName, string ColorCode, string ColorTemplatePhoto, string? Note, int itemTypeId1)
        {
            if (ModelState.IsValid)
            {
                Color color = new Color();
                color.itemTypeId1 = itemTypeId1;
                color.ColorName = ColorName;
                color.ColorCode = ColorCode;
                color.ColorTemplatePhoto = ColorTemplatePhoto;
                if (Note == null)
                {
                    color.Note = "noNote";
                }
                else color.Note = Note;
                Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ItemId = items[i].ItemId;

                    ic.isEnabled = true;
                    ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                    color.ItemColors.Add(ic);
                }


                _context.Colors.Add(color);
                _context.SaveChanges();

                return Json(color);
            }
            return Json("not added");
        }

        // GET: Colors/Create
        public IActionResult Create()
        {
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");
            return View();
        }

        // POST: Colors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ColorId,ColorName,ColorCode,ColorTemplatePhoto,Note,itemTypeId1")] Color color)
        {
            if (ModelState.IsValid)
            {

                //color.ColorCode = "colorCode";
                
                Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
               for(int i=0;i<items.Length;i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ItemId = items[i].ItemId;
                    
                    ic.isEnabled = true;
                    ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                   color.ItemColors.Add(ic);
                }
                color = _context.Add(color).Entity;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Colors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");

            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return View(color);
        }

        // POST: Colors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ColorId,ColorName,ColorCode,ColorTemplatePhoto,Note,itemTypeId1")] Color color)
        {
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");

            if (id != color.ColorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(color);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorExists(color.ColorId))
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
            return View(color);
        }


        public int deactivate(int id)
        {
            Color color=  _context.Colors.Find(id);
            ItemColor[] ics = _context.ItemColors.Where(c => c.ColorId == id).ToArray();
           for(int i=0;i< ics.Length;i++)
            {
                ics[i].isEnabled = false;
                _context.ItemColors.Update(ics[i]);
            }
            _context.SaveChanges();
            return 0;
        }

        public int activate(int id)
        {
            Color color = _context.Colors.Find(id);
            ItemColor[] ics = _context.ItemColors.Where(c => c.ColorId == id).ToArray();
            for (int i = 0; i < ics.Length; i++)
            {
                ics[i].isEnabled = true;
                _context.ItemColors.Update(ics[i]);
            }
            _context.SaveChanges();
            return 0;
        }

        // GET: Colors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.ColorId == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }
        public IActionResult itemTypeSelectView(int? id)
        {
            
            return View();
        }

        public IActionResult itemTypeSelect(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var color =  _context.Colors
                .FirstOrDefault(m => m.ColorId == id);
            Color[] samecolors = _context.Colors.Where(c => c.ColorName == color.ColorName).ToArray();
            List<ItemType> selecteditemtypes = new List<ItemType>();
            List<ItemType> itemTypes = _context.ItemTypes.ToList();
           // List<ItemType> itemTypes1 = _context.ItemTypes.ToList();
          //  itemTypes1.Clear();
            for (int i = 0; i < samecolors.Length; i++)
            {
                selecteditemtypes.Add(samecolors[i].ItemTypeId);
            }
            foreach (var itemtype in selecteditemtypes)
            {
              //  itemTypes1.Add(itemtype);
                itemTypes.Remove(itemtype);
               
            }
            //ViewBag.notSelectedItemTypes = itemTypes;
            //ViewBag.selectedTypes = selecteditemtypes;
           // ViewData["notSelectedItemTypes"] = new SelectList(itemTypes, "TypeId", "TypeArName");
            //ViewBag.selectedTypes = new SelectList(itemTypes1, "TypeId", "TypeArName");
            if (color == null)
            {
                return NotFound();
            }

            return Json(new { data1 = itemTypes , data2=selecteditemtypes });
        }





        public JsonResult updateItemType(int colorId,int itemTypeId)

        {


            var itemtype = _context.ItemTypes.Find(itemTypeId);

            var color1 = _context.Colors.Find(colorId);


            Color color = new Color();
            
            color.itemTypeId1 = itemTypeId;
            color.ColorName = color1.ColorName;
            color.ColorCode = color1.ColorCode;
            color.ColorTemplatePhoto = color1.ColorTemplatePhoto;
            if (color1.Note == null)
            {
                color.Note = "noNote";
            }
            else color.Note = color1.Note;
            Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                ItemColor ic = new ItemColor();
                ic.ItemId = items[i].ItemId;

                ic.isEnabled = true;
                ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                color.ItemColors.Add(ic);
            }


            _context.Colors.Add(color);
            _context.SaveChanges();

            return Json(color);
          
             
         //   return Json(new { itemtype });





        }

        public JsonResult updateItemTypenew(int colorId, int itemTypeId)

        {


            var itemtype = _context.ItemTypes.Find(itemTypeId);

            var color1 = _context.Colors.Find(colorId);
          // var colorforitemtypeExist = _context.Colors.Where(i => i.itemTypeId1 == itemTypeId).ToArray();
           var colorforitemtypeExist = _context.Colors.Where(i => i.ColorName == color1.ColorName).ToArray();

            var result = Array.Find(colorforitemtypeExist, element => element.itemTypeId1==itemTypeId); // returns Boski
            if (result == null)
            {



                Color color = new Color();

                color.itemTypeId1 = itemTypeId;
                color.ColorName = color1.ColorName;
                color.ColorCode = color1.ColorCode;
                color.ColorTemplatePhoto = color1.ColorTemplatePhoto;
                if (color1.Note == null)
                {
                    color.Note = "noNote";
                }
                else color.Note = color1.Note;
                Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ItemId = items[i].ItemId;

                    ic.isEnabled = true;
                    ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                    color.ItemColors.Add(ic);
                }


                _context.Colors.Add(color);
                _context.SaveChanges();

                return Json(color);
            }
            else { return Json("exist"); }
           

        }

        public JsonResult deletItemTypeNew(int colorId, int itemTypeId)

        {


            var itemtype = _context.ItemTypes.Find(itemTypeId);
            var color = _context.Colors.Where(i => i.ColorId == colorId && i.itemTypeId1 == itemTypeId).FirstOrDefault();
          //  var color1 = _context.Colors.Find(colorId);
            //var colorforitemtypeExist = _context.Colors.Where(i => i.itemTypeId1 == itemTypeId).ToArray();
            var colorforitemtypeExist = _context.Colors.Where(i => i.ColorName == color.ColorName).ToArray();

            var result = Array.Find(colorforitemtypeExist, element => element.itemTypeId1 == itemTypeId); // returns Boski
            if (result != null)
            {


                Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ItemId = items[i].ItemId;

                    ic.isEnabled = true;
                    ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                    color.ItemColors.Remove(ic);
                }


                _context.Colors.Remove((Color)color);

                _context.SaveChanges();

                return Json(color);


            }
            else { return Json("exist"); }




        }


        public JsonResult deletItemType(int colorId,int itemTypeId)

        {


            var itemtype = _context.ItemTypes.Find(itemTypeId);
            var Colorname = _context.Colors.Find(colorId);
            String Name = Colorname.ColorName;
            var color = _context.Colors.Where(i=>i.ColorName== Name && i.itemTypeId1==itemTypeId).FirstOrDefault();

            if (color != null)
            {
                Item[] items = _context.Items.Where(c => c.ItemSubType.TypeId == color.itemTypeId1).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ItemId = items[i].ItemId;

                    ic.isEnabled = true;
                    ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                    color.ItemColors.Remove(ic);
                }


                _context.Colors.Remove((Color)color);

                _context.SaveChanges();
                return Json(color);
            }

            else

            return Json("EXixt");

          





        }



        // POST: Colors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Colors == null)
            {
                return Problem("Entity set 'PricingDBContext.Colors'  is null.");
            }
            var color = await _context.Colors.FindAsync(id);
            if (color != null)
            {
                _context.Colors.Remove(color);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ColorExists(int id)
        {
          return _context.Colors.Any(e => e.ColorId == id);
        }

        public IActionResult fixColors()
        {
            ItemColor[] ics = _context.ItemColors.ToArray();
            Item[] items = _context.Items.ToArray();
            for(int i = 0; i < items.Length; i++)
            {
                ItemType it =_context.ItemTypes.Find( _context.SubTypes.Find(items[i].ItemSubTypeId).TypeId);
                Color[] cs = _context.Colors.Where(c => c.itemTypeId1 == it.TypeId).ToArray();
                for(int j = 0; j < cs.Length; j++)
                {
                    bool isExits = false;
                    for(int k = 0; k < ics.Length; k++) {
                        if ((ics[k].ItemId == items[i].ItemId) && ics[k].ColorId == cs[j].ColorId)
                        {
                            isExits = true;
                        }
                    }
                    if (!isExits)
                    {
                        ItemColor ic = new ItemColor();
                        ic.isEnabled = true;
                        ic.ItemId = items[i].ItemId;
                        ic.ColorId = cs[j].ColorId;
                        ic.SpecialPrice = (double)items[i].ItemCurrentPrice;
                        _context.ItemColors.Add(ic);
                    }
                }
            }
            _context.SaveChanges();
            return Ok(new { ItemColors = ics });
        }
    }
}
