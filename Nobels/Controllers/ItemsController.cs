using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;

namespace Nobels.Controllers
{
    public class ItemsController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private UserManager<Employee> userManager;

        public ItemsController(db_a8d242_exadecor2Context context, IWebHostEnvironment appEnvironment, UserManager<Employee> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
           this.userManager = userManager;
        }



        public IActionResult csvView()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile formFile)
        {
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
                    Delimiter=";",
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
                        var ItemArName = csvReader.GetField(0).ToString();
                        var ItemCode = csvReader.GetField(1).ToString();
                        var ItemCurrentPrice = csvReader.GetField(2);
                        var ItemSubTypeId = csvReader.GetField(3);
                        await _context.Items.AddAsync(new Item
                        {
                            ItemArName = ItemArName,
                            ItemCode =  ItemCode,
                           ItemCurrentPrice = double.Parse(ItemCurrentPrice)
                                             ,
                            ItemSubTypeId= 66
                            ,
                           
                        });
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction(nameof(Index));
            // return ViewComponent("Index");
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var pricingDBContext = _context.Items.Include(i => i.ItemSubType);

            //Employee user = await userManager.GetUserAsync(HttpContext.User);
            //var roles = await userManager.GetRolesAsync(user);

          //  if (user != null && roles != null && ((roles.Contains("مدير تنفيذي")) || (roles.Contains("مدير منتج"))))
           // {
                // if(userId==qu1.employeeId)

               // ViewBag.role = 1;

           // }
            //else { ViewBag.role = 0; }
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
           // ViewData["MainItemId"] = new SelectList(_context.MainItem, "Id", "MainItemName");

            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemArName,ItemEnName,ItemCode,ItemCurrentPrice,ItemSubTypeId,ItemPhotoPath,Notes,UOM,ICost,FCost,RMCost")] Item item)
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
                       
                        var uploadPathWithfileName = Path.Combine(uploadPath, fileName);

                        var uploadAbsolutePath = Path.Combine(_appEnvironment.WebRootPath, uploadPathWithfileName);

                        using (var fileStream = new FileStream(uploadAbsolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            item.ItemPhotoPath = uploadPathWithfileName;
                        }
                    }
                }

                item.ItemCode = "itemCode";
                var subTypeid = _context.SubTypes.Where(i => i.SubTypeId == item.ItemSubTypeId).Include(t => t.Type).FirstOrDefault();
                var colors = _context.Colors.Where(c => c.itemTypeId1 == subTypeid.TypeId).ToArray();
                for (int i = 0; i < colors.Length; i++)
                {
                    ItemColor ic = new ItemColor();
                    ic.ColorId = colors[i].ColorId;
                    ic.SpecialPrice = (double)item.ItemCurrentPrice;
                    ic.isEnabled = true;
                    item.ItemColors.Add(ic);
                }
                item.COG = item.ICost + item.RMCost + item.FCost;

                _context.Add(item);
                _context.SaveChanges();
                return RedirectToAction("Create", "ItemColors");
            }
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName", item.ItemSubTypeId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

           // ViewData["MainItemId"] = new SelectList(_context.MainItem, "Id", "MainItemName");

            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName", item.ItemSubTypeId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemArName,ItemEnName,ItemCode,ItemCurrentPrice,ItemSubTypeId,ItemPhotoPath,Notes,UOM,ICost,FCost,RMCost")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    item.COG = item.ICost + item.RMCost + item.FCost;

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
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName", item.ItemSubTypeId);
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

        public IActionResult AddItem()
        {
            ViewData["item"] = new SelectList(_context.Items, "ItemId", "ItemEnName");
            ViewData["itemMain"] = new SelectList(_context.ItemTypes, "TypeId", "TypeEnName");
            ViewData["Subtype"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName");
            ViewData["color"] = new SelectList(_context.ItemColors, "ColorId", "ColorId");






            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("ItemId,ItemArName,ItemEnName,ItemCode,ItemCurrentPrice,ItemSubTypeId,ItemPhotoPath,Notes")] Item item)
        {

            if (ModelState.IsValid)
            {


                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName", item.ItemSubTypeId);
            return View(item);
        }

        public IActionResult AddColor(int id)
        {
            var item = _context.Items.Find(id);
            ViewBag.name = item.ItemArName;

            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName");
            // ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName");

            return View();
        }


        // POST: ItemColors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColor([Bind("ItemColorId,ColorId,ItemId,SpecialPrice,isEnabled")] ItemColor itemColor)
        {
            if (ModelState.IsValid)
            {
                //var colorItem = _context.Items.Where(item => item.ItemId == ItemId);
                itemColor.isEnabled = true;
                _context.Add(itemColor);
                await _context.SaveChangesAsync(); 
                return RedirectToAction("Edit", "Items", new { id = itemColor.ItemId });
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", itemColor.ColorId);
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemArName", itemColor.ItemId);
            return View(itemColor);
        }
        public IActionResult GetItemName(int id)
        {
            var itemName = _context.Items.Find(id);

            String name = itemName.ItemArName;

            return Json(name);
        }


        public JsonResult GetItemColor(int Itemid)
        {
            //  List<ItemColor> listItemColor = _context.ItemColors.Where(x => x.ItemId == Itemid).ToList();

            var query = (from m in _context.Items
                         join d in _context.ItemColors on m.ItemId equals d.ItemId
                         join dist in _context.Colors on d.ColorId equals dist.ColorId

                         where m.ItemId == Itemid
                         select new BillModel
                         {
                             Colorid= (int)dist.ColorId,
                             Colorname=dist.ColorName,
                             
                             isEnabel= d.isEnabled
                         }




                         ) ;



            var res = query.ToList();

            // return Json(listItemColor);
            return Json(res);

        }


        public IActionResult ItemColor(int ItemColorId)
        {
            var list = _context.ItemColors.Where(id => id.ItemId == ItemColorId).Include(o=>o.Color).Include(s=>s.Item).ToList();

            return View(list);
        }

        public async Task<JsonResult> color(int Iid)
        {
            var color = _context.ItemColors.Where(id => id.ItemColorId == Iid).FirstOrDefault();
            color.isEnabled = !color.isEnabled;
            _context.Update(color);
            _context.SaveChanges();
            return Json(new { Iid });
        }


        public IActionResult UpdatePrice(int ColorId, int itemId, float NewPrice)
        {

            var item = _context.ItemColors.Where(i => i.ColorId == ColorId && i.ItemId == itemId).FirstOrDefault();
            item.SpecialPrice = NewPrice;

            _context.Update(item);
            _context.SaveChanges();
            return Json(item.SpecialPrice);
        }
    }
}
