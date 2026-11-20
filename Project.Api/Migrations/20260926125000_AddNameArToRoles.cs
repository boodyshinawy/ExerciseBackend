using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNameArToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name_ar",
                table: "roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE roles SET name_ar = 'مدير' WHERE id = '1';
                UPDATE roles SET name_ar = 'مكتب حرس حدود' WHERE id = '2';
                UPDATE roles SET name_ar = 'لجنة فرعية' WHERE id = '3';
                UPDATE roles SET name_ar = 'لجنة دائمة' WHERE id = '4';
                UPDATE roles SET name_ar = 'جهاز الخدمات العامة' WHERE id = '5';
                UPDATE roles SET name_ar = 'هيئة الشئون المالية' WHERE id = '6';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name_ar",
                table: "roles");
        }
    }
}
