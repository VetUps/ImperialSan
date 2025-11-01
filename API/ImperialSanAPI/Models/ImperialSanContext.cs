using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ImperialSanAPI.Models;

public partial class ImperialSanContext : DbContext
{
    public ImperialSanContext()
    {
    }

    public ImperialSanContext(DbContextOptions<ImperialSanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketPosition> BasketPositions { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderPosition> OrderPositions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=1234;database=imperial_san", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.BasketId).HasName("PRIMARY");

            entity.ToTable("basket");

            entity.HasIndex(e => e.UserId, "user_basket_fk_idx");

            entity.Property(e => e.BasketId).HasColumnName("basket_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_basket_fk");
        });

        modelBuilder.Entity<BasketPosition>(entity =>
        {
            entity.HasKey(e => e.BasketPositionId).HasName("PRIMARY");

            entity.ToTable("basket_position");

            entity.HasIndex(e => e.ProductId, "basket_position_product_fk");

            entity.HasIndex(e => e.BasketId, "basket_to_position_fk_idx");

            entity.Property(e => e.BasketPositionId).HasColumnName("basket_position_id");
            entity.Property(e => e.BasketId).HasColumnName("basket_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductQuantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("product_quantity");

            entity.HasOne(d => d.Basket).WithMany(p => p.BasketPositions)
                .HasForeignKey(d => d.BasketId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("basket_to_position_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.BasketPositions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("basket_position_product_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category");

            entity.HasIndex(e => e.ParenCategoryId, "parent_category_fk_idx");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryDescription)
                .HasColumnType("text")
                .HasColumnName("category_description");
            entity.Property(e => e.CategoryTitle)
                .HasMaxLength(60)
                .HasColumnName("category_title");
            entity.Property(e => e.ParenCategoryId).HasColumnName("paren_category_id");

            entity.HasOne(d => d.ParenCategory).WithMany(p => p.InverseParenCategory)
                .HasForeignKey(d => d.ParenCategoryId)
                .HasConstraintName("parent_category_fk");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.UserId, "order_user_fk_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.DateOfCreate)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("date_of_create");
            entity.Property(e => e.DiliveryAddres)
                .HasColumnType("text")
                .HasColumnName("dilivery_addres");
            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("'В обработке'")
                .HasColumnType("enum('В обработке','Собиарется','Собран','В пути','Доставлен','Отменён')")
                .HasColumnName("order_status");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("enum('Онлайн','Наличными')")
                .HasColumnName("payment_method");
            entity.Property(e => e.Price)
                .HasColumnType("float unsigned")
                .HasColumnName("price");
            entity.Property(e => e.UserComment)
                .HasColumnType("text")
                .HasColumnName("user_comment");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("order_user_fk");
        });

        modelBuilder.Entity<OrderPosition>(entity =>
        {
            entity.HasKey(e => e.OrderPositionId).HasName("PRIMARY");

            entity.ToTable("order_position");

            entity.HasIndex(e => e.OrderId, "order_to_position");

            entity.HasIndex(e => e.ProductId, "product_order_position_fk");

            entity.Property(e => e.OrderPositionId).HasColumnName("order_position_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductPriceInMoment)
                .HasColumnType("float unsigned")
                .HasColumnName("product_price_in_moment");
            entity.Property(e => e.ProductQuantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("product_quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPositions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_to_position");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderPositions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_order_position_fk");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.CategoryId, "category_fk_idx");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.BrandTitle)
                .HasMaxLength(50)
                .HasColumnName("brand_title");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.DateOfCreate)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("date_of_create");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasDefaultValue("https://forums.ea.com/t5/s/tghpe58374/images/bS00ODUxNjExLTFpRUFCRDZCNEE4QTVBQjVEOQ?revision=1")
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasColumnType("float unsigned")
                .HasColumnName("price");
            entity.Property(e => e.ProductDescription)
                .HasColumnType("text")
                .HasColumnName("product_description");
            entity.Property(e => e.ProductTitle)
                .HasColumnType("text")
                .HasColumnName("product_title");
            entity.Property(e => e.QuantityInStock).HasColumnName("quantity_in_stock");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.UserMail, "user_mail_UNIQUE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DiliveryAddress)
                .HasColumnType("text")
                .HasColumnName("dilivery_address");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("blob")
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasDefaultValueSql("'User'")
                .HasColumnName("role");
            entity.Property(e => e.UserMail)
                .HasMaxLength(100)
                .HasColumnName("user_mail");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPatronymic)
                .HasMaxLength(100)
                .HasColumnName("user_patronymic");
            entity.Property(e => e.UserPhone)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("user_phone");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(100)
                .HasColumnName("user_surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
