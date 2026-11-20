using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeizureItemFieldsAndNewStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "remaining_quantity",
                table: "seizure_request_detail",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "seizure_request_detail",
                type: "numeric",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "item_name",
                table: "seizure_request_detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "main_category",
                table: "seizure_request_detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sub_category",
                table: "seizure_request_detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "seizure_request_detail",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "allocated_quantity",
                table: "lot_item_allocation",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "item_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "main_category",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "sub_category",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "seizure_request_detail");

            migrationBuilder.AlterColumn<int>(
                name: "remaining_quantity",
                table: "seizure_request_detail",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "seizure_request_detail",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 1m);

            migrationBuilder.AlterColumn<int>(
                name: "allocated_quantity",
                table: "lot_item_allocation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
