using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;
using System.Security.Claims;

namespace Nobels.Controllers
{

    //المكتب المالي:112233
    //roleId:18
    [Authorize]
    public class FinancialOffice : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;


        public FinancialOffice(db_a8d242_exadecor2Context context, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;
        }
        public JsonResult GetTax()
        {
            var tax = _context.Settings.Where(i => i.id == 1).FirstOrDefault();
            return Json(tax.tax);
        }

        //api:in CreateOrderDetail
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
        //api:in CreateOrderDetail

        public JsonResult RoomDiscount(int roomId)
        {
            var room = _context.QutationRoom.Find(roomId);
            string type = room.discountType;
            int value = (int)room.discount;
            return Json(new { type, value });
        }
        //api:in CreateOrderDetail

        public JsonResult getTypes(int id)
        {
            //List<ItemType> listSub = _context.ItemTypes.Where(x => x.MainItemId == id).ToList();
            List<ItemType> listSub = _context.ItemTypes.ToList();

            return Json(listSub);
        }
        //api:in CreateOrderDetail
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
        //api:in CreateOrderDetail
        public IActionResult GetRoom(int q)
        {
            // var order = _context.OrderDetails.Find(orderid);
            // int Item_colore = (int)order.ItemId;
            var rooms = _context.QutationRoom.Where(c => c.qutationId == q);


            return Json(rooms);
        }
        //api:in CreateOrderDetail
        public JsonResult Getsub(int cid)
        {
            List<SubType> listSub = _context.SubTypes.Where(x => x.TypeId == cid).ToList();

            return Json(listSub);
        }
        //api:in CreateOrderDetail
        public JsonResult GetItem(int subid)
        {
            List<Item> listItem = _context.Items.Where(x => x.ItemSubTypeId == subid).ToList();

            return Json(listItem);
        }
        //api:in CreateOrderDetail

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
        //api:in CreateOrderDetail
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
        //........................................................


        public IActionResult ShowQuotationUpdate(int quotationId)
        {
            //var update = _context.QoutationUpdates.Find(quotationId);
            var note = _context.QoutationUpdates.Where(q => q.QoutationId == quotationId);
            return Json(note.ToList());
        }

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

            return View(c);
        }

        public async Task<IActionResult> OrderDetailsUpdateAsync(int id)
        {
            var qu1 = _context.Quotations.Where(q => q.QuotationId == id).FirstOrDefault();
            ViewBag.qqid = qu1.CustomerId;
            ViewBag.qqqid = id;
            
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

        public async Task<IActionResult> OrderDetailsAsync(int id)
        {

            ViewBag.qqqid = id;





            var query = (from order in _context.OrderDetails
                         join qu in _context.Quotations on order.QoutationId equals id
                         join cus in _context.Customers on qu.CustomerId equals cus.Id
                         join item in _context.Items on order.ItemId equals item.ItemId

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

                             MainItemName = mainitem.MainItemName


                         });

            ViewBag.qId = id;

            var res = query.ToList();
            return View(res);
        }


        public async Task<IActionResult> Accept(int? id)
        {
            ViewBag.Id = id;
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var qu = await _context.Quotations.FindAsync(id);
            if (qu == null)
            {
                return NotFound();
            }
            return View(qu);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            var qu = await _context.Quotations.FindAsync(id);
            if (id != qu.QuotationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    qu.QuotationStatus = "مقبول مالياً";
                    _context.Update(qu);
                    await _context.SaveChangesAsync();


                    QoutationUpdate update = new QoutationUpdate();
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    update.ChangeDate = DateTime.Now;
                    update.QoutationId = qu.QuotationId;
                    update.EmployeeId = Int32.Parse(userId);
                    //var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                    update.Updates = "تم قبول العرض مالياً بتاريخ" + DateTime.Now;
                    _context.Add(update);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!quExists(qu.QuotationId))
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
            // ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(qu);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> refuse(int id)
        {

            var qu = await _context.Quotations.FindAsync(id);
            if (id != qu.QuotationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    qu.QuotationStatus = "مرفوض مالياً";
                    _context.Update(qu);
                    await _context.SaveChangesAsync();


                    QoutationUpdate update = new QoutationUpdate();
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    update.ChangeDate = DateTime.Now;
                    update.QoutationId = qu.QuotationId;
                    update.EmployeeId = Int32.Parse(userId);
                    //var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                    update.Updates = "تم رفض  العرض مالياً بتاريخ" + DateTime.Now;
                    _context.Add(update);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!quExists(qu.QuotationId))
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
            // ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(qu);
        }

        private bool quExists(int id)
        {
            return _context.Quotations.Any(e => e.QuotationId == id);
        }

        public async Task<IActionResult> Delete(int? id)
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
            ViewBag.Id = id;


            return View(quotation);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id ,string note)
        {



            var qu = await _context.Quotations.FindAsync(id);
            if (id != qu.QuotationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                
                try
                {
                    qu.QuotationStatus = "مرفوض مالياً";
                    qu.Notes = note;
                    _context.Update(qu);
                    await _context.SaveChangesAsync();

                    if (note != null || note != String.Empty) {
                        QoutationUpdate update = new QoutationUpdate();
                    var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    update.ChangeDate = DateTime.Now;
                    update.QoutationId = qu.QuotationId;
                    update.EmployeeId = Int32.Parse(userId);
                    //var employee = _context.Employees.FirstOrDefault(x => x.Id == update.EmployeeId);
                    update.Updates = "تم رفض العرض مالياً بتاريخ" + DateTime.Now + " ," + "السبب:" + note;
                    _context.Add(update);

                    await _context.SaveChangesAsync();
                    }
                        
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!quExists(qu.QuotationId))
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
            // ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", customer.CityId);
            return View(qu);
        }


        public IActionResult Index()
        {


            var query = (


            //from bill in _context.Bills
               from qu in _context.Quotations
                   //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
               join customer in _context.Customers on qu.CustomerId equals customer.Id


               join branch in _context.Branches on qu.BranchId equals branch.BranchId
               join Employee in _context.Employees on qu.employeeId equals Employee.Id
               join add in _context.Addresses on qu.addressId equals add.id
               join city in _context.Cities on add.cityid equals city.CityId

               where /*qu.QuotationStatus == "مقبول فنياً"||*/qu.QuotationStatus== "قيد التدقيق المالي"

               select new
                        BillModel
               {
                   Customer = customer.ArabicName,
                   //gender = customer.Gender,

                   branch = branch.BranchName,
                   BillDate = qu.QuotationDate,
                   quationId = qu.QuotationId,
                   employeeName = Employee.FirstName + Employee.LastName,
                   status = qu.QuotationStatus,
                   code = qu.Code,
                   CityName=city.CityName






               });
            var res = query.ToList();

            return View(res);
        }

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

            //var sub = from od in _context.OrderDetails
            //          where od.QoutationId == id
            //          group od by new { od.roomId, od.MainItemId }
            //           into v
            //          select new
            //          {
            //              mainId = v.Key.MainItemId,
            //              roomid = v.Key.roomId,
            //              value = v.Sum(x => x.TotalValue)

            //          };
            //var y = sub.ToList();
            //var query = from mainitem in _context.MainItem
            //            join vs in sub on mainitem.Id equals vs.mainId
            //            join room in _context.QutationRoom on vs.roomid equals room.id
            //            // join room in _context.QutationRoom on id equals room.id



            //            select new
            //            {
            //                roomname = room.roomName,
            //                id = vs.mainId,
            //                total = vs.value,
            //                mainitem = mainitem.MainItemName
            //            }
            //            ;
            //var x = query.ToList();







            // return Json(query);
            //var sub = from od in _context.OrderDetails
            //          where od.QoutationId == id
            //          group od by new { od.roomId, od.MainItemId }
            //           into v
            //          select new
            //          {
            //              mainId = v.Key.MainItemId,
            //              roomid = v.Key.roomId,
            //              value = v.Sum(x => x.TotalValue)

            //          };
            //var y = sub.ToList();
            //var query = from mainitem in _context.MainItem
            //            join vs in sub on mainitem.Id equals vs.mainId
            //            join room in _context.QutationRoom on vs.roomid equals room.id
            //            // join room in _context.QutationRoom on id equals room.id



            //            select new
            //            {
            //                roomname = room.roomName,
            //                id = vs.mainId,
            //                total = vs.value,
            //                mainitem = mainitem.MainItemName
            //            }
            //            ;
            //var x = query.ToList();


            // return Json(query);


            //**********
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
                          value = v.Sum(x => x.TotalValue)

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
                            total = vs.value,
                            mainitem = mainitem.SubTypeArName
                        }
                        ;
            var x = query.ToList();







            return Json(query);



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
                    Unitprice = (double)oreder.Price,
                    quantity = oreder.Quantity,
                    //discountValue = (double)oreder.Discount,
                    //percentType = oreder.DiscountType,
                    //phone = customer.Phone,
                    totalValue = (double)oreder.TotalValue,
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
                    UOM = item.UOM





                });





            var res = query.OrderBy(i => i.roomName).ToList();

            return View(res);





        }

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
                   Colorname = oreder.colorname,
                   UOM = item.UOM





               });


            var tax = _context.Settings.FirstOrDefault(x => x.id == 1);
            ViewBag.tax = tax.tax;

            var res = query.OrderBy(i => i.roomName).ToList();
            //ViewBag.count = query.Count();

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

            //var branch = _context.Branches.Where(i => i.BranchId == employee.BranchId).FirstOrDefault();
            //var Role = _context.EmployeeRoles.Where(i => i.RoleId == employee.RoleId).FirstOrDefault();

            //ViewBag.BranchId = branch.BranchName;
            //ViewBag.RoleId = Role.RoleName;
            return View(employee);

        }

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


        public IActionResult RefuseQoutation()
        {


            var query = (


            //from bill in _context.Bills
               from qu in _context.Quotations
                   //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
               join customer in _context.Customers on qu.CustomerId equals customer.Id


               join branch in _context.Branches on qu.BranchId equals branch.BranchId
               join Employee in _context.Employees on qu.employeeId equals Employee.Id
               join add in _context.Addresses on qu.addressId equals add.id
               join city in _context.Cities on add.cityid equals city.CityId

               where qu.QuotationStatus == "مرفوض مالياً"

               select new
                        BillModel
               {
                   Customer = customer.ArabicName,
                   //gender = customer.Gender,

                   branch = branch.BranchName,
                   BillDate = qu.QuotationDate,
                   quationId = qu.QuotationId,
                   employeeName = Employee.FirstName + Employee.LastName,
                   status = qu.QuotationStatus,
                   code = qu.Code,
                   CityName=city.CityName





               });
            var res = query.ToList();
            var qUpdate = _context.QoutationUpdates.Where(q => q.Updates.Contains("تم رفض العرض مالياً")).ToList();
            ViewData["MyData"] = qUpdate;

            return View(res);
        }

        public IActionResult AcceptQoutation()
        {


            var query = (


            //from bill in _context.Bills
               from qu in _context.Quotations
                   //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
               join customer in _context.Customers on qu.CustomerId equals customer.Id


               join branch in _context.Branches on qu.BranchId equals branch.BranchId
               join Employee in _context.Employees on qu.employeeId equals Employee.Id
               join add in _context.Addresses on qu.addressId equals add.id
               join city in _context.Cities on add.cityid equals city.CityId

               where qu.QuotationStatus == "مقبول مالياً"

               select new
                        BillModel
               {
                   Customer = customer.ArabicName,


                   branch = branch.BranchName,
                   BillDate = _context.QoutationUpdates.Where(i=>i.QoutationId==qu.QuotationId&&i.Updates.Contains("قبول العرض مالياً")).First().ChangeDate,
                   quationId = qu.QuotationId,
                   employeeName = Employee.FirstName + Employee.LastName,
                   status = qu.QuotationStatus,
                   code = qu.Code,
                   CityName=city.CityName






               });
            var res = query.ToList();

            return View(res);
        }

        public async Task<IActionResult> deleteOrder(int? id)
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

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteOrderConfirmed(int id)
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
                update.Updates = "تم الحذف بتاريخ:" + " " + DateTime.Now + " " + "من قبل   " + " " + " المكتب المالي"
                   ;



                _context.Add(update);
                _context.OrderDetails.Remove(orderDetail);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("OrderDetailsUpdate", "FinancialOffice", new { id = orderDetail.QoutationId });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.DetailId == id);
        }




        public async Task<IActionResult> editOrder(int? id)
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
        //api:in editOrder
        public IActionResult editApi(int orderid, float quantity, string colorname, string colorname1, string colorname2, int UserId)
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
                   " "+"المكتب المالي"
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

        //api:in editOrder
        public IActionResult ItemsColor(int orderid)
        {
            var order = _context.OrderDetails.Find(orderid);
            int Item_colore = (int)order.ItemId;
            var coloritems = _context.ItemColors.Where(c => c.ItemId == Item_colore && c.isEnabled == true).Include(o => o.Color);


            return Json(coloritems);
        }

      

        public IActionResult changeItemColor(int ItemColorId)
        {
            var list = _context.ItemColors.Where(id => id.ItemId == ItemColorId).Include(o => o.Color).Include(s => s.Item).ToList();

            return View(list);
        }


    }
}
