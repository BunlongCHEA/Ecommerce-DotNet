using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameToTotalFinalAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CouponUserLists_CouponUserListId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AmountAfterDiscount",
                table: "Orders",
                newName: "TotalFinalAmount");

            migrationBuilder.AlterColumn<int>(
                name: "CouponUserListId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CouponUserLists_CouponUserListId",
                table: "Orders",
                column: "CouponUserListId",
                principalTable: "CouponUserLists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CouponUserLists_CouponUserListId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TotalFinalAmount",
                table: "Orders",
                newName: "AmountAfterDiscount");

            migrationBuilder.AlterColumn<int>(
                name: "CouponUserListId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CouponUserLists_CouponUserListId",
                table: "Orders",
                column: "CouponUserListId",
                principalTable: "CouponUserLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
