
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Nobels.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace Nobels.Controllers
{
    //مديرمشروع..مدير معرض
    [Authorize]
    public class showRoomAndProjectManger : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;


        public showRoomAndProjectManger(db_a8d242_exadecor2Context context, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;
        }
        //*


        public IActionResult GetRoom(int q)
        {
            // var order = _context.OrderDetails.Find(orderid);
            // int Item_colore = (int)order.ItemId;
            var rooms = _context.QutationRoom.Where(c => c.qutationId == q);


            return Json(rooms);
        }


        //*
        [HttpGet]
        public async Task<IActionResult> addneworder(int ItemId, int QoutationId, float Quantity, float Price, int roomId, int colorname, int MainItemId)
        {
            OrderDetail orderDetail = new OrderDetail();

            orderDetail.Notes = "لا يوجد";

            orderDetail.ItemId = ItemId;
            orderDetail.MainItemId = MainItemId;
            orderDetail.Price = Price;
            orderDetail.Quantity = Quantity;
            orderDetail.DiscountType = "نسبة";
            orderDetail.Discount = 0;
            orderDetail.QoutationId = QoutationId;
            var c = (Price * Quantity) * (orderDetail.Discount / 100);
            orderDetail.TotalValue = (Price * Quantity) - c;

            if ((colorname == 0) || (colorname == 0))
            {
                orderDetail.colorname = "NOCOLOR";
            }
            else
            {
                int selectedColor = Convert.ToInt32(colorname);

                var color = _context.Colors.Where(c => c.ColorId == selectedColor).FirstOrDefault();
                orderDetail.colorname = color.ColorName;
            }
            orderDetail.roomId = roomId;
            QoutationUpdate update = new QoutationUpdate();
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            update.ChangeDate = DateTime.Now;
            update.QoutationId = QoutationId;
            update.EmployeeId = Int32.Parse(userId);
            var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
            update.Updates = "تم أضافة منتج بتاريخ: " + " " + DateTime.Now + " " + "من قبل" + employee.UserName
               ;
            _context.Add(update);
            _context.SaveChanges();
            _context.Add(orderDetail);
            _context.SaveChanges();


            return Json(orderDetail);

        }

        //*
        public IActionResult CreateOrderDetail(int qid)
        {
            //  var orderDetail = _context.OrderDetails.Where(order => order.QoutationId == qid);
            //ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId");
            //ViewData["QoutationId"] = new SelectList(_context.Quotations, "QuotationId", "QuotationId");
            ViewData["itemMain"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");
            //  ViewData["room"] = new SelectList(_context.QutationRoom.Where(r=>r.qutationId==qid), "id", "roomName");
            ViewData["MainItem"] = new SelectList(_context.MainItem, "Id", "MainItemName");
            // ViewData["color"] = new SelectList(_context.ItemColors.Where(c => c.isEnabled == true && c.ItemId == orderDetail.).Include(c1 => c1.Color), "ColorId", "Color.ColorName");
            return View();
        }


        //*
        // POST: OrderDetails/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrderDetail([Bind("DetailId,ItemId,QoutationId,Quantity,Notes,Price,Discount,TotalValue,DiscountType,Type,roomId,colorname,MainItemId")] OrderDetail orderDetail)
        {

            if (ModelState.IsValid)
            {
                orderDetail.Notes = "لا يوجد";
                // orderDetail.Type = "....";
                // orderDetail.colorname = colorname;
                if (orderDetail.DiscountType == "0")
                {
                    orderDetail.DiscountType = "نسبة";
                    var c = (orderDetail.Price * orderDetail.Quantity) * (orderDetail.Discount / 100);
                    orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - c;
                }
                else
                {
                    orderDetail.DiscountType = "قيمة";
                    orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - orderDetail.Discount;
                }
                if ((orderDetail.colorname == "الرجاء اختيار لون المنتج") || (orderDetail.colorname == "الرجاء اختيار اللون"))
                {
                    orderDetail.colorname = "NOCOLOR";
                }
                else
                {
                    int selectedColor = Convert.ToInt32(orderDetail.colorname);

                    var color = _context.Colors.Where(c => c.ColorId == selectedColor).FirstOrDefault();
                    orderDetail.colorname = color.ColorName;
                }

                QoutationUpdate update = new QoutationUpdate();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                update.ChangeDate = DateTime.Now;
                update.QoutationId = orderDetail.QoutationId;
                update.EmployeeId = Int32.Parse(userId);
                var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                update.Updates = "تم أضافة منتج بتاريخ: " + " " + DateTime.Now + " " + "من قبل" + employee.UserName
                   ;
                _context.Add(update);
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();


                return View(orderDetail);
                // return RedirectToAction("OrderDetails", "AddCustomer", new { id = orderDetail.QoutationId });
            }
            //ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", orderDetail.ItemId);
            // ViewData["QoutationId"] = new SelectList(_context.Quotations, "QuotationId", "QuotationId", orderDetail.QoutationId);

            ViewData["itemMain"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");
            // ViewData["color"] = new SelectList(_context.ItemColors.Where(c => c.isEnabled == true && c.ItemId == orderDetail.ItemId).Include(c1 => c1.Color), "ColorId", "Color.ColorName");
            return View(orderDetail);
        }


        //*
        public async Task<IActionResult> EditOrderDetail(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", orderDetail.ItemId);
            ViewData["QoutationId"] = new SelectList(_context.Quotations, "QuotationId", "QuotationId", orderDetail.QoutationId);
            ViewData["color"] = new SelectList(_context.ItemColors.Where(c => c.isEnabled == true && c.ItemId == orderDetail.ItemId), "ColorId", "ColorName");
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.userId = Int32.Parse(userId);
            return View(orderDetail);
        }
        //*
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderDetail(int id, [Bind("DetailId,ItemId,QoutationId,Quantity,Notes,Price,Discount,TotalValue,DiscountType,Type,colorname,roomId,MainItemId")] OrderDetail orderDetail)
        {
            int iii = (int)orderDetail.MainItemId;
            QoutationUpdate update = new QoutationUpdate();

            if (id != orderDetail.DetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    //Employee user = await userManager.GetUserAsync(HttpContext.User);
                    //var user2 = _context.Employees.Find(user.Id);
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    update.ChangeDate = DateTime.Now;
                    update.QoutationId = orderDetail.QoutationId;
                    update.EmployeeId = Int32.Parse(userId);
                    var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                    update.Updates = ":تم تعديل بتاريخ" + " " + "\n" +
                        "" + DateTime.Now + " " +
                       "لتصبح القيم:" + "\n" +
                       "الكمية:" + "\n" +
                       orderDetail.Quantity +
                       employee.UserName
                       ;



                    _context.Add(update);

                    if (orderDetail.DiscountType == "نسية")
                    {

                        var x = (orderDetail.Price * orderDetail.Quantity) * (orderDetail.Discount / 100);
                        orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - x;
                    }
                    else
                    {
                        orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - orderDetail.Discount;

                    }
                    // var colorid = _context.Colors.Find(orderDetail.colorname);
                    //string colorname11 = colorid.ColorName;
                    // orderDetail.colorname = colorname11;

                    _context.Update(orderDetail);
                    //Quotation order = orderDetail.Qoutation;
                    //order.QuotationStatus = "معدل";
                    //_context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.DetailId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("OrderDetails", "showRoomAndProjectManger", new { id = orderDetail.QoutationId });
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", orderDetail.ItemId);
            ViewData["QoutationId"] = new SelectList(_context.Quotations, "QuotationId", "QuotationId", orderDetail.QoutationId);
            ViewData["color"] = new SelectList(_context.ItemColors.Where(c => c.isEnabled == true), "ColorId", "ColorId");
            return View(orderDetail);
        }




        //*
        public IActionResult editApi(int orderid, float quantity, string colorname, int UserId)
        {
            var orderDetail = _context.OrderDetails.Find(orderid);
            QoutationUpdate update = new QoutationUpdate();



            if (ModelState.IsValid)
            {


                //Employee user = await userManager.GetUserAsync(HttpContext.User);
                //var user2 = _context.Employees.Find(user.Id);
                update.ChangeDate = DateTime.Now;
                update.QoutationId = orderDetail.QoutationId;
                update.EmployeeId = UserId;
                var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                update.Updates = ":تم تعديل بتاريخ" + " " + "\n" +
                    "" + DateTime.Now + " " +
                   "لتصبح القيم:" + "\n" +
                   "الكمية:" + "\n" +
                   orderDetail.Quantity + "من قبل" +
                   employee.UserName
                   ;



                _context.Add(update);

                if (orderDetail.DiscountType == "نسية")
                {

                    var x = (orderDetail.Price * orderDetail.Quantity) * (orderDetail.Discount / 100);
                    orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - x;
                }
                else
                {
                    orderDetail.TotalValue = (orderDetail.Price * orderDetail.Quantity) - orderDetail.Discount;

                }

                orderDetail.colorname = colorname;
                orderDetail.Quantity = quantity;
                _context.Update(orderDetail);

                _context.SaveChanges();


                // return RedirectToAction("OrderDetails", "AddCustomer", new { id = orderDetail.QoutationId });
                return Json(orderDetail);
            }

            //  return View(orderDetail);
            return Json("notUodated");
        }

        //*
        public IActionResult ItemsColor(int orderid)
        {
            var order = _context.OrderDetails.Find(orderid);
            int Item_colore = (int)order.ItemId;
            var coloritems = _context.ItemColors.Where(c => c.ItemId == Item_colore && c.isEnabled == true).Include(o => o.Color);


            return Json(coloritems);
        }
        //*
        public IActionResult ItemColorFromItem(int ItemColorId)
        {
            var list = _context.ItemColors.Where(id => id.ItemId == ItemColorId).Include(o => o.Color).Include(s => s.Item).ToList();

            return View(list);
    }
        //*


        public async Task<JsonResult> color(int Iid)
        {
            var color = _context.ItemColors.Where(id => id.ItemColorId == Iid).FirstOrDefault();
            color.isEnabled = !color.isEnabled;
            _context.Update(color);
            _context.SaveChanges();
            return Json(new { Iid });
        }


        //*
        public IActionResult UpdatePrice(int ColorId, int itemId, float NewPrice)
        {

            var item = _context.ItemColors.Where(i => i.ColorId == ColorId && i.ItemId == itemId).FirstOrDefault();
            item.SpecialPrice = NewPrice;

            _context.Update(item);
            _context.SaveChanges();
            return Json(item.SpecialPrice);
        }



        //*
        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> DeleteOrderDetail(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Item)
                .Include(o => o.Qoutation)
                .FirstOrDefaultAsync(m => m.DetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }
        //*
        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("DeleteOrderDetail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            QoutationUpdate update = new QoutationUpdate();
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'PricingDBContext.OrderDetails'  is null.");
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                update.ChangeDate = DateTime.Now;
                update.QoutationId = orderDetail.QoutationId;
                update.EmployeeId = Int32.Parse(userId);
                var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                update.Updates = "تم الحذف بتاريخ:" + " " + DateTime.Now + " " + "من قبل" + employee.UserName
                   ;



                _context.Add(update);
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("OrderDetails", "showRoomAndProjectManger", new { id = orderDetail.QoutationId });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.DetailId == id);
        }





        //*
        public async Task<JsonResult> AddRoomAsync(string name, string note, int qid, int discount, string discountType)

        {

            QutationRoom room = new QutationRoom();
            // room.notes = note;
            room.qutationId = qid;
            room.roomName = name;
            room.discount = discount;
            room.discountType = "نسبة";
            if (discountType == "0")
            { room.discountType = "نسبة"; }
            if (discountType == "1")
            {
                room.discountType = "قيمة";
            }
            if (note == "")
            {
                room.notes = "-";
            }
            else room.notes = note;


            await _context.QutationRoom.AddAsync(room);
            await _context.SaveChangesAsync();

            return Json("");





        }
        //*
        public async Task<IActionResult> CreateQuatationRoomsAsync(int id)
        {
            Quotation q = new Quotation();
            q.CustomerId = id;
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            q.BranchId = user.BranchId;
            q.employeeId = Int32.Parse(userId);

            q.QuotationStatus = "جديد";

            q.Code = "000";
            q.Notes = "no notes";
            q.QuotationSimpleDate = DateTime.Now;

            q.discount = 10;
            q.discountType = "نسبة";
            q.QuotationStatus = "جديد";

            await _context.Quotations.AddAsync(q);
            await _context.SaveChangesAsync();
            int a = (int)((q.BranchId * 100000000) + q.QuotationId);
            var qu = _context.Quotations.Find(q.QuotationId);
            qu.Code = a.ToString();
            _context.Update(qu);
            await _context.SaveChangesAsync();


            ViewBag.qid = q.QuotationId;

            return View();
        }



        //*
        public async Task<ActionResult> IndexAsync()
        {
            var cus = _context.Customers.Include(c => c.City ).OrderBy(c=>c.ArabicName).ToList();
            ViewBag.Employee = _context.Employees.ToArray();
            return View("Index", cus);


            //Employee user = await userManager.GetUserAsync(HttpContext.User);
            //int id= (int)user.BranchId;
            //var cus = _context.Customers.Where(c=>c.branchId==id).Include(c => c.City).ToList();
            // return View("Index", cus);
        }
        //*
        public ActionResult home()
        {
            bager();

            return View();
        }


        //*

        // GET: AddCustomer/Details/5
        public async Task<IActionResult> OrderDetailsAsync(int id)
        {
            var qu1 = _context.Quotations.Where(q => q.QuotationId == id).FirstOrDefault();
            ViewBag.qqid = qu1.CustomerId;
            ViewBag.qqqid = id;
            int stat = 0;
            if (qu1.QuotationStatus == "مقبول"||qu1.QuotationStatus == "مرفوض")
            {
                stat = 1;
                ViewBag.status = stat;
            }
            else
                ViewBag.status = 0;
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            int userId = (int)user.Id;
            ViewBag.CurrentUId = userId;
            ViewBag.QUserId = qu1.employeeId;




           


            var query = (from order in _context.OrderDetails
                         join qu in _context.Quotations on order.QoutationId equals id
                         join cus in _context.Customers on qu.CustomerId equals cus.Id
                         join item in _context.Items on order.ItemId equals item.ItemId
                         //join sub in _context.SubTypes on item.ItemSubTypeId equals sub.SubTypeId
                         //join itemType in _context.ItemTypes on sub.TypeId equals itemType.TypeId
                         //join mainitem in _context.MainItem on itemType.MainItemId equals mainitem.Id
                        // join mainitem in _context.MainItem on item.MainItemId equals mainitem.Id
                         join mainitem in _context.MainItem on order.MainItemId equals mainitem.Id

                         join Branch in _context.Branches on qu.BranchId equals Branch.BranchId
                         join room in _context.QutationRoom on order.roomId equals room.id

                         where qu.QuotationId == id
                         select new BillModel
                         {

                             itemname = item.ItemArName,
                             quationId = qu.QuotationId,
                             Unitprice = (double)order.Price,
                             quantity = (int)order.Quantity,
                             percentType = order.DiscountType,
                             discountValue = order.Discount,
                             totalValue = order.TotalValue,
                             orderId = order.DetailId,
                             Customer = cus.ArabicName,
                             Colorname = order.colorname,
                             branch = Branch.BranchName,
                             roomId = room.id,
                             roomName = room.roomName,

                             MainItemName=mainitem.MainItemName,
                             status=qu.QuotationStatus


                         });



            var res = query.ToList();
            return View(res);
        }



        //*
        // GET: AddCustomer/Create

        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: AddCustomer/Create
        //*
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                Employee user = await userManager.GetUserAsync(HttpContext.User);

                customer.branchId = (int)user.BranchId;
                customer.Phone = "05" + customer.Phone;
                customer.PhonNumber = "05" + customer.PhonNumber;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        // GET: AddCustomer/Edit/5
        //*
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        // POST: AddCustomer/Edit/5
        //*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        //*
        public async Task<IActionResult> DeleteQU(int? id)
        {
            if (id == null || _context.Quotations == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Customer).Include(q => q.Branch)
                .FirstOrDefaultAsync(m => m.QuotationId == id);
            if (quotation == null)
            {
                return NotFound();
            }

            return View(quotation);
        }
        //*
        // POST: Quotations/Delete/5
        [HttpPost, ActionName("DeleteQU")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQU(int id)
        {
            if (_context.Quotations == null)
            {
                return Problem("Entity set 'PricingDBContext.Quotations'  is null.");
            }
            var quotation = await _context.Quotations.FindAsync(id);
            var cusromerID = quotation.CustomerId;
            var quotationRooms = _context.QutationRoom.Where(r => r.qutationId == quotation.QuotationId).ToList();
            if (quotationRooms.Any())
            {
                _context.QutationRoom.RemoveRange(quotationRooms);
                await _context.SaveChangesAsync();
            }
            if (quotation != null)
            {
                _context.Quotations.Remove(quotation);
            }

            await _context.SaveChangesAsync();
            // return RedirectToAction(nameof(QuatationIndex));

            return RedirectToAction("QuatationIndex", "showRoomAndProjectManger", new { id = cusromerID });
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
        //*
        public async Task<ActionResult> QuatationIndex(int id)
        {
            ViewBag.customerid = id;


            Employee user = await userManager.GetUserAsync(HttpContext.User);
            int userId = (int)user.Id;
            ViewBag.uid = userId;
            var roles = await userManager.GetRolesAsync(user);

            

         
                int idc = (int)user.BranchId;
                var customer = _context.Customers.Find(id);
                ViewBag.Employee = _context.Employees.ToArray();
                var qu = _context.Quotations.Where(qu => qu.CustomerId == id && qu.BranchId == idc).Include(qu => qu.Branch).Include(qu => qu.Customer).ToList();
                return View("QuatationIndex", qu);

         




        }

        //*
        public IActionResult AddItem(int id)
        {
            try
            {
                var existingQutaion = _context.Quotations.Find(id);
                ViewBag.qid = existingQutaion.QuotationId;

                ViewData["itemMain"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");
                ViewData["MainItem"] = new SelectList(_context.MainItem, "Id", "MainItemName");
                ViewData["room"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId == id), "id", "roomName");


                return View(/*subTypeobj*/);

            }
            catch (Exception ex)
            {
                return null;
            }

        }

     


        //*
        public IActionResult GetCustomername(int id)
        {
            var customerid = _context.Quotations.Find(id);
            var customer = _context.Customers.Find(customerid.CustomerId);

            String name = customer.ArabicName;

            return Json(name);
        }
        //*
        public IActionResult GetPrice(int color, int item)
        {

            var ColorPrice = _context.ItemColors.Where(i => i.ColorId == color && i.ItemId == item).FirstOrDefault();

            //double price = ColorPrice.SpecialPrice;
            if (ColorPrice != null)

            { return Json(ColorPrice.SpecialPrice); }
            else
            {
                var ItemPrice = _context.Items.Where(i => i.ItemId == item).FirstOrDefault();
                return Json(ItemPrice.ItemCurrentPrice);
            }
        }
        //*
        public JsonResult Getsub(int cid)
        {
            List<SubType> listSub = _context.SubTypes.Where(x => x.TypeId == cid).ToList();

            return Json(listSub);
        }
        //*
        public JsonResult getTypes(int id)
        {
            //List<ItemType> listSub = _context.ItemTypes.Where(x => x.MainItemId == id).ToList();
            List<ItemType> listSub = _context.ItemTypes.ToList();

            return Json(listSub);
        }
        //*
        public JsonResult GetItem(int subid)
        {
            List<Item> listItem = _context.Items.Where(x => x.ItemSubTypeId == subid).ToList();

            return Json(listItem);
        }

        public JsonResult GetItemColor(int ItemTypeid)
        {

            {
              

                var query = (from m in _context.Colors

                             where m.itemTypeId1 == ItemTypeid
                             select new Color
                             {
                                 ColorId = (int)m.ColorId,
                                 ColorName = m.ColorName


                             }




                             );

                //var colorlist = _context.Colors.Where(x => x.itemTypeId1 == ItemTypeid).ToList();

                var res = query.ToList();

                return Json(res);


            }
        }
        //*
        public Models.Color[] GetItemColorById(int ItemId)
        {
            ItemColor[] ics = _context.ItemColors.Where(c => c.ItemId == ItemId && c.isEnabled == true).ToArray();
            List<Color> colors = new List<Color>();
            for (int i = 0; i < ics.Length; i++)
            {
                colors.Add(_context.Colors.Find(ics[i].ColorId));
            }
            return colors.ToArray();
        }
        //*
        public void bager()
        {
            ViewBag.count = _context.Items.Count();
            ViewBag.count1 = _context.ItemTypes.Count();
            ViewBag.count2 = _context.Customers.Count();
            ViewBag.count3= _context.MainItem.Count();
            //ViewBag.count3 = _context.Appointments.Count();


        }



        
        //*
        public ActionResult BillView(int id)
        {
            var q = _context.Quotations.Where(i => i.QuotationId == id).FirstOrDefault();
            int sts = 0;
            if (q.QuotationStatus == "جديد")
            {
                sts = 1;
                ViewBag.status = 1;
            }
            //ViewBag.date = q.BillDate;
            var customer = _context.Customers.Where(c => c.Id == q.CustomerId).FirstOrDefault();
            ViewBag.Customername = customer.ArabicName;

            ViewBag.code = q.QuotationId;
            ViewBag.phone = customer.Phone;
            ViewBag.Gender = customer.Gender;
            var Branch = _context.Branches.Find(q.BranchId);
            ViewBag.branch = Branch.BranchName;
            var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            ViewBag.tax = tax.tax;
            ViewBag.addr=Branch.Address;
            var query = (


                //from bill in _context.Bills
                from qu in _context.Quotations
                //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
                join oreder in _context.OrderDetails on qu.QuotationId equals oreder.QoutationId
                join item in _context.Items on oreder.ItemId equals item.ItemId
                //join sub in _context.SubTypes on item.ItemSubTypeId equals sub.SubTypeId
                //join itemType in _context.ItemTypes on sub.TypeId equals itemType.TypeId
                //join mainitem in _context.MainItem on itemType.MainItemId equals mainitem.Id
               // join mainitem in _context.MainItem on item.MainItemId equals mainitem.Id
                join mainitem in _context.MainItem on oreder.MainItemId equals mainitem.Id


                join room in _context.QutationRoom on oreder.roomId equals room.id

                where qu.QuotationId == id

                select new
                         BillModel
                {
                    //code = "123",
                    itemname = item.ItemArName,
                    //unit = "unit",
                    Unitprice = (double)oreder.Price,
                    quantity = oreder.Quantity,
                    //discountValue = (double)oreder.Discount,
                    //percentType = oreder.DiscountType,
                    //phone = customer.Phone,
                    totalValue = (double)oreder.TotalValue,
                    quationId = (int)qu.QuotationId,
                    MainItemName=mainitem.MainItemName,
                    //Customer = customer.ArabicName,
                    //gender = customer.Gender,

                    //branch = Branch.BranchName,
                    //BillDate = bill.BillDate,
                    roomId = room.id,
                    roomName = room.roomName,
                    Colorname = oreder.colorname





                });




            //var query = (


            //    from bill in _context.Bills
            //    join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
            //    join oreder in _context.OrderDetails on qu.QuotationId equals oreder.QoutationId
            //    join item in _context.Items on oreder.ItemId equals item.ItemId
            //    join customer in _context.Customers on qu.CustomerId equals customer.Id
            //    join Branch in _context.Branches on qu.BranchId equals Branch.BranchId
            //    join room in _context.QutationRoom on oreder.roomId equals room.id

            //    where qu.QuotationId == id

            //    select new
            //             BillModel
            //    {
            //        code = "123",
            //        itemname = item.ItemArName,
            //        unit = "unit",
            //        Unitprice = (double)oreder.Price,
            //        quantity = (int)oreder.Quantity,
            //        discountValue = (double)oreder.Discount,
            //        percentType = oreder.DiscountType,
            //        phone = customer.Phone,
            //        totalValue = (double)oreder.TotalValue,
            //        quationId = (int)qu.QuotationId,
            //        Customer = customer.ArabicName,
            //        gender = customer.Gender,

            //        branch = Branch.BranchName,
            //        BillDate = bill.BillDate,
            //        roomId = room.id,
            //        roomName = room.roomName,
            //        Colorname = oreder.colorname





            //    });




            //foreach (var item in query)
            //{
            //    ViewBag.date = item.BillDate;
            //    ViewBag.cutom = item.Customer;
            //    ViewBag.branch = item.branch;
            //    ViewBag.code = item.code;
            //    ViewBag.phone = item.phone;
            //    ViewBag.Gender = item.gender;
            //}
            //var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            //ViewBag.tax = tax.tax;

            var res = query.OrderBy(i => i.roomName).ToList();
            ////ViewBag.count = query.Count();

            return View(res);





        }


       
        //*
        public JsonResult GetQutationUpdate(int qid, string itemName, float price, float quantity, float discount, string type, float total, string roomId, string color,int mainItemId)

        {
            var item = _context.Items.Where(i => i.ItemArName == itemName).FirstOrDefault();
           // var mainItem = _context.MainItem.Where(i => i.MainItemName == mainItemId).FirstOrDefault();
            var room = _context.QutationRoom.Where(i => i.roomName == roomId && i.qutationId == qid).FirstOrDefault();
            var id = qid;
            var quotation = _context.Quotations.Find(id);
            OrderDetail[] olddetail = _context.OrderDetails.Where(c => (c.QoutationId == id) && (c.roomId == room.id) && (c.ItemId == item.ItemId)).ToArray();
            if (olddetail.Length > 0)
            {
                Console.WriteLine(olddetail.Length);
                _context.OrderDetails.Remove(olddetail[0]);
                _context.SaveChanges();
            }


            OrderDetail detail = new OrderDetail();
            detail.QoutationId = quotation.QuotationId;
            detail.ItemId = item.ItemId;
            detail.Price = price;
            detail.Quantity = quantity;
            detail.Notes = "لا يوجد";
            detail.Discount = discount;
            detail.DiscountType = type;
            detail.TotalValue = total;
            detail.roomId = room.id;
            detail.MainItemId = mainItemId;
            if ((detail.colorname == "الافتراضي") || (detail.colorname == "الرجاء اختيار اللون") || (detail.colorname == "الرجاء اختيار لون المنتج"))
            {
                detail.colorname = "NOCOLOR";
            }
            else
            {
                detail.colorname = color;

            }
            _context.OrderDetails.Add(detail);


            _context.SaveChanges();
            return Json(new { result = detail });




        }
        //*
        public JsonResult RoomDiscount(int roomId)
        {
            var room = _context.QutationRoom.Find(roomId);
            string type = room.discountType;
            int value = (int)room.discount;
            return Json(new { type, value });
        }

      
        //*
        public JsonResult GetTax()
        {
            var tax = _context.Settings.Where(i => i.id == 1).FirstOrDefault();
            return Json(tax.tax);
        }

        //*
        public async Task<JsonResult> GetBranchNameAsync()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branch = await _context.Branches.FindAsync(user.BranchId);
            var BranchName = branch.BranchName;


            return Json(BranchName);
        }
        //*
        public async Task<JsonResult> GetCityNameAsync()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branch = await _context.Branches.FindAsync(user.BranchId);
            var city = await _context.Cities.FindAsync(branch.CityId);
            var cityName = city.CityName;

            return Json(cityName);
        }
        //*
        public IActionResult ShowQuotationUpdate(int quotationId)
        {
            //var update = _context.QoutationUpdates.Find(quotationId);
            var note = _context.QoutationUpdates.Where(q => q.QoutationId == quotationId);
            return Json(note.ToList());
        }
        //*
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }




        


        //*
        public IActionResult RoomBillView(int id)
        {

            ViewData["room"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId == id), "id", "roomName");

            // ViewData["BillRoom"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId== id), "id", "roomName ");
            var qu = _context.Quotations.Find(id);
            int sts = 0;
            if (qu.QuotationStatus == "جديد")
            {
                sts = 1;
                ViewBag.status = 1;
            }
            ViewBag.Qnumber = qu.QuotationId;
            var customerId = _context.Customers.Find(qu.CustomerId);
            ViewBag.Customername = customerId.ArabicName;
            ViewBag.CustomerPhone = customerId.Phone;
            var Branch = _context.Branches.Find(qu.BranchId);
            ViewBag.BranchName = Branch.BranchName;
            ViewBag.addr = Branch.Address;

            var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            ViewBag.tax = tax.tax;

            return View();

        }
        //*
        public IActionResult RoomBill(int id)
        {

            var sub = from od in _context.OrderDetails
                      where od.QoutationId == id
                      group od by new {od.roomId,od.MainItemId}
                       into v
                      select new
                      {
                          mainId = v.Key.MainItemId,
                          roomid=v.Key.roomId,
                          value = v.Sum(x => x.TotalValue)
                          
                      };
            var y = sub.ToList();
            var query = from mainitem in _context.MainItem
                        join vs in sub on mainitem.Id equals vs.mainId
                        join room in _context.QutationRoom on vs.roomid equals room.id
                       // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            roomname = room.roomName,
                            id = vs.mainId,
                            total = vs.value,
                            mainitem=mainitem.MainItemName
                        }
                        ;
            var x = query.ToList();





          

            return Json(query);
          

        }
        //*

        public IActionResult BranchReportView()
        {

            //  ViewData["Branch"] = new SelectList(_context.Branches, "BranchId", "BranchName");

            return View();

        }

       

        //*
        public IActionResult CustomerReportView()
        {

            ViewData["Customer"] = new SelectList(_context.Customers, "Id", "ArabicName");

            return View();

        }



      


       

       
        //*
        public IActionResult AllSelesReportView()
        {
            return View();
        }

       

        //*
        public IActionResult RoomDetaildView(int id)
        {
            try
            {
                ViewData["room"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId == id), "id", "roomName");

                // ViewData["BillRoom"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId== id), "id", "roomName ");
                var qu = _context.Quotations.Find(id);
                int sts = 0;
                if (qu.QuotationStatus == "جديد")
                {
                    sts = 1;
                    ViewBag.status = 1;
                }
                else
                    ViewBag.status = 0;
                ViewBag.Qnumber = qu.QuotationId;
                var customerId = _context.Customers.Find(qu.CustomerId);
                ViewBag.Customername = customerId.ArabicName;
                ViewBag.CustomerPhone = customerId.Phone;
                var Branch = _context.Branches.Find(qu.BranchId);
                ViewBag.BranchName = Branch.BranchName;
                ViewBag.addr = Branch.Address;
                var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
                ViewBag.tax = tax.tax;
            }
            catch
            {
                ViewBag.status = 0;
            }
                return View();

        }
        //*
        public IActionResult RoomDetaildBill(int id)
        {

            // ViewData["BillRoom"] = new SelectList(_context.QutationRoom.Where(q=>q.qutationId==id), "id", "roomName");
            var query = (


               from qu in _context.Quotations
               //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
               join oreder in _context.OrderDetails on qu.QuotationId equals oreder.QoutationId
               join item in _context.Items on oreder.ItemId equals item.ItemId
               join customer in _context.Customers on qu.CustomerId equals customer.Id
               join Branch in _context.Branches on qu.BranchId equals Branch.BranchId
               join room in _context.QutationRoom on oreder.roomId equals room.id

               where room.id == id

               select new BillModel
               {
                   code = qu.Code == null ? "0" : qu.Code,
                   itemname = item.ItemArName,
                   unit = "unit",
                   Unitprice = (double)oreder.Price,
                   quantity = (int)oreder.Quantity,
                   discountValue = (double)oreder.Discount,
                   percentType = oreder.DiscountType,
                   phone = customer.Phone,
                   totalValue = (double)oreder.TotalValue,

                   quationId = (int)qu.QuotationId,
                   Customer = customer.ArabicName,
                   gender = customer.Gender,

                   branch = Branch.BranchName,
                //   BillDate = bill.BillDate,
                   roomId = room.id,
                   roomName = room.roomName,
                   Colorname = oreder.colorname





               });


            var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            ViewBag.tax = tax.tax;

            var res = query.OrderBy(i => i.roomName).ToList();
            //ViewBag.count = query.Count();

            return Json(res);


        }

        //*
        public async Task<IActionResult> userprofile(string? name)
        {
            if (name == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Where(e => e.UserName == name).FirstOrDefaultAsync();
            string phone = employee.Phone;

            string str = "+05";
            if (phone.StartsWith(str))
            {
                phone = phone.Substring(3);
                ViewBag.phone = phone;
            }


            if (employee == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.EmployeeRoles, "RoleId", "RoleName");

            
            return View(employee);

        }
        //*
        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> userprofile(int id, [Bind("Id,EmployeeNumber,BranchId,RoleId,FirstName,LastName,Phone,Email,UserName,Password")] Employee employee)
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
                    user.FirstName = employee.FirstName;
                    user.LastName = employee.LastName;
                    user.Phone = "+05" + employee.Phone;
                    user.UserName = employee.UserName;
                    user.NormalizedUserName = employee.UserName.ToUpper();
                    user.Email = employee.Email;
                    user.NormalizedEmail = employee.Email.ToUpper();
                    user.PasswordHash = _passwordHasher.HashPassword(user, employee.Password);
                    user.RoleId = employee.RoleId;


                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    var dbrole = _context.EmployeeRoles.Where(rr => rr.RoleId == user.RoleId).FirstOrDefault();
                    var role = _context.Roles.FirstOrDefault(r => r.Name == dbrole.RoleName);

                    var updatedRole = _context.UserRoles.Where(rr => rr.UserId == user.Id).FirstOrDefault();
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
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        //*
        public async Task<IActionResult> chart2Api()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);


            var sub = from od in _context.OrderDetails
                      join q in _context.Quotations on od.QoutationId equals q.QuotationId
                      where q.BranchId == user.BranchId
                      group new { q, od } by new { q.BranchId, q.QuotationDate }
                          into v
                      select new
                      {

                          bId = v.Key.BranchId,

                          value = v.Sum(x => x.od.TotalValue),
                          d = v.Key.QuotationDate.Value.ToShortDateString(),

                      };
            var y = sub.ToList();
            var query = from branch in _context.Branches
                        join vs in sub on branch.BranchId equals vs.bId

                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            branch = branch.BranchName,
                            total = vs.value,
                            date = vs.d,

                        }
                        ;
            var x = query.ToArray();



            //List<object> objs = new List<object>();
            //objs.Add(new[] { "branch", "total" });
            //foreach (var item in x)
            //{

            //    objs.Add(new[] { item.branch,item.total});
            //}


            return Json(y);

        }

        //*
        public async Task<IActionResult> chartTopCustomersApi()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);


            var sub = from od in _context.OrderDetails
                      join q in _context.Quotations on od.QoutationId equals q.QuotationId
                      where q.BranchId == user.BranchId
                      group new { q, od } by new { q.BranchId, q.CustomerId }
                          into v
                      select new
                      {

                          bId = v.Key.BranchId,

                          value = v.Sum(x => x.od.TotalValue),
                          name = v.Key.CustomerId,

                      };
            var y = sub.ToList();
            var query = from customer in _context.Customers
                        join vs in sub on customer.Id equals vs.name

                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            name = customer.ArabicName,
                            total = vs.value,


                        }
                        ;
            var x = query.ToArray();






            return Json(query);

        }

    }
}
