using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreXF.Store.Data.Migrations
{
    public partial class X1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CoreXF");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "CoreXF",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(1,1)", nullable: true, defaultValue: 0m),
                    ProjectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Downloads = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductQuestionsAndAnswers",
                schema: "CoreXF",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AskedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AskedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnsweredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductQuestionsAndAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductQuestionsAndAnswers_AspNetUsers_AskedById",
                        column: x => x.AskedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductQuestionsAndAnswers_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "CoreXF",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductRatings",
                schema: "CoreXF",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<long>(type: "bigint", nullable: false),
                    RatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRatings_AspNetUsers_RatedById",
                        column: x => x.RatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductRatings_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "CoreXF",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsMetaData",
                schema: "CoreXF",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0m),
                    MetaDataType = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsMetaData_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "CoreXF",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuestionsAndAnswers_AskedById",
                schema: "CoreXF",
                table: "ProductQuestionsAndAnswers",
                column: "AskedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuestionsAndAnswers_ProductId",
                schema: "CoreXF",
                table: "ProductQuestionsAndAnswers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatings_ProductId",
                schema: "CoreXF",
                table: "ProductRatings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatings_RatedById",
                schema: "CoreXF",
                table: "ProductRatings",
                column: "RatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OwnerId",
                schema: "CoreXF",
                table: "Products",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsMetaData_ProductId",
                schema: "CoreXF",
                table: "ProductsMetaData",
                column: "ProductId",
                unique: true,
                filter: "[ProductId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductQuestionsAndAnswers",
                schema: "CoreXF");

            migrationBuilder.DropTable(
                name: "ProductRatings",
                schema: "CoreXF");

            migrationBuilder.DropTable(
                name: "ProductsMetaData",
                schema: "CoreXF");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "CoreXF");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegisteredOn",
                table: "AspNetUsers");
        }
    }
}
