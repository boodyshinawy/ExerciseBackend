using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFirmSchemaAndNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_firm_firm_temp_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "second_commander_code",
                table: "firm",
                newName: "second_commander_id");

            migrationBuilder.RenameColumn(
                name: "parent_firm_code",
                table: "firm",
                newName: "parent_firm_id");

            migrationBuilder.RenameColumn(
                name: "comander_code",
                table: "firm",
                newName: "commander_id");

            migrationBuilder.RenameColumn(
                name: "firm_code",
                table: "firm",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_firm_firm_id",
                table: "users",
                column: "firm_id",
                principalTable: "firm",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_firm_firm_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "second_commander_id",
                table: "firm",
                newName: "second_commander_code");

            migrationBuilder.RenameColumn(
                name: "parent_firm_id",
                table: "firm",
                newName: "parent_firm_code");

            migrationBuilder.RenameColumn(
                name: "commander_id",
                table: "firm",
                newName: "comander_code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "firm",
                newName: "firm_code");

            migrationBuilder.AddForeignKey(
                name: "fk_users_firm_firm_temp_id",
                table: "users",
                column: "firm_id",
                principalTable: "firm",
                principalColumn: "firm_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
