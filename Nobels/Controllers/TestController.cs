using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nobels.Models;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.CodeAnalysis.Differencing;

namespace Nobels.Controllers
{
    public class TestController : Controller
    {
        private readonly db_a8d242_exadecor2Context _context;
        private UserManager<Employee> userManager;
        public readonly IPasswordHasher<Employee> _passwordHasher;
        public IActionResult Index()
        {
            return View();
        }
        public TestController(db_a8d242_exadecor2Context context, UserManager<Employee> userManager, IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            this.userManager = userManager;
            _passwordHasher = passwordHasher;

        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile formFile)
        {
            ItemsPrices q = new ItemsPrices();
            //Employee user = await userManager.GetUserAsync(HttpContext.User);
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

           // q.branchId = (int)user.BranchId;
           // var branch = _context.Branches.Find(q.branchId);
           // var branchname = branch.BranchName;
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
                    Delimiter = ",",
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
                      //  var branchId = csvReader.GetField(0);
                       // var ItemId = csvReader.GetField(1);
                        var ItemCurrentPrice = float.Parse( csvReader.GetField(2));
                        var ItemName= csvReader.GetField(0).ToString();
                        var BranchName = csvReader.GetField(1).ToString();
                        var pricefound = _context.ItemsPrices.Where(p => p.ItemName == ItemName && p.BranchName == BranchName).FirstOrDefault();
                       if(pricefound!=null)
                        {
                            pricefound.price =ItemCurrentPrice;
                            _context.ItemsPrices.Update(pricefound);
                            _context.SaveChanges();
                        }
                       else
                        {
                            await _context.ItemsPrices.AddAsync(new ItemsPrices
                            {
                                branchId = 0,
                                ItemId = 0,
                                price = ItemCurrentPrice,
                                ItemName = ItemName,
                                BranchName = BranchName

                                
                            });
                            _context.SaveChanges();
                        }
                       
                        
                    }
                }
            }
            return RedirectToAction("home", "RepresntitiveAndAdvisor");


            // return ViewComponent("Index");
        }

        [HttpPost]
        public FileResult Export()
        {
            List<object> prices = (from itemprice in _context.ItemsPrices
                                      select new object[] {
                                        itemprice.ItemName,
                                        itemprice.BranchName,
                                        itemprice.price,
                                        itemprice.branchId,
                                        itemprice.ItemId
                                        
                                 }).ToList<object>();
            //Insert the Column Names.
            prices.Insert(0, new string[3] { "ItemName",  "BranchName" , "price" });

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < prices.Count; i++)
            {
                //string[] customer = (string[])customers[i];
                object[] customer = (object[])prices[i];
                for (int j = 0; j < customer.Length; j++)
                {
                    //Append data with separator.
                    sb.Append(customer[j].ToString() + ',');
                }

                //Append new line character.
                sb.Append("\r\n");

            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "ItemPrices.csv");
        }
    }

}

