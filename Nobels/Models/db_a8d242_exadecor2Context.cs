

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nobels.Models;
using Nobels.Models;

namespace Nobels.Models
{
    public class db_a8d242_exadecor2Context : IdentityDbContext<Employee, AppRole, int>
    {
        public db_a8d242_exadecor2Context()
        {
        }

        public db_a8d242_exadecor2Context(DbContextOptions<db_a8d242_exadecor2Context> options)
            : base(options)
        {
        }

        

        public virtual DbSet<QutationRoom> QutationRoom { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Email> Emails { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<ItemColor> ItemColors { get; set; } = null!;
        public virtual DbSet<ItemType> ItemTypes { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<QoutationUpdate> QoutationUpdates { get; set; } = null!;
        public virtual DbSet<Quotation> Quotations { get; set; } = null!;
        public virtual DbSet<SubType> SubTypes { get; set; } = null!;

        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<MainItem> MainItem { get; set; } = null;
        public virtual DbSet<RoeMaterialColor> RoeMaterialColors { get; set; } = null!;
        public virtual DbSet<RowMaterialItem> RowMaterialItems { get; set; } = null!;
        public virtual DbSet<RawMaterial> RawMaterials { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<QutationStatus> QutationStatuses { get; set; } = null!;
        public virtual DbSet<SubStatus> SubStatuses { get; set; } = null!;


        public virtual DbSet<Team> Teams { get; set; } = null!;
        public virtual DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Warehouse> Warehouses { get; set; } = null!;
        public virtual DbSet<WarehouseUpdate> WarehouseUpdates { get; set; } = null!;
        public virtual DbSet<ProductionSchedule> ProductionSchedules { get; set; } = null!;
        public virtual DbSet<ProgressUpdate> ProgressUpdates { get; set; } = null!;
        public virtual DbSet<InstallationRequest> InstallationRequests { get; set; } = null!;
        public virtual DbSet<InstallationSchedule> InstallationSchedules { get; set; } = null!;
        public virtual DbSet<InstallationUpdate> InstallationUpdates { get; set; } = null!;
        public virtual DbSet<ItemsPrices> ItemsPrices { get; set; } = null!;





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=SQL5104.site4now.net;Initial Catalog=db_a8d242_exadecor2;User Id=db_a8d242_exadecor2_admin;Password=Let01@2030!");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.cityid).HasColumnName("cityid");

                entity.Property(e => e.neighborhood).HasMaxLength(150);
                entity.Property(e => e.street).HasMaxLength(150);
                entity.Property(e => e.houseNumber).HasMaxLength(40);

               
            });
            
            modelBuilder.Entity<ItemsPrices>(entity =>
            {
                entity.ToTable("ItemsPrices");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.branchId).HasColumnName("branchId");
                entity.Property(e => e.ItemId).HasColumnName("ItemId");

                entity.Property(e => e.price).HasColumnType("float");
                entity.Property(e => e.ItemName).HasMaxLength(500);
                entity.Property(e => e.BranchName).HasMaxLength(500);




            });


            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team");

                entity.Property(e => e.Id)
                   
                    .HasColumnName("id");
                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.Property(e => e.Id)
                   
                    .HasColumnName("id");


                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.TeamId).HasColumnName("teamId");
            });
            modelBuilder.Entity<SubStatus>(entity =>
            {
                entity.ToTable("SubStatus");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.QutationStatusId).HasColumnName("QutationStatusId");

                entity.Property(e => e.status).HasMaxLength(250);

                //entity.HasOne(d => d.Status)
                //    .WithMany(p => p.SubStatuses)
                //    .HasForeignKey(d => d.StatusId)
                //    .HasConstraintName("FK_SubStatus_Status");
            });
            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.Note)
                    .HasMaxLength(150)
                    .HasColumnName("note");
            });
            modelBuilder.Entity<QutationStatus>(entity =>
            {
                entity.ToTable("QutationStatus");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.QutationId).HasColumnName("qutationId");
                entity.Property(e => e.status).HasColumnName("status");

                entity.Property(e => e.Statusdate)
                    .HasColumnType("date")
                    .HasColumnName("statusdate");

                //  entity.Property(e => e.SubStatusId).HasColumnName("subStatusId");

                //entity.HasOne(d => d.Qutation)
                //    .WithMany(p => p.QutationStatuses)
                //    .HasForeignKey(d => d.QutationId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_QutationStatus_Quotation");

                //entity.HasOne(d => d.SubStatus)
                //    .WithMany(p => p.QutationStatuses)
                //    .HasForeignKey(d => d.SubStatusId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_QutationStatus_SubStatus");
            });
            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.ToTable("Quotation");

                entity.Property(e => e.QuotationId).HasColumnName("quotationId");

                entity.Property(e => e.BranchId).HasColumnName("branchId");
                entity.Property(e => e.addressId).HasColumnName("addressId");

                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .HasColumnName("code");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");
                entity.Property(e => e.employeeId).HasColumnName("employeeId");



                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.QuotationDate)
                    .HasColumnType("date")
                    .HasColumnName("quotationDate")
                    .HasComputedColumnSql("(CONVERT([date],[quotationSimpleDate]))", false);

                entity.Property(e => e.QuotationSimpleDate)
                    .HasColumnType("datetime")
                    .HasColumnName("quotationSimpleDate");


                entity.Property(e => e.enddate)
                   .HasColumnType("date")
                   .HasColumnName("enddate");

                //entity.Property(e => e.QuotationStatus)
                //    .HasMaxLength(50)
                //    .HasColumnName("quotationStatus");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Quotations)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Quotation_Customer");

                entity.Property(e => e.discount).HasColumnName("discount");

                entity.Property(e => e.discountType)
                  .HasMaxLength(50)
                  .HasColumnName("discountType");

            });

            modelBuilder.Entity<RowMaterialItem>(entity =>
            {
                entity.ToTable("RowMaterialItem");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.RowMaterialItems)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_RowMaterialItem_Item");

                entity.HasOne(d => d.RowMatererial)
                    .WithMany(p => p.RowMaterialItems)
                    .HasForeignKey(d => d.RowMatererialId)
                    .HasConstraintName("FK_RowMaterialItem_RawMaterial");
            });
            modelBuilder.Entity<RoeMaterialColor>(entity =>
            {
                entity.ToTable("RoeMaterialColor");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.RoeMaterialColors)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_RoeMaterialColor_Colors");

                entity.HasOne(d => d.RowMaterial)
                    .WithMany(p => p.RoeMaterialColors)
                    .HasForeignKey(d => d.RowMaterialId)
                    .HasConstraintName("FK_RoeMaterialColor_RawMaterial");
            });
            modelBuilder.Entity<RawMaterial>(entity =>
            {
                entity.ToTable("RawMaterial");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.DamagedPercent).HasColumnName("damagedPercent");

                entity.Property(e => e.MeasurUnit)
                    .HasMaxLength(50)
                    .HasColumnName("measurUnit");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.BillId).HasColumnName("billId");

                entity.Property(e => e.BillDate)
                    .HasColumnType("datetime")
                    .HasColumnName("billDate");

                entity.Property(e => e.BillLanguage)
                    .HasMaxLength(50)
                    .HasColumnName("billLanguage");

                entity.Property(e => e.BillNum)
                    .HasMaxLength(50)
                    .HasColumnName("billNum");

                entity.Property(e => e.BillText)
                    .HasMaxLength(500)
                    .HasColumnName("billText");

                entity.Property(e => e.discountType)
                   .HasMaxLength(500)
                   .HasColumnName("discountType");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.FinalValue)
                    .HasMaxLength(10)
                    .HasColumnName("finalValue")
                    .IsFixedLength();

                entity.Property(e => e.Notes)
                    .HasMaxLength(50)
                    .HasColumnName("notes");

                entity.Property(e => e.QuotationId).HasColumnName("quotationId");

                entity.Property(e => e.TotalValue).HasColumnName("totalValue");

                entity.HasOne(d => d.Quotation)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.QuotationId)
                    .HasConstraintName("FK_Bill_Quotation");
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch");

                entity.Property(e => e.BranchId).HasColumnName("branchId");

                entity.Property(e => e.Address)
                    .HasMaxLength(150)
                    .HasColumnName("address");
                entity.Property(e => e.branchImg)
                    .HasMaxLength(500)
                    .HasColumnName("branchImg");

                entity.Property(e => e.BranchName)
                    .HasMaxLength(50)
                    .HasColumnName("branchName");

                entity.Property(e => e.BranchType).HasColumnName("branchType");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.Email)
                    .HasMaxLength(250)
                    .HasColumnName("email");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .HasColumnName("location");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Branches)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Branch_City");
            });
            //FK_ItemType_MainItem
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.CityName)
                    .HasMaxLength(50)
                    .HasColumnName("cityName");
            });

            modelBuilder.Entity<MainItem>(entity =>
            {
                entity.ToTable("MainItem");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MainItemEnName)
                    .HasMaxLength(1000)
                    .HasColumnName("mainItemEnName");
                entity.Property(e => e.MainItemName)
                    .HasMaxLength(1000)
                    .HasColumnName("mainItemName");
                entity.Property(e => e.MainItemPhoto)
                    .HasMaxLength(500)
                    .HasColumnName("mainItemPhoto");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.tax)

                    .HasColumnName("tax")
                    .HasColumnType("decimal")
                    .HasPrecision(12, 2);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.Property(e => e.ColorId).HasColumnName("colorId");

                entity.Property(e => e.ColorCode)
                    .HasMaxLength(150)
                    .HasColumnName("colorCode");

                entity.Property(e => e.ColorName)
                    .HasMaxLength(150)
                    .HasColumnName("colorName");

                entity.Property(e => e.ColorTemplatePhoto)
                    .HasMaxLength(500)
                    .HasColumnName("colorTemplatePhoto");
                entity.Property(e => e.itemTypeId1).HasColumnName("itemTypeId");

                //  entity.Property(e => e.ItemTypeId).HasColumnName("itemTypeId");
                entity.HasOne(d => d.ItemTypeId)
                    .WithMany(p => p.Colors)
                    .HasForeignKey(d => d.itemTypeId1)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Colors_ItemType");


                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasColumnName("note");
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.adressId).HasColumnName("adressId");
                entity.Property(e => e.branchId).HasColumnName("branchId");
                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasColumnName("address");

                entity.Property(e => e.SecondAddress)
                    .HasMaxLength(250)
                    .HasColumnName("SecondAddress");

                entity.Property(e => e.ArabicName)
                    .HasMaxLength(150)
                    .HasColumnName("arabicName");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.Email)
                    .HasMaxLength(250)
                    .HasColumnName("email");

                entity.Property(e => e.EnglishName)
                    .HasMaxLength(150)
                    .HasColumnName("englishName");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .HasColumnName("gender");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone")
                    .IsFixedLength();

                entity.Property(e => e.PhonNumber)
                   .HasMaxLength(20)
                   .HasColumnName("PhonNumber")
                   .IsFixedLength();
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.Property(e => e.EmailId).HasColumnName("emailId");

                entity.Property(e => e.EmailText)
                    .HasMaxLength(500)
                    .HasColumnName("emailText");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.Receivers)
                    .HasMaxLength(500)
                    .HasColumnName("receivers");

                entity.Property(e => e.Senddate)
                    .HasColumnType("datetime")
                    .HasColumnName("senddate");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.BranchId).HasColumnName("branchId");

                entity.Property(e => e.Email)
                    .HasMaxLength(250)
                    .HasColumnName("email");

                entity.Property(e => e.EmployeeNumber)
                    .HasMaxLength(50)
                    .HasColumnName("employeeNumber");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(500)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.RegisterDate)
                    .HasColumnType("date")
                    .HasColumnName("registerDate");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("userName");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK_Employee_Branch");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Employee_EmployeeRoles");
            });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.Notes)
                    .HasMaxLength(200)
                    .HasColumnName("notes");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("roleName");
            });

            modelBuilder.Entity<QutationRoom>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.notes)
                    .HasMaxLength(150)
                    .HasColumnName("notes");

                entity.Property(e => e.roomName)
                    .HasMaxLength(50)
                    .HasColumnName("roomName");



                entity.Property(e => e.qutationId)
                .HasColumnName("qutationId");

                entity.Property(e => e.discount).HasColumnName("discount");

                entity.Property(e => e.discountType)
                  .HasMaxLength(50)
                  .HasColumnName("discountType");
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.HasIndex(e => e.ItemSubTypeId, "IX_Item_itemSubTypeId");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.COG).HasColumnName("COG");

                entity.Property(e => e.FCost).HasColumnName("FCost");

                entity.Property(e => e.ICost).HasColumnName("ICost");

                entity.Property(e => e.ItemArName)
                    .HasMaxLength(150)
                    .HasColumnName("itemArName");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(20)
                    .HasColumnName("itemCode");

                entity.Property(e => e.ItemCurrentPrice).HasColumnName("itemCurrentPrice");

                entity.Property(e => e.ItemEnName)
                    .HasMaxLength(150)
                    .HasColumnName("itemEnName");

                entity.Property(e => e.ItemPhotoPath)
                    .HasMaxLength(500)
                    .HasColumnName("itemPhotoPath");

                entity.Property(e => e.ItemSubTypeId).HasColumnName("itemSubTypeId");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.RMCost).HasColumnName("RMCost");

                entity.Property(e => e.UOM)
                    .HasMaxLength(150)
                    .HasColumnName("UOM");

                entity.HasOne(d => d.ItemSubType)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ItemSubTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Item_SubType");
            });

            modelBuilder.Entity<ItemColor>(entity =>
            {
                entity.ToTable("ItemColor");

                entity.Property(e => e.ItemColorId).HasColumnName("itemColorId");

                entity.Property(e => e.ItemId).HasColumnName("itemId");
                entity.Property(e => e.isEnabled).HasColumnName("isEnabled");

                entity.Property(e => e.SpecialPrice).HasColumnName("specialPrice");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.ItemColors)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_ItemColor_Colors");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemColors)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_ItemColor_Item");
            });

            modelBuilder.Entity<ItemType>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("ItemType");

                entity.Property(e => e.TypeId).HasColumnName("typeId");
                //entity.Property(e => e.MainItemId).HasColumnName("mainItemId");
                entity.Property(e => e.Photopath)
                    .HasMaxLength(250)
                    .HasColumnName("photopath");
                entity.Property(e => e.TypeArName)
                    .HasMaxLength(150)
                    .HasColumnName("typeArName");
                entity.Property(e => e.TypeEnName)
                    .HasMaxLength(150)
                    .HasColumnName("typeEnName");

                //entity.HasOne(d => d.MainItem).WithMany(p => p.ItemTypes)
                //    .HasForeignKey(d => d.MainItemId)
                //    .HasConstraintName("FK_ItemType_MainItem");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.ToTable("OrderDetail");

                entity.Property(e => e.DetailId).HasColumnName("detailId");

                entity.Property(e => e.colorname)
                   .HasMaxLength(50)
                   .HasColumnName("colorname");
                entity.Property(e => e.colorname1)
                   .HasMaxLength(50)
                   .HasColumnName("colorname1");
                entity.Property(e => e.colorname2)
                   .HasMaxLength(50)
                   .HasColumnName("colorname2");

                entity.Property(e => e.ItemId).HasColumnName("itemId");
                entity.Property(e => e.MainItemId).HasColumnName("mainItemId");


                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");



                entity.Property(e => e.DiscountType)
                   .HasMaxLength(50)
                   .HasColumnName("DiscountType");
                entity.Property(e => e.roomId).HasColumnName("roomId");





                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal").HasPrecision(12, 2);
                entity.Property(e => e.Discount).HasColumnName("Discount").HasColumnType("decimal").HasPrecision(12, 2);
                entity.Property(e => e.TotalValue).HasColumnName("TotalValue").HasColumnType("decimal").HasPrecision(12, 2);
                entity.Property(e => e.QoutationId).HasColumnName("qoutationId");

                entity.Property(e => e.Quantity).HasColumnName("quantity").HasColumnType("decimal").HasPrecision(12, 2);

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_OrderDetail_Item");

                entity.HasOne(d => d.Qoutation)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.QoutationId)
                    .HasConstraintName("FK_OrderDetail_Quotation");
                entity.HasOne(d => d.MainItem).WithMany(p => p.OrderDetails)
                 .HasForeignKey(d => d.MainItemId)
                 .HasConstraintName("FK_OrderDetail_MainItem");
            });

            modelBuilder.Entity<QoutationUpdate>(entity =>
            {
                entity.HasKey(e => e.UpdateId);

                entity.Property(e => e.UpdateId).HasColumnName("updateId");

                entity.Property(e => e.ChangeDate)
                    .HasColumnType("datetime")
                    .HasColumnName("changeDate");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.QoutationId).HasColumnName("qoutationId");

                entity.Property(e => e.Updates)
                    .HasMaxLength(500)
                    .HasColumnName("updates");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.QoutationUpdates)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_QoutationUpdates_Employee");

                entity.HasOne(d => d.Qoutation)
                    .WithMany(p => p.QoutationUpdates)
                    .HasForeignKey(d => d.QoutationId)
                    .HasConstraintName("FK_QoutationUpdates_Quotation");
            });


            modelBuilder.Entity<SubType>(entity =>
            {
                entity.ToTable("SubType");

                entity.Property(e => e.SubTypeId).HasColumnName("subTypeId");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.Photopath)
                    .HasMaxLength(500)
                    .HasColumnName("photopath");

                entity.Property(e => e.SubTypeArName)
                    .HasMaxLength(150)
                    .HasColumnName("subTypeArName");

                entity.Property(e => e.SubTypeEnName)
                    .HasMaxLength(150)
                    .HasColumnName("subTypeEnName");

                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.SubTypes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SubType_ItemType");
            });

            modelBuilder.Entity<WarehouseUpdate>(entity =>
            {
                entity.ToTable("WarehouseUpdate");

                entity.Property(e => e.Id)
                    
                    .HasColumnName("id");

                entity.Property(e => e.PalletsDate).HasColumnType("date");

                entity.Property(e => e.ShipmentDate).HasColumnType("date");

                entity.Property(e => e.TeamReceiveDate).HasColumnType("date");
            });
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("Warehouse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.Name)
                    .HasMaxLength(1000)
                    .HasColumnName("name")
                    .IsFixedLength();
            });
            modelBuilder.Entity<ProgressUpdate>(entity =>
            {
                entity.ToTable("ProgressUpdate");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrentProgress).HasColumnName("currentProgress");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.UpdateDate).HasColumnType("date");
            });
            modelBuilder.Entity<ProductionSchedule>(entity =>
            {
                entity.ToTable("ProductionSchedule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddedDate)
                    .HasColumnType("date")
                    .HasColumnName("addedDate");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.IntallationRequestId).HasColumnName("intallationRequestId");

                entity.Property(e => e.IsConfirmed).HasColumnName("isConfirmed");

                entity.Property(e => e.ProductionDate)
                    .HasColumnType("date")
                    .HasColumnName("productionDate");
            });
            modelBuilder.Entity<InstallationUpdate>(entity =>
            {
                entity.ToTable("InstallationUpdate");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.UpdateString).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });
            modelBuilder.Entity<InstallationSchedule>(entity =>
            {
                entity.ToTable("InstallationSchedule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddedDate)
                    .HasColumnType("date")
                    .HasColumnName("addedDate");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.InstallationDate)
                    .HasColumnType("date")
                    .HasColumnName("installationDate");

                entity.Property(e => e.InstallationRequestId).HasColumnName("installationRequestId");

                entity.Property(e => e.IsConfirmed).HasColumnName("isConfirmed");

                entity.Property(e => e.TeamId).HasColumnName("teamId");
            });
            modelBuilder.Entity<InstallationRequest>(entity =>
            {
                entity.ToTable("InstallationRequest");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate).HasColumnType("date");
                entity.Property(e => e.DesiredDate).HasColumnType("date");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.QutationId).HasColumnName("qutationId");
            });








            base.OnModelCreating(modelBuilder);
        }
        
    }
}












//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;

//namespace Nobels.Models
//{
//    public  class db_a8d242_exadecor2Context : IdentityDbContext<Employee, AppRole, int>
//    {
//        public db_a8d242_exadecor2Context()
//        {
//        }

//        public db_a8d242_exadecor2Context(DbContextOptions<db_a8d242_exadecor2Context> options)
//            : base(options)
//        {
//        }

//        public virtual DbSet<Bill> Bills { get; set; } = null!;
//        public virtual DbSet<Branch> Branches { get; set; } = null!;
//        public virtual DbSet<City> Cities { get; set; } = null!;
//        public virtual DbSet<Color> Colors { get; set; } = null!;
//        public virtual DbSet<Customer> Customers { get; set; } = null!;
//        public virtual DbSet<Email> Emails { get; set; } = null!;
//        public virtual DbSet<Employee> Employees { get; set; } = null!;
//        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; } = null!;
//        public virtual DbSet<Item> Items { get; set; } = null!;
//        public virtual DbSet<ItemColor> ItemColors { get; set; } = null!;
//        public virtual DbSet<ItemType> ItemTypes { get; set; } = null!;
//        public virtual DbSet<MainItem> MainItems { get; set; } = null!;
//        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
//        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
//        public virtual DbSet<QoutationUpdate> QoutationUpdates { get; set; } = null!;
//        public virtual DbSet<Quotation> Quotations { get; set; } = null!;
//        public virtual DbSet<QuotationCheck> QuotationChecks { get; set; } = null!;
//        public virtual DbSet<QutationRoom> QutationRooms { get; set; } = null!;
//        public virtual DbSet<RawMaterial> RawMaterials { get; set; } = null!;
//        public virtual DbSet<Setting> Settings { get; set; } = null!;
//        public virtual DbSet<Status> Statuses { get; set; } = null!;
//        public virtual DbSet<SubType> SubTypes { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=SQL5104.site4now.net;Initial Catalog=db_a8d242_exadecor2;User Id=db_a8d242_exadecor2_admin;Password=Let01@2030!");
//            }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Bill>(entity =>
//            {
//                entity.ToTable("Bill");

//                entity.HasIndex(e => e.QuotationId, "IX_Bill_quotationId");

//                entity.Property(e => e.BillId).HasColumnName("billId");

//                entity.Property(e => e.BillDate)
//                    .HasColumnType("datetime")
//                    .HasColumnName("billDate");

//                entity.Property(e => e.BillLanguage)
//                    .HasMaxLength(50)
//                    .HasColumnName("billLanguage");

//                entity.Property(e => e.BillNum)
//                    .HasMaxLength(50)
//                    .HasColumnName("billNum");

//                entity.Property(e => e.BillText)
//                    .HasMaxLength(500)
//                    .HasColumnName("billText");

//                entity.Property(e => e.Discount).HasColumnName("discount");

//                entity.Property(e => e.DiscountType)
//                    .HasMaxLength(50)
//                    .HasColumnName("discountType");

//                entity.Property(e => e.FinalValue)
//                    .HasMaxLength(10)
//                    .HasColumnName("finalValue")
//                    .IsFixedLength();

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(50)
//                    .HasColumnName("notes");

//                entity.Property(e => e.QuotationId).HasColumnName("quotationId");

//                entity.Property(e => e.TotalValue).HasColumnName("totalValue");

//                entity.HasOne(d => d.Quotation)
//                    .WithMany(p => p.Bills)
//                    .HasForeignKey(d => d.QuotationId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Bill_Quotation");
//            });

//            modelBuilder.Entity<Branch>(entity =>
//            {
//                entity.ToTable("Branch");

//                entity.HasIndex(e => e.CityId, "IX_Branch_cityId");

//                entity.Property(e => e.BranchId).HasColumnName("branchId");

//                entity.Property(e => e.Address)
//                    .HasMaxLength(150)
//                    .HasColumnName("address");

//                entity.Property(e => e.BranchName)
//                    .HasMaxLength(50)
//                    .HasColumnName("branchName");

//                entity.Property(e => e.BranchType).HasColumnName("branchType");

//                entity.Property(e => e.CityId).HasColumnName("cityId");

//                entity.Property(e => e.Email)
//                    .HasMaxLength(250)
//                    .HasColumnName("email");

//                entity.Property(e => e.Location)
//                    .HasMaxLength(50)
//                    .HasColumnName("location");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.Phone)
//                    .HasMaxLength(50)
//                    .HasColumnName("phone");

//                entity.HasOne(d => d.City)
//                    .WithMany(p => p.Branches)
//                    .HasForeignKey(d => d.CityId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Branch_City");
//            });

//            modelBuilder.Entity<City>(entity =>
//            {
//                entity.ToTable("City");

//                entity.Property(e => e.CityId).HasColumnName("cityId");

//                entity.Property(e => e.CityName)
//                    .HasMaxLength(50)
//                    .HasColumnName("cityName");
//            });

//            modelBuilder.Entity<Color>(entity =>
//            {
//                entity.Property(e => e.ColorId).HasColumnName("colorId");

//                entity.Property(e => e.ColorCode)
//                    .HasMaxLength(10)
//                    .HasColumnName("colorCode");

//                entity.Property(e => e.ColorName)
//                    .HasMaxLength(150)
//                    .HasColumnName("colorName");

//                entity.Property(e => e.ColorTemplatePhoto)
//                    .HasMaxLength(500)
//                    .HasColumnName("colorTemplatePhoto");

//                entity.Property(e => e.ItemTypeId).HasColumnName("itemTypeId");

//                entity.Property(e => e.Note)
//                    .HasMaxLength(500)
//                    .HasColumnName("note");
//            });

//            modelBuilder.Entity<Customer>(entity =>
//            {
//                entity.ToTable("Customer");

//                entity.HasIndex(e => e.CityId, "IX_Customer_cityId");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.Address)
//                    .HasMaxLength(250)
//                    .HasColumnName("address");

//                entity.Property(e => e.ArabicName)
//                    .HasMaxLength(150)
//                    .HasColumnName("arabicName");

//                entity.Property(e => e.BranchId).HasColumnName("branchId");

//                entity.Property(e => e.CityId).HasColumnName("cityId");

//                entity.Property(e => e.Email)
//                    .HasMaxLength(250)
//                    .HasColumnName("email");

//                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

//                entity.Property(e => e.EnglishName)
//                    .HasMaxLength(150)
//                    .HasColumnName("englishName");

//                entity.Property(e => e.Gender)
//                    .HasMaxLength(50)
//                    .HasColumnName("gender");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.PhonNumber)
//                    .HasMaxLength(20)
//                    .IsFixedLength();

//                entity.Property(e => e.Phone)
//                    .HasMaxLength(20)
//                    .HasColumnName("phone")
//                    .IsFixedLength();

//                entity.Property(e => e.SecondAddress).HasMaxLength(250);
//            });

//            modelBuilder.Entity<Email>(entity =>
//            {
//                entity.Property(e => e.EmailId).HasColumnName("emailId");

//                entity.Property(e => e.EmailText)
//                    .HasMaxLength(500)
//                    .HasColumnName("emailText");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.Receivers)
//                    .HasMaxLength(500)
//                    .HasColumnName("receivers");

//                entity.Property(e => e.Senddate)
//                    .HasColumnType("datetime")
//                    .HasColumnName("senddate");
//            });

//            modelBuilder.Entity<Employee>(entity =>
//            {
//                entity.ToTable("Employee");

//                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

//                entity.Property(e => e.BranchId).HasColumnName("branchId");

//                entity.Property(e => e.Email)
//                    .HasMaxLength(250)
//                    .HasColumnName("email");

//                entity.Property(e => e.EmployeeNumber)
//                    .HasMaxLength(50)
//                    .HasColumnName("employeeNumber");

//                entity.Property(e => e.FirstName)
//                    .HasMaxLength(50)
//                    .HasColumnName("firstName");

//                entity.Property(e => e.LastName).HasMaxLength(50);

//                entity.Property(e => e.Password)
//                    .HasMaxLength(500)
//                    .HasColumnName("password");

//                entity.Property(e => e.Phone)
//                    .HasMaxLength(20)
//                    .HasColumnName("phone");

//                entity.Property(e => e.RegisterDate)
//                    .HasColumnType("date")
//                    .HasColumnName("registerDate");

//                entity.Property(e => e.RoleId).HasColumnName("roleId");

//                entity.Property(e => e.UserName)
//                    .HasMaxLength(50)
//                    .HasColumnName("userName");

//                entity.HasOne(d => d.Branch)
//                    .WithMany(p => p.Employees)
//                    .HasForeignKey(d => d.BranchId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Employee_Branch");

//                entity.HasOne(d => d.Role)
//                    .WithMany(p => p.Employees)
//                    .HasForeignKey(d => d.RoleId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Employee_EmployeeRoles");
//            });

//            modelBuilder.Entity<EmployeeRole>(entity =>
//            {
//                entity.HasKey(e => e.RoleId);

//                entity.Property(e => e.RoleId).HasColumnName("roleId");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(200)
//                    .HasColumnName("notes");

//                entity.Property(e => e.RoleName)
//                    .HasMaxLength(50)
//                    .HasColumnName("roleName");
//            });

//            modelBuilder.Entity<Item>(entity =>
//            {
//                entity.ToTable("Item");

//                entity.HasIndex(e => e.ItemSubTypeId, "IX_Item_itemSubTypeId");

//                entity.Property(e => e.ItemId).HasColumnName("itemId");

//                entity.Property(e => e.ItemArName)
//                    .HasMaxLength(150)
//                    .HasColumnName("itemArName");

//                entity.Property(e => e.ItemCode)
//                    .HasMaxLength(20)
//                    .HasColumnName("itemCode");

//                entity.Property(e => e.ItemCurrentPrice).HasColumnName("itemCurrentPrice");

//                entity.Property(e => e.ItemEnName)
//                    .HasMaxLength(150)
//                    .HasColumnName("itemEnName");

//                entity.Property(e => e.ItemPhotoPath)
//                    .HasMaxLength(500)
//                    .HasColumnName("itemPhotoPath");

//                entity.Property(e => e.ItemSubTypeId).HasColumnName("itemSubTypeId");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.HasOne(d => d.ItemSubType)
//                    .WithMany(p => p.Items)
//                    .HasForeignKey(d => d.ItemSubTypeId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Item_SubType");
//            });

//            modelBuilder.Entity<ItemColor>(entity =>
//            {
//                entity.ToTable("ItemColor");

//                entity.HasIndex(e => e.ColorId, "IX_ItemColor_ColorId");

//                entity.HasIndex(e => e.ItemId, "IX_ItemColor_itemId");

//                entity.Property(e => e.ItemColorId).HasColumnName("itemColorId");

//                entity.Property(e => e.IsEnabled).HasColumnName("isEnabled");

//                entity.Property(e => e.ItemId).HasColumnName("itemId");

//                entity.Property(e => e.SpecialPrice).HasColumnName("specialPrice");

//                entity.HasOne(d => d.Color)
//                    .WithMany(p => p.ItemColors)
//                    .HasForeignKey(d => d.ColorId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_ItemColor_Colors");

//                entity.HasOne(d => d.Item)
//                    .WithMany(p => p.ItemColors)
//                    .HasForeignKey(d => d.ItemId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_ItemColor_Item");
//            });

//            modelBuilder.Entity<ItemType>(entity =>
//            {
//                entity.HasKey(e => e.TypeId);

//                entity.ToTable("ItemType");

//                entity.Property(e => e.TypeId).HasColumnName("typeId");

//                entity.Property(e => e.Photopath)
//                    .HasMaxLength(250)
//                    .HasColumnName("photopath");

//                entity.Property(e => e.TypeArName)
//                    .HasMaxLength(150)
//                    .HasColumnName("typeArName");

//                entity.Property(e => e.TypeEnName)
//                    .HasMaxLength(150)
//                    .HasColumnName("typeEnName");
//            });

//            modelBuilder.Entity<MainItem>(entity =>
//            {
//                entity.ToTable("MainItem");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.MainItemEnName)
//                    .HasMaxLength(1000)
//                    .HasColumnName("mainItemEnName");

//                entity.Property(e => e.MainItemName)
//                    .HasMaxLength(1000)
//                    .HasColumnName("mainItemName");

//                entity.Property(e => e.MainItemPhoto)
//                    .HasMaxLength(500)
//                    .HasColumnName("mainItemPhoto");
//            });

//            modelBuilder.Entity<OrderDetail>(entity =>
//            {
//                entity.HasKey(e => e.DetailId);

//                entity.ToTable("OrderDetail");

//                entity.HasIndex(e => e.ItemId, "IX_OrderDetail_itemId");

//                entity.HasIndex(e => e.QoutationId, "IX_OrderDetail_qoutationId");

//                entity.Property(e => e.DetailId).HasColumnName("detailId");

//                entity.Property(e => e.Colorname)
//                    .HasMaxLength(50)
//                    .HasColumnName("colorname");

//                entity.Property(e => e.Discount).HasColumnType("decimal(12, 2)");

//                entity.Property(e => e.DiscountType).HasMaxLength(50);

//                entity.Property(e => e.ItemId).HasColumnName("itemId");

//                entity.Property(e => e.MainItemId).HasColumnName("mainItemId");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.Price)
//                    .HasColumnType("decimal(12, 2)")
//                    .HasColumnName("price");

//                entity.Property(e => e.QoutationId).HasColumnName("qoutationId");

//                entity.Property(e => e.Quantity)
//                    .HasColumnType("decimal(12, 2)")
//                    .HasColumnName("quantity");

//                entity.Property(e => e.RoomId).HasColumnName("roomId");

//                entity.Property(e => e.TotalValue).HasColumnType("decimal(12, 2)");

//                entity.HasOne(d => d.Item)
//                    .WithMany(p => p.OrderDetails)
//                    .HasForeignKey(d => d.ItemId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_OrderDetail_Item");

//                entity.HasOne(d => d.MainItem)
//                    .WithMany(p => p.OrderDetails)
//                    .HasForeignKey(d => d.MainItemId)
//                    .HasConstraintName("FK_OrderDetail_MainItem");

//                entity.HasOne(d => d.Qoutation)
//                    .WithMany(p => p.OrderDetails)
//                    .HasForeignKey(d => d.QoutationId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_OrderDetail_Quotation");
//            });

//            modelBuilder.Entity<OrderStatus>(entity =>
//            {
//                entity.ToTable("OrderStatus");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.Comment).HasMaxLength(250);

//                entity.Property(e => e.UpdateDate).HasColumnType("date");

//                entity.HasOne(d => d.Status)
//                    .WithMany(p => p.OrderStatuses)
//                    .HasForeignKey(d => d.StatusId)
//                    .HasConstraintName("FK_OrderStatus_OrderStatus");
//            });

//            modelBuilder.Entity<QoutationUpdate>(entity =>
//            {
//                entity.HasKey(e => e.UpdateId);

//                entity.HasIndex(e => e.EmployeeId, "IX_QoutationUpdates_employeeId");

//                entity.HasIndex(e => e.QoutationId, "IX_QoutationUpdates_qoutationId");

//                entity.Property(e => e.UpdateId).HasColumnName("updateId");

//                entity.Property(e => e.ChangeDate)
//                    .HasColumnType("datetime")
//                    .HasColumnName("changeDate");

//                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

//                entity.Property(e => e.QoutationId).HasColumnName("qoutationId");

//                entity.Property(e => e.Updates)
//                    .HasMaxLength(500)
//                    .HasColumnName("updates");

//                entity.HasOne(d => d.Employee)
//                    .WithMany(p => p.QoutationUpdates)
//                    .HasForeignKey(d => d.EmployeeId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_QoutationUpdates_Employee");

//                entity.HasOne(d => d.Qoutation)
//                    .WithMany(p => p.QoutationUpdates)
//                    .HasForeignKey(d => d.QoutationId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_QoutationUpdates_Quotation");
//            });

//            modelBuilder.Entity<Quotation>(entity =>
//            {
//                entity.ToTable("Quotation");

//                entity.HasIndex(e => e.BranchId, "IX_Quotation_branchId");

//                entity.HasIndex(e => e.CustomerId, "IX_Quotation_customerId");

//                entity.Property(e => e.QuotationId).HasColumnName("quotationId");

//                entity.Property(e => e.BranchId).HasColumnName("branchId");

//                entity.Property(e => e.Code)
//                    .HasMaxLength(100)
//                    .HasColumnName("code");

//                entity.Property(e => e.CustomerId).HasColumnName("customerId");

//                entity.Property(e => e.Discount).HasColumnName("discount");

//                entity.Property(e => e.DiscountType)
//                    .HasMaxLength(50)
//                    .HasColumnName("discountType");

//                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

//                entity.Property(e => e.Enddate)
//                    .HasColumnType("date")
//                    .HasColumnName("enddate");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.QuotationDate)
//                    .HasColumnType("date")
//                    .HasColumnName("quotationDate")
//                    .HasComputedColumnSql("(CONVERT([date],[quotationSimpleDate]))", false);

//                entity.Property(e => e.QuotationSimpleDate)
//                    .HasColumnType("datetime")
//                    .HasColumnName("quotationSimpleDate");

//                entity.Property(e => e.QuotationStatus)
//                    .HasMaxLength(50)
//                    .HasColumnName("quotationStatus");

//                entity.HasOne(d => d.Customer)
//                    .WithMany(p => p.Quotations)
//                    .HasForeignKey(d => d.CustomerId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_Quotation_Customer");
//            });

//            modelBuilder.Entity<QuotationCheck>(entity =>
//            {
//                entity.ToTable("QuotationCheck");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.Checkdate)
//                    .HasColumnType("date")
//                    .HasColumnName("checkdate");

//                entity.Property(e => e.Employeeid).HasColumnName("employeeid");

//                entity.Property(e => e.Isok).HasColumnName("isok");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.Quotationid).HasColumnName("quotationid");

//                entity.HasOne(d => d.Employee)
//                    .WithMany(p => p.QuotationChecks)
//                    .HasForeignKey(d => d.Employeeid)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_QuotationCheck_Employee");

//                entity.HasOne(d => d.Quotation)
//                    .WithMany(p => p.QuotationChecks)
//                    .HasForeignKey(d => d.Quotationid)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_QuotationCheck_Quotation");
//            });

//            modelBuilder.Entity<QutationRoom>(entity =>
//            {
//                entity.ToTable("QutationRoom");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.Discount).HasColumnName("discount");

//                entity.Property(e => e.DiscountType)
//                    .HasMaxLength(50)
//                    .HasColumnName("discountType");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(50)
//                    .HasColumnName("notes");

//                entity.Property(e => e.QutationId).HasColumnName("qutationId");

//                entity.Property(e => e.RoomName)
//                    .HasMaxLength(50)
//                    .HasColumnName("roomName");
//            });

//            modelBuilder.Entity<RawMaterial>(entity =>
//            {
//                entity.ToTable("RawMaterial");

//                entity.Property(e => e.Id)
//                    .ValueGeneratedNever()
//                    .HasColumnName("id");

//                entity.Property(e => e.Cost).HasColumnName("cost");

//                entity.Property(e => e.DamagedPercent).HasColumnName("damagedPercent");

//                entity.Property(e => e.MeasurUnit)
//                    .HasMaxLength(50)
//                    .HasColumnName("measurUnit");

//                entity.Property(e => e.Name)
//                    .HasMaxLength(150)
//                    .HasColumnName("name");

//                entity.Property(e => e.Price).HasColumnName("price");
//            });

//            modelBuilder.Entity<Setting>(entity =>
//            {
//                entity.ToTable("Setting");

//                entity.Property(e => e.Id)
//                    .ValueGeneratedNever()
//                    .HasColumnName("id");

//                entity.Property(e => e.Tax)
//                    .HasColumnType("decimal(12, 2)")
//                    .HasColumnName("tax");
//            });

//            modelBuilder.Entity<Status>(entity =>
//            {
//                entity.ToTable("Status");

//                entity.Property(e => e.Id).HasColumnName("id");

//                entity.Property(e => e.Name)
//                    .HasMaxLength(150)
//                    .HasColumnName("name");

//                entity.Property(e => e.Note)
//                    .HasMaxLength(150)
//                    .HasColumnName("note");
//            });

//            modelBuilder.Entity<SubType>(entity =>
//            {
//                entity.ToTable("SubType");

//                entity.HasIndex(e => e.TypeId, "IX_SubType_typeId");

//                entity.Property(e => e.SubTypeId).HasColumnName("subTypeId");

//                entity.Property(e => e.Notes)
//                    .HasMaxLength(500)
//                    .HasColumnName("notes");

//                entity.Property(e => e.Photopath)
//                    .HasMaxLength(500)
//                    .HasColumnName("photopath");

//                entity.Property(e => e.SubTypeArName)
//                    .HasMaxLength(150)
//                    .HasColumnName("subTypeArName");

//                entity.Property(e => e.SubTypeEnName)
//                    .HasMaxLength(150)
//                    .HasColumnName("subTypeEnName");

//                entity.Property(e => e.TypeId).HasColumnName("typeId");

//                entity.HasOne(d => d.Type)
//                    .WithMany(p => p.SubTypes)
//                    .HasForeignKey(d => d.TypeId)
//                    .OnDelete(DeleteBehavior.Cascade)
//                    .HasConstraintName("FK_SubType_ItemType");
//            });

//            base.OnModelCreating(modelBuilder);
//        }

//    }
//}
