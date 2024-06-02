using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;
using System.Globalization;
using System.Security.Claims;

namespace Nobels.Controllers
{

    [Authorize]
    //ProductManager user:8099
    //roleId:6
    public class ProductManager : Controller
    {



        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;
        private readonly IWebHostEnvironment _appEnvironment;



        public ProductManager(db_a8d242_exadecor2Context context, IWebHostEnvironment env, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;

            _appEnvironment = env;


        }
        //**the start of section "العروض"
        //api:used in layout
        public async Task<JsonResult> GetBranchNameAsync()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branch = await _context.Branches.FindAsync(user.BranchId);
            var BranchName = branch.BranchName;


            return Json(BranchName);
        }
        //api:used in layout

        public async Task<JsonResult> GetCityNameAsync()
        {
            Employee user = await userManager.GetUserAsync(HttpContext.User);
            var branch = await _context.Branches.FindAsync(user.BranchId);
            var city = await _context.Cities.FindAsync(branch.CityId);
            var cityName = city.CityName;

            return Json(cityName);
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
                return RedirectToAction(nameof(IndexQutation));
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

       

        public IActionResult IndexQutation()
        {


            var query = (


            //from bill in _context.Bills
               from qu in _context.Quotations
                   //join qu in _context.Quotations on bill.QuotationId equals qu.QuotationId
               join customer in _context.Customers on qu.CustomerId equals customer.Id


               join branch in _context.Branches on qu.BranchId equals branch.BranchId
               join Employee in _context.Employees on qu.employeeId equals Employee.Id


               where qu.QuotationStatus == "جديد" || qu.QuotationStatus == "مرفوض"

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





               });
            var res = query.ToList();

            return View(res);
        }

        //action: in IndexQutation
        public async Task<IActionResult> OrderDetailsUpdateAsync(int id)
        {
            var qu1 = _context.Quotations.Where(q => q.QuotationId == id).FirstOrDefault();
            ViewBag.qqid = qu1.CustomerId;
            ViewBag.qqqid = id;
            int stat = 0;
            if (qu1.QuotationStatus == "مقبول" || qu1.QuotationStatus == "مرفوض")
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




            var roles = await userManager.GetRolesAsync(user);

            if (user != null && roles != null && ((roles.Contains("استشاري مبيعات")) || (roles.Contains("مندوب مشاريع"))))
            {

                ViewBag.role = 1;

            }



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



            var res = query.ToList();
            return View(res);
        }

        //action:in OrderDetailsUpdate
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
            ViewBag.id = orderDetail.QoutationId;
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
                update.Updates = "تم الحذف بتاريخ:" + " " + DateTime.Now + " " + "من قبل   " + employee.UserName
                   ;



                _context.Add(update);
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("OrderDetailsUpdate", "ProductManager", new { id = orderDetail.QoutationId });
        }

        //action:in OrderDetailsUpdate

        public IActionResult CreateOrderDetail(int qid)
        {
            ViewData["itemMain"] = new SelectList(_context.ItemTypes, "TypeId", "TypeArName");

            ViewData["MainItem"] = new SelectList(_context.MainItem, "Id", "MainItemName");

            return View();
        }

        //action:in OrderDetailsUpdate


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
        //api:in OrderDetailsUpdate

        public IActionResult ShowQuotationUpdate(int quotationId)
        {
            //var update = _context.QoutationUpdates.Find(quotationId);
            var note = _context.QoutationUpdates.Where(q => q.QoutationId == quotationId);
            return Json(note.ToList());
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


        //action:in editOrder

        public IActionResult changeItemColor(int ItemColorId)
        {
            var list = _context.ItemColors.Where(id => id.ItemId == ItemColorId).Include(o => o.Color).Include(s => s.Item).ToList();

            return View(list);
        }
        //api:in editOrder
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
                   quantity + "من قبل" +
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
        //api:in editOrder
        public IActionResult ItemsColor(int orderid)
        {
            var order = _context.OrderDetails.Find(orderid);
            int Item_colore = (int)order.ItemId;
            var coloritems = _context.ItemColors.Where(c => c.ItemId == Item_colore && c.isEnabled == true).Include(o => o.Color);


            return Json(coloritems);
        }

        //api:in changeItemColor
        public async Task<JsonResult> color(int Iid)
        {
            var color = _context.ItemColors.Where(id => id.ItemColorId == Iid).FirstOrDefault();
            color.isEnabled = !color.isEnabled;
            _context.Update(color);
            _context.SaveChanges();
            return Json(new { Iid });
        }

        //api:in changeItemColor

        public IActionResult UpdatePrice(int ColorId, int itemId, float NewPrice)
        {

            var item = _context.ItemColors.Where(i => i.ColorId == ColorId && i.ItemId == itemId).FirstOrDefault();
            item.SpecialPrice = NewPrice;

            _context.Update(item);
            _context.SaveChanges();
            return Json(item.SpecialPrice);
        }
        //end of "العروض"**
        //*************************************************************************









       

       
    }
}
