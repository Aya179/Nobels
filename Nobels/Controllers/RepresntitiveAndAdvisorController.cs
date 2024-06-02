using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nobels.Models;
using System.Drawing;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Color = Nobels.Models.Color;

namespace Nobels.Controllers
{
    //استشاري مبيعات...مندوب مشاريع
    [Authorize]
    public class RepresntitiveAndAdvisorController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;


        public RepresntitiveAndAdvisorController(db_a8d242_exadecor2Context context, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> updateQuotationAddAddressAsync(string neighborhood,string street,string houseNumber,int city,int cid/*,int qid*/)
        {
            Quotation q = new Quotation();
            q.CustomerId = cid;
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

            _context.Quotations.Add(q);
           _context.SaveChanges();
            Address address = new Address();
            address.cityid=city;
            address.houseNumber=houseNumber;
            address.neighborhood=neighborhood;
            address.street=street;
            _context.Add(address);
            _context.SaveChanges();
            int a = (int)((q.BranchId * 100000000) + q.QuotationId);
            var qu = _context.Quotations.Find(q.QuotationId);
            qu.Code = a.ToString();
            qu.addressId = address.id;
            _context.Update(qu);
             _context.SaveChanges();
           
         
            return Json(qu.QuotationId);
            //return Json(raws);
        }
        public IActionResult updateQoutationStataus(int qid)
        {
            var qu = _context.Quotations.Find(qid);
            if(qu.QuotationStatus== "جديد"||qu.QuotationStatus== "مرفوض فنياً")
            {
                qu.QuotationStatus = "قيد التدقيق الفني";
            }
            else if(qu.QuotationStatus== "مقبول فنياً"||qu.QuotationStatus== "مرفوض مالياً")
            {
                qu.QuotationStatus = "قيد التدقيق المالي";
            }
            _context.Update(qu);
            QoutationUpdate qoutationUpdate = new QoutationUpdate();
            qoutationUpdate.QoutationId = qid;
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            qoutationUpdate.ChangeDate = DateTime.Now;

            qoutationUpdate.EmployeeId = Int32.Parse(userId);
            var employee = _context.Employees.FirstOrDefault(x => x.Id == qoutationUpdate.EmployeeId);
            qoutationUpdate.Updates = "تم تعديل حالة العرض بتاريخ: " + " " + DateTime.Now + " " + "لتصبح " + qu.QuotationStatus;
            _context.Add(qoutationUpdate);
            _context.SaveChanges();
            return Ok(qu);
        }

        public IActionResult cover_arabic(int id)
        {
            ViewData["room"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId == id), "id", "roomName");
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName");


            customerSelectModel c = new customerSelectModel();
            var qu = _context.Quotations.Include(c=>c.Customer).Where(q=>q.QuotationId==id).First();
            
            c.qu= qu;
            var raws = _context.RawMaterials.Include(r=>r.RoeMaterialColors).ToList();
            c.rawMaterials = raws;
            var rawcolor = _context.RoeMaterialColors.Include(r => r.Color).ToList();
            c.roeMaterialColors = rawcolor;
            ViewBag.customerId = qu.CustomerId;
            return View(c);
           //return Json(raws);
        }
          
        public async Task<IActionResult> getItemData(int id)
        {
            var item = _context.Items.Find(id);
         
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var price=0;
             var branchId = (int)user.BranchId;
            var branch = _context.Branches.Find(branchId);
            var branchname = branch.BranchName;
            var Itemprice = _context.ItemsPrices.Where(p => p.ItemName == item.ItemArName && p.BranchName == branchname).FirstOrDefault();
            if (Itemprice == null)
            {

                price = 0;
            }
            else
            {
               price = (int)Itemprice.price;
            }
            
            return Json(new { item, price });
        }
      
        //*


        public IActionResult GetRoom(int q)
        {
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
        public IActionResult CreateOrderDetail(int id)
        {
            ViewData["room"] = new SelectList(_context.QutationRoom.Where(q => q.qutationId == id), "id", "roomName");
            ViewData["ItemSubTypeId"] = new SelectList(_context.SubTypes, "SubTypeId", "SubTypeEnName");
        
            customerSelectModel c = new customerSelectModel();
            var qu = _context.Quotations.Include(c => c.Customer).Where(q => q.QuotationId == id).First();

            c.qu = qu;
            var raws = _context.RawMaterials.Include(r => r.RoeMaterialColors).ToList();
            c.rawMaterials = raws;
            var rawcolor = _context.RoeMaterialColors.Include(r => r.Color).ToList();
            c.roeMaterialColors = rawcolor;
            ViewBag.qid = qu.QuotationId;
            ViewBag.customerId = qu.CustomerId;
            return View(c);
        }

        public IActionResult getAllquRoms(int id)
        {
           return Json(_context.QutationRoom.Where(r => r.qutationId == id).ToList());
        }
        public IActionResult deleteRoom(int id)
        {
            var room = _context.QutationRoom.Find(id);
           
            var orderdes = _context.OrderDetails.Where(r => r.QoutationId ==room.id ).ToList();

            if (orderdes.Any())
            {
                _context.OrderDetails.RemoveRange(orderdes);
                
            }
            _context.QutationRoom.Remove(room);
            _context.SaveChanges();
           return Json("done");
        }
        public IActionResult editeRoomName(int roomId, string roomname)
        {
            var room = _context.QutationRoom.Find(roomId);

            room.roomName = roomname;
            
            _context.QutationRoom.Update(room);
            _context.SaveChanges();
           return Json("done");
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
            }
           
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
                return RedirectToAction("OrderDetails", "RepresntitiveAndAdvisor", new { id = orderDetail.QoutationId });
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId", orderDetail.ItemId);
            ViewData["QoutationId"] = new SelectList(_context.Quotations, "QuotationId", "QuotationId", orderDetail.QoutationId);
            ViewData["color"] = new SelectList(_context.ItemColors.Where(c => c.isEnabled == true), "ColorId", "ColorId");
            return View(orderDetail);
        }




        //*
        public IActionResult editApi(int orderid, float quantity, string colorname,string colorname1,string colorname2 ,int UserId)
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
                orderDetail.colorname1 = colorname1;
                orderDetail.colorname2 = colorname2;
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
            return RedirectToAction("OrderDetails", "RepresntitiveAndAdvisor", new { id = orderDetail.QoutationId });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.DetailId == id);
        }





        //*
        public async Task<JsonResult> AddRoomAsync(string name, string note, int qid, int discount, string discountType,int qutationid)

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
            Customer cu = _context.Customers.Find(id);
        
            
            Address address = _context.Addresses.Find(cu.adressId);
            ViewBag.neighborhood = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.houseNumber = address.houseNumber;
            ViewBag.customerId = cu.Id;
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");


            return View();
        }



        //*
        public async Task<ActionResult> IndexAsync()
        {
            var query = from Customers in _context.Customers
                        join add in _context.Addresses on Customers.adressId equals add.id
                        join city in _context.Cities on Customers.CityId equals city.CityId





                        select new BillModel
                        {
                            ArabicName =Customers.ArabicName,
            EnglishName =Customers.EnglishName,
            street =add.street,
            neighborhood =add.neighborhood,
            houseNumber =add.houseNumber,
            CityName =city.CityName,
                Phone1=Customers.Phone,
                customerid=Customers.Id,
                EmployeeId=Customers.EmployeeId

                        }
                        ;



            var cus = _context.Customers.Include(c => c.City).OrderBy(c => c.ArabicName).ToList();
            ViewBag.Employee = _context.Employees.ToArray();
            return View("Index", query.ToList());


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
            if (qu1.QuotationStatus == "مقبول مالياً" || qu1.QuotationStatus == "مقبول فنياً" || qu1.QuotationStatus == "مرفوض"|| qu1.QuotationStatus == "قيد التدقيق المالي")
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

                             MainItemName = mainitem.MainItemName,
                             status = qu.QuotationStatus


                         });



            var res = query.ToList();
            return View(res);
        }



        //*

        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        //*
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer,string houseNumber,string street,string neighborhood)
        {
            if (ModelState.IsValid)
            {
                Employee user = await userManager.GetUserAsync(HttpContext.User);

                customer.branchId = (int)user.BranchId;
                customer.EmployeeId = user.Id;
                customer.Phone = "05" + customer.Phone;
                customer.PhonNumber = "05" + customer.PhonNumber;
                Address address = new Address();
                address.cityid = customer.CityId;
                address.houseNumber = houseNumber;
                address.neighborhood = neighborhood;
                address.street = street;
                //int oldID = 0;
                //oldID = IsPhoneExist(customer.Phone);
                //if (oldID > 0)
                //{
                //  ViewBag.Message = "عدم قبول إضافة العميل لوجود رقم الهاتف مسبقاً";
                //    //TempData["Alert"] = $"The phone number '{customer.Phone}' already exists.";
                //  return RedirectToAction("Edit", new { Id = oldID });
                //  //return View(customer);
                    
                //}
                //else
                //{
                    _context.Add(address);
                    _context.SaveChanges();
                    customer.adressId = address.id;
                    customer = _context.Customers.Add(customer).Entity;
                    _context.SaveChanges();
                    return RedirectToAction("Details", new { Id = customer.Id });
                //}
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        public IActionResult IsPhoneExist(string phone)
        {
            Customer[] cs = _context.Customers.Where(c => c.Phone == phone).ToArray();
            if (cs.Length > 0)
                // return cs[0].Id;
                return Json(cs[0].Id);
            else
                //  return 0;
                return Json("add");
        }

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
            ViewBag.add = _context.Addresses.Find(customer.adressId);
            return View(customer);
        }

        // POST: AddCustomer/Edit/5
        //*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes,adressId")] Customer customer, string houseNumber, string street, string neighborhood)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }
            var cusid = _context.Customers.Where(v=>v.Id==id).First();
            var addr = _context.Addresses.Where(v => v.id == cusid.adressId).First();

            if (ModelState.IsValid)
            {
                try
                {
                    addr.houseNumber = houseNumber;
                    addr.neighborhood = neighborhood;
                    addr.street = street;
                    addr.cityid = customer.CityId;
                    _context.Update(addr);
                    cusid.ArabicName= customer.ArabicName;
                    cusid.EnglishName= customer.EnglishName;
                    cusid.Email= customer.Email;
                    cusid.Phone= customer.Phone;
                    cusid.PhonNumber= customer.PhonNumber;
                    cusid.Address= customer.Address;
                    cusid.SecondAddress= customer.SecondAddress;
                    cusid.CityId= customer.CityId;
                    cusid.Gender= customer.Gender;
                    cusid.Notes= customer.Notes;
                    _context.SaveChanges();
                    _context.Update(cusid);
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
                return RedirectToAction("Details", new { Id = customer.Id });
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

            return RedirectToAction("QuatationIndex", "RepresntitiveAndAdvisor", new { id = cusromerID });
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
        public bool instexist(int id)
        {
            if (_context.InstallationRequests.Any(request => request.QutationId == id))
                return false;
            else return true;
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
            ViewData["MyData"] = _context.InstallationRequests.ToList();
            ViewData["productionData"] = _context.ProductionSchedules.ToList();
            ViewData["instSchedData"] = _context.InstallationSchedules.ToList();
            // var qu = _context.Quotations.Where(qu => qu.CustomerId == id && qu.BranchId == idc&&instexist(qu.QuotationId)).Include(qu => qu.Branch).Include(qu => qu.Customer).ToList();
            var qu = (from q in _context.Quotations
                      join branch in _context.Branches on q.BranchId equals branch.BranchId
                      join custom in _context.Customers on q.CustomerId equals custom.Id
                      join add in _context.Addresses on q.addressId equals add.id
                      join city in _context.Cities on add.cityid equals city.CityId


                      where q.CustomerId==id&& q.BranchId==idc&&q.addressId!=null &&_context.OrderDetails.Where(o=>o.QoutationId==q.QuotationId).Any()
                      select new Quotation
                      {
                         QuotationId=q.QuotationId,
                          Customer=custom,
                          QuotationStatus=  q.QuotationStatus,
                          Code=q.Code,
                          Branch=q.Branch,
                          employeeId=q.employeeId,
                          QuotationSimpleDate=q.QuotationSimpleDate,
                           Notes=q.Notes,
                          discountType=city.CityName


                      }
                      
                      ).ToList();
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
        public IActionResult GetItemColorById(int ItemId)
        {
          
            var col = _context.Colors.ToArray();
            var itemRawMaterial=_context.RowMaterialItems.Where(r=>r.ItemId==ItemId).ToList();
            if (itemRawMaterial.Count == 0)
            {
                return Json("empty");
            }

            if (itemRawMaterial.Count == 1)
            {
              var  color1 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[0].RowMatererialId).Include(c=>c.Color);
                string[] color2= new string[] { };
                string[] color3= new string[] { };
                return Json(new { color1 = color1,color2=color2,color3= color3 });
            }
           else if (itemRawMaterial.Count == 2)
            {
                string[] color3 = new string[] { };

                var color1 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[0].RowMatererialId).Include(c=>c.Color);
              var  color2 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[1].RowMatererialId).Include(c=>c.Color);
                return Json(new { color1 = color1,color2=color2,color3=color3, col });
            }
           else 
            {
                var color1 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[0].RowMatererialId).Include(c => c.Color);
                var color2 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[1].RowMatererialId).Include(c => c.Color);
                var color3 = _context.RoeMaterialColors.Where(c => c.RowMaterialId == itemRawMaterial[2].RowMatererialId).Include(c => c.Color);
                return Json(new { color1 = color1,color2=color2,color3=color3 });

            }



            //ItemColor[] ics = _context.ItemColors.Where(c => c.ItemId == ItemId && c.isEnabled == true).ToArray();
            //List<Color> colors = new List<Color>();
            //for (int i = 0; i < ics.Length; i++)
            //{
            //    colors.Add(_context.Colors.Find(ics[i].ColorId));
            //}
            //return colors.ToArray();
        }
        //*
        public void bager()
        {
            ViewBag.count = _context.Items.Count();
            ViewBag.count1 = _context.ItemTypes.Count();
            ViewBag.count2 = _context.Customers.Count();
            ViewBag.count3 = _context.MainItem.Count();
           
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
            ViewBag.addr = Branch.Address;
            var query = (


                //from bill in _context.Bills
                from qu in _context.Quotations
                    //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
                join oreder in _context.OrderDetails on qu.QuotationId equals oreder.QoutationId
                join item in _context.Items on oreder.ItemId equals item.ItemId
                join sub in _context.SubTypes on item.ItemSubTypeId equals sub.SubTypeId
                //join itemType in _context.ItemTypes on sub.TypeId equals itemType.TypeId
                //join mainitem in _context.MainItem on itemType.MainItemId equals mainitem.Id
                // join mainitem in _context.MainItem on item.MainItemId equals mainitem.Id
               // join mainitem in _context.MainItem on oreder.MainItemId equals mainitem.Id


                join room in _context.QutationRoom on oreder.roomId equals room.id

                where qu.QuotationId == id

                select new
                         BillModel
                {
                    //code = "123",
                    itemname = item.ItemArName,
                    notes = item.Notes,
                    //unit = "unit",
                    Unitprice = (double)item.ItemCurrentPrice,
                    quantity = oreder.Quantity,
                    //discountValue = (double)oreder.Discount,
                    //percentType = oreder.DiscountType,
                    //phone = customer.Phone,
                    totalValue = (double)oreder.Price,
                    quationId = (int)qu.QuotationId,
                    MainItemName = sub.SubTypeArName,
                    //Customer = customer.ArabicName,
                    //gender = customer.Gender,

                    //branch = Branch.BranchName,
                    //BillDate = bill.BillDate,
                    roomId = room.id,
                    roomName = room.roomName,
                    Colorname = oreder.colorname,
                    Colorname1 = oreder.colorname1,
                    Colorname2 = oreder.colorname2,
                    UOM=item.UOM





                });




           
            var res = query.OrderBy(i => i.roomName).ToList();

            return View(res);





        }



        //*
        public JsonResult GetQutationUpdate(int qid, int itemid, float price, float quantity, float discount, string type, float total, string roomId, string color, string color1, string color2, int mainItemId)

        {
            var item = _context.Items.Where(i => i.ItemId == itemid).FirstOrDefault();
            // var mainItem = _context.MainItem.Where(i => i.MainItemName == mainItemId).FirstOrDefault();
            var room = _context.QutationRoom.Where(i => i.roomName == roomId && i.qutationId == qid).FirstOrDefault();
            var id = qid;
            var quotation = _context.Quotations.Find(id);
            OrderDetail[] olddetail = _context.OrderDetails.Where(c => (c.QoutationId == id) && (c.roomId == room.id) && (c.ItemId == item.ItemId)).ToArray();
            if (olddetail.Length > 0)
            {
                Console.WriteLine(olddetail.Length);
                _context.OrderDetails.RemoveRange(olddetail);
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
                if ((detail.colorname == "الرجاء اختيار اللون") || (detail.colorname == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname = "NOCOLOR";
                }
                else
                {
                    detail.colorname = color;

                }
                if ((detail.colorname1 == "الرجاء اختيار اللون") || (detail.colorname1 == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname1 = "NOCOLOR";
                }
                else
                {
                    detail.colorname1 = color1;

                }
                if ((detail.colorname2 == "الرجاء اختيار اللون") || (detail.colorname2 == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname2 = "NOCOLOR";
                }
                else
                {
                    detail.colorname2 = color2;

                }


                _context.OrderDetails.Add(detail);

                _context.SaveChanges();
                return Json("added");
            }
            else {
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
                if ((detail.colorname == "الرجاء اختيار اللون") || (detail.colorname == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname = "NOCOLOR";
                }
                else
                {
                    detail.colorname = color;

                }
                if ((detail.colorname1 == "الرجاء اختيار اللون") || (detail.colorname1 == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname1 = "NOCOLOR";
                }
                else
                {
                    detail.colorname1 = color1;

                }
                if ((detail.colorname2 == "الرجاء اختيار اللون") || (detail.colorname2 == "غير مطلوب تحديد اللون "))
                {
                    detail.colorname2 = "NOCOLOR";
                }
                else
                {
                    detail.colorname2 = color2;

                }


                _context.OrderDetails.Add(detail);


                _context.SaveChanges();
                return Json(new { result = detail });






            }


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
           

            
            var res2 = from queryResult in _context.ProductionSchedules
                       where (_context.ProductionSchedules.Where(c => c.IntallationRequestId == queryResult.IntallationRequestId).Count() == 1) && (_context.ProductionSchedules.Count(c => c.IntallationRequestId == queryResult.IntallationRequestId && c.IsConfirmed == 1) == 0) && queryResult.IsConfirmed==0 && (queryResult.ProductionDate == DateTime.Now.Date || queryResult.ProductionDate == DateTime.Now.Date.AddDays(1) || queryResult.ProductionDate == DateTime.Now.Date.AddDays(2))
                       group queryResult by queryResult.IntallationRequestId into a
                       
                       select a.OrderByDescending(g => g.Id).First();



            return Json(new { BranchName,res2.ToList().Count });
        }
        //*
        public async Task<JsonResult> GetCityNameAsync()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branch =  _context.Branches.Find(user.BranchId);
            var city =  _context.Cities.Find(branch.CityId);
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
            ViewBag.add = _context.Addresses.Find(customer.adressId);
            ViewBag.CID= id;
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
                      join item in _context.Items on od.ItemId equals item.ItemId
                      join subtype in _context.SubTypes on item.ItemSubTypeId equals subtype.SubTypeId
                      where od.QoutationId == id
                      group od by new { od.roomId, subtype.SubTypeId }
                       into v
                      select new
                      {
                          mainId = v.Key.SubTypeId,
                          roomid = v.Key.roomId,
                          value = v.Sum(x => x.Price)

                      };
            var y = sub.ToList();
            var query = from mainitem in _context.SubTypes
                        join vs in sub on mainitem.SubTypeId equals vs.mainId
                        join room in _context.QutationRoom on vs.roomid equals room.id
                        // join room in _context.QutationRoom on id equals room.id



                        select new
                        {
                            roomname = room.roomName,
                            id = vs.mainId,
                            total = vs.value.ToString("#.#"),
                            mainitem = mainitem.SubTypeArName              }
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
              // join price in _context.ItemsPrices on item.ItemId equals price.ItemId

               where room.id == id

               select new BillModel
               {
                   code = qu.Code == null ? "0" : qu.Code,
                   itemname = item.ItemArName,
                   unit = "unit",
                   Unitprice = (double)item.ItemCurrentPrice,
                   quantity = (int)oreder.Quantity,
                   discountValue = (double)oreder.Discount,
                   percentType = oreder.DiscountType,
                   phone = customer.Phone,
                   totalValue = (double)oreder.Price,

                   quationId = (int)qu.QuotationId,
                   Customer = customer.ArabicName,
                   gender = customer.Gender,

                   branch = Branch.BranchName,
                   //   BillDate = bill.BillDate,
                   roomId = room.id,
                   roomName = room.roomName,
                   Colorname = oreder.colorname,
                   UOM=item.UOM





               });


            var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            ViewBag.tax = tax.tax;

            var res = query.OrderBy(i => i.roomName).ToList();
          
            return Json(res);


        }

      
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
                return RedirectToAction("Index");
            }

           
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

                     
                        select new
                        {
                            branch = branch.BranchName,
                            total = vs.value,
                            date = vs.d,

                        }
                        ;
            var x = query.ToArray();





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
        public IActionResult csvView() 
        {
            return View();
        }
        public IActionResult CreateOrder()
        {
            //ViewData["Clients"] = new SelectList(_context.Customers, "Id", "ArabicName");
            return View();
        }

        public IActionResult GetClients()
        {
            var client = _context.Customers.ToList();
            return Json(client);
        }
        public IActionResult CreateCustomer()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer([Bind("Id,ArabicName,EnglishName,Email,Phone,PhonNumber,Address,SecondAddress,CityId,Gender,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                Employee user = await userManager.GetUserAsync(HttpContext.User);

                customer.branchId = (int)user.BranchId;
                customer.Phone = "+05" + customer.Phone;
                customer.PhonNumber = "+05" + customer.PhonNumber;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreateOrder");
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(customer);
        }

        public async Task<IActionResult> GetAllOldQuotationOrdersAsync(int Qid)
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branchId = (int)user.BranchId;
            var branch = _context.Branches.Find(branchId);
            var branchname = branch.BranchName;
            var query = (from order in _context.OrderDetails
                         join qu in _context.Quotations on order.QoutationId equals Qid
                         join item in _context.Items on order.ItemId equals item.ItemId
                         join sub in _context.SubTypes on item.ItemSubTypeId equals sub.SubTypeId



                         join room in _context.QutationRoom on order.roomId equals room.id

                         where qu.QuotationId == Qid
                         select new
                         {
                             subText = sub.SubTypeArName,


                             itemsText = item.ItemCode,
                             //(item.ItemSubTypeId==58||item.ItemSubTypeId==59||item.ItemSubTypeId==60||item.ItemSubTypeId==63||item.ItemSubTypeId==61||item.ItemSubTypeId==62||item.ItemSubTypeId==64||item.ItemSubTypeId==65)?item.ItemCode : item.Notes,
                             itemsId = item.ItemId,
                             itemdes = item.Notes,
                             roomText = room.roomName,
                             roomId = room.id,
                             uom = item.UOM,
                             qytinput = order.Quantity,
                             priceinput = (_context.ItemsPrices.Where(p => p.ItemName == item.ItemArName && p.BranchName == branchname).Any()) ? _context.ItemsPrices.Where(p => p.ItemName == item.ItemArName && p.BranchName == branchname).Select(i => i.price).FirstOrDefault() : 0,
                             //rm=item.RMCost,
                             //fcost=item.FCost,
                             //icost=item.ICost,
                             colorsText = order.colorname,
                             colorsText1 = order.colorname1,
                             colorsText2 = order.colorname2,










                         });



            var res = query.ToList();
            return Json(res);
        }

        public JsonResult removeAllOrders(int qid)
        {
            var orders = _context.OrderDetails.Where(o => o.QoutationId == qid).ToList();
            if (orders.Any())
            {
                _context.OrderDetails.RemoveRange(orders);
                _context.SaveChanges();
            }
            return Json("done");
        }

        public async Task<IActionResult> CreateRoomForQutation(int id)
        {
            Customer cu = _context.Customers.Find(id);

            var qutation = _context.Quotations.Where(q => q.CustomerId == id).OrderBy(i=>i.QuotationId).LastOrDefault();
            Address address = _context.Addresses.Find(cu.adressId);
            ViewBag.neighborhood = address.neighborhood;
            ViewBag.street = address.street;
            ViewBag.houseNumber = address.houseNumber;
            ViewBag.customerId = cu.Id;
            ViewBag.qutationId = qutation.QuotationId;
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");


            return View();
        }

        public async Task<JsonResult> AddRoomQutation(string name, string note, int qid, int discount, string discountType, int qutationid)

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


    }
}
