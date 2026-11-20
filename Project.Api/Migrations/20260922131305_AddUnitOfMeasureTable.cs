using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitOfMeasureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "unit_of_measure_id",
                table: "seizure_request_detail",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "unit_of_measure",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_unit_of_measure", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_seizure_request_detail_unit_of_measure_id",
                table: "seizure_request_detail",
                column: "unit_of_measure_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedUnitOfMeasureIndex",
                table: "unit_of_measure",
                column: "is_deleted");

            migrationBuilder.AddForeignKey(
                name: "fk_seizure_request_detail_unit_of_measure_unit_of_measure_id",
                table: "seizure_request_detail",
                column: "unit_of_measure_id",
                principalTable: "unit_of_measure",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                INSERT INTO unit_of_measure (id, name, is_deleted) VALUES
                (1, 'كجم', false),
                (2, 'جرام', false),
                (3, 'طن', false),
                (4, 'لتر', false),
                (5, 'مليلتر', false),
                (6, 'متر', false),
                (7, 'سم', false),
                (8, 'قطعة', false),
                (9, 'كرتونة', false),
                (10, 'صندوق', false),
                (11, 'شيكارة', false),
                (12, 'باكو', false),
                (13, 'عبوة', false),
                (14, 'طرد', false),
                (15, 'برميل', false),
                (16, 'جركن', false),
                (17, 'لفة', false),
                (18, 'زوج', false),
                (19, 'رأس', false),
                (20, 'اسطوانة', false)
                ON CONFLICT (id) DO NOTHING;

                SELECT setval(
                    pg_get_serial_sequence('unit_of_measure', 'id'),
                    COALESCE((SELECT MAX(id) FROM unit_of_measure), 1));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_seizure_request_detail_unit_of_measure_unit_of_measure_id",
                table: "seizure_request_detail");

            migrationBuilder.DropTable(
                name: "unit_of_measure");

            migrationBuilder.DropIndex(
                name: "ix_seizure_request_detail_unit_of_measure_id",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "unit_of_measure_id",
                table: "seizure_request_detail");
        }
    }
}
