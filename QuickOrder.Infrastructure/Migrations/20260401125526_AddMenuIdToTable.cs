using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuIdToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_MenuId",
                table: "Tables",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Menus_MenuId",
                table: "Tables",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Menus_MenuId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_MenuId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Tables");
        }
    }
}
