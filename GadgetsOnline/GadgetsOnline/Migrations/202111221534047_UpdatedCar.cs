using Microsoft.EntityFrameworkCore.Migrations;

namespace GadgetsOnline.Migrations
{
    public partial class UpdatedCar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Products_Product_ProductId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_Product_ProductId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "Product_ProductId",
                table: "Carts",
                newName: "ProductId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductId",
                table: "Carts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Products_ProductId",
                table: "Carts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
                name: "PizzaId",
                table: "Carts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PizzaId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Products_ProductId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductId",
                table: "Carts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Carts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Carts",
                newName: "Product_ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Product_ProductId",
                table: "Carts",
                column: "Product_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Products_Product_ProductId",
                table: "Carts",
                column: "Product_ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }
    }
}
