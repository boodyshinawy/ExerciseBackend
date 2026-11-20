using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class DropReturnReasonFromDetailAndUpdateFirmCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop return_reason_from_sub_commite from seizure_request_detail
            migrationBuilder.Sql("ALTER TABLE seizure_request_detail DROP COLUMN IF EXISTS return_reason_from_sub_commite;");
            migrationBuilder.Sql("ALTER TABLE seizure_request_detail DROP COLUMN IF EXISTS return_reason_from_subcommite;");
            migrationBuilder.Sql("ALTER TABLE seizure_request_detail DROP COLUMN IF EXISTS return_reason_from_permanent_commite;");

            // Ensure firm schema has created_date and is_deleted
            migrationBuilder.Sql("ALTER TABLE firm ADD COLUMN IF NOT EXISTS created_date timestamp with time zone;");
            migrationBuilder.Sql("ALTER TABLE firm ADD COLUMN IF NOT EXISTS is_deleted boolean NOT NULL DEFAULT false;");

            // Ensure seizure_request has case_number and mahdr_number
            migrationBuilder.Sql("ALTER TABLE seizure_request ADD COLUMN IF NOT EXISTS case_number character varying(100);");
            migrationBuilder.Sql("ALTER TABLE seizure_request ADD COLUMN IF NOT EXISTS mahdr_number character varying(100);");

            // Populate created_date with current timestamp in firm table for existing records (one-time update, no column default)
            migrationBuilder.Sql("UPDATE firm SET created_date = CURRENT_TIMESTAMP WHERE created_date IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "return_reason_from_sub_commite",
                table: "seizure_request_detail",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
