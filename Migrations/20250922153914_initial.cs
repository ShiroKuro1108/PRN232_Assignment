using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace assignment.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Image", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(6938), "Comfortable cotton t-shirt perfect for everyday wear", "https://example.com/tshirt.jpg", "Classic T-Shirt", 19.99m, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(7150) },
                    { 2, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(7301), "High-quality denim jeans with a modern fit", "https://example.com/jeans.jpg", "Denim Jeans", 79.99m, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(7301) },
                    { 3, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(7303), "Light and airy summer dress perfect for warm weather", "https://example.com/dress.jpg", "Summer Dress", 49.99m, new DateTime(2025, 9, 22, 15, 39, 13, 197, DateTimeKind.Utc).AddTicks(7304) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
