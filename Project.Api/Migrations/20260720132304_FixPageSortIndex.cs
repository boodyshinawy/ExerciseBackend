using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPageSortIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_page_parent_id",
                table: "page");

            migrationBuilder.DropIndex(
                name: "PageSortNumberIndex",
                table: "page");

            migrationBuilder.CreateIndex(
                name: "ix_page_parent_id_sort_number",
                table: "page",
                columns: new[] { "parent_id", "sort_number" },
                unique: true,
                filter: "parent_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_page_sort_number",
                table: "page",
                column: "sort_number",
                unique: true,
                filter: "parent_id IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_page_parent_id_sort_number",
                table: "page");

            migrationBuilder.DropIndex(
                name: "ix_page_sort_number",
                table: "page");

            migrationBuilder.CreateIndex(
                name: "ix_page_parent_id",
                table: "page",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "PageSortNumberIndex",
                table: "page",
                column: "sort_number",
                unique: true);
        }
    }
}
