using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublicServiceFlagInSaleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE sale_entity SET is_public_service = TRUE WHERE name = 'جهاز الخدمات العامة';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE sale_entity SET is_public_service = FALSE WHERE name = 'جهاز الخدمات العامة';");
        }
    }
}
