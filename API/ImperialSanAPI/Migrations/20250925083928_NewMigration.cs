using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImperialSanAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    paren_category_id = table.Column<int>(type: "int", nullable: true),
                    category_title = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.category_id);
                    table.ForeignKey(
                        name: "parent_category_fk",
                        column: x => x.paren_category_id,
                        principalTable: "category",
                        principalColumn: "category_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_mail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<byte[]>(type: "blob", nullable: false),
                    user_surname = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_patronymic = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_phone = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dilivery_address = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValueSql: "'User'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.user_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    product_title = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    product_description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<float>(type: "float unsigned", nullable: false),
                    quantity_in_stock = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    brand_title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_of_create = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "curdate()"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.product_id);
                    table.ForeignKey(
                        name: "category_fk",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "basket",
                columns: table => new
                {
                    basket_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.basket_id);
                    table.ForeignKey(
                        name: "user_basket_fk",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    date_of_create = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "curdate()"),
                    order_status = table.Column<string>(type: "enum('В обработке','Собиарется','Собран','В пути','Доставлен','Отменён')", nullable: true, defaultValueSql: "'В обработке'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dilivery_addres = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_method = table.Column<string>(type: "enum('Онлайн','Наличными')", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<float>(type: "float unsigned", nullable: false),
                    user_comment = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.order_id);
                    table.ForeignKey(
                        name: "order_user_fk",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "basket_position",
                columns: table => new
                {
                    basket_position_id = table.Column<int>(type: "int", nullable: false),
                    basket_id = table.Column<int>(type: "int", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    product_quantity = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.basket_position_id);
                    table.ForeignKey(
                        name: "basket_position_product_fk",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "basket_to_position_fk",
                        column: x => x.basket_id,
                        principalTable: "basket",
                        principalColumn: "basket_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "order_position",
                columns: table => new
                {
                    order_position_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    product_quantity = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'1'"),
                    product_price_in_moment = table.Column<float>(type: "float unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.order_position_id);
                    table.ForeignKey(
                        name: "order_to_position",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "product_order_position_fk",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "user_basket_fk_idx",
                table: "basket",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "basket_position_product_fk_idx",
                table: "basket_position",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "basket_to_position_fk_idx",
                table: "basket_position",
                column: "basket_id");

            migrationBuilder.CreateIndex(
                name: "parent_category_fk_idx",
                table: "category",
                column: "paren_category_id");

            migrationBuilder.CreateIndex(
                name: "order_user_fk_idx",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "order_to_position",
                table: "order_position",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "product_order_position_fk",
                table: "order_position",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "category_fk_idx",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "user_mail_UNIQUE",
                table: "user",
                column: "user_mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basket_position");

            migrationBuilder.DropTable(
                name: "order_position");

            migrationBuilder.DropTable(
                name: "basket");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
