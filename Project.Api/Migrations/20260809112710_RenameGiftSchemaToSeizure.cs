using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameGiftSchemaToSeizure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_gift_request_firm_firm_id",
                table: "gift_request");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_request_detail_gift_request_gift_request_id",
                table: "gift_request_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_request_detail_item_item_id",
                table: "gift_request_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_request_detail_status_status_id",
                table: "gift_request_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_request_detail_attachment_gift_request_detail_gift_request_",
                table: "request_detail_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_gift_request_detail",
                table: "gift_request_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_gift_request",
                table: "gift_request");

            migrationBuilder.RenameTable(
                name: "gift_request_detail",
                newName: "seizure_request_detail");

            migrationBuilder.RenameTable(
                name: "gift_request",
                newName: "seizure_request");

            migrationBuilder.RenameColumn(
                name: "gift_request_detail_id",
                table: "request_detail_attachment",
                newName: "seizure_request_detail_id");

            migrationBuilder.RenameIndex(
                name: "ix_request_detail_attachment_gift_request_detail_id",
                table: "request_detail_attachment",
                newName: "ix_request_detail_attachment_seizure_request_detail_id");

            migrationBuilder.RenameColumn(
                name: "why_gift",
                table: "seizure_request_detail",
                newName: "why_seizure");

            migrationBuilder.RenameColumn(
                name: "gift_value",
                table: "seizure_request_detail",
                newName: "seizure_value");

            migrationBuilder.RenameColumn(
                name: "gift_request_id",
                table: "seizure_request_detail",
                newName: "seizure_request_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_request_detail_status_id",
                table: "seizure_request_detail",
                newName: "ix_seizure_request_detail_status_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_request_detail_item_id",
                table: "seizure_request_detail",
                newName: "ix_seizure_request_detail_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_request_detail_gift_request_id",
                table: "seizure_request_detail",
                newName: "ix_seizure_request_detail_seizure_request_id");

            migrationBuilder.RenameIndex(
                name: "IsDeletedGiftRequestDetailIndex",
                table: "seizure_request_detail",
                newName: "IsDeletedSeizureRequestDetailIndex");

            migrationBuilder.RenameIndex(
                name: "ix_gift_request_firm_id",
                table: "seizure_request",
                newName: "ix_seizure_request_firm_id");

            migrationBuilder.RenameIndex(
                name: "IsDeletedGiftRequestIndex",
                table: "seizure_request",
                newName: "IsDeletedSeizureRequestIndex");

            migrationBuilder.AddPrimaryKey(
                name: "pk_seizure_request_detail",
                table: "seizure_request_detail",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_seizure_request",
                table: "seizure_request",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_detail_attachment_seizure_request_detail_seizure_re",
                table: "request_detail_attachment",
                column: "seizure_request_detail_id",
                principalTable: "seizure_request_detail",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_seizure_request_firm_firm_id",
                table: "seizure_request",
                column: "firm_id",
                principalTable: "firm",
                principalColumn: "firm_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_seizure_request_detail_item_item_id",
                table: "seizure_request_detail",
                column: "item_id",
                principalTable: "item",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_seizure_request_detail_seizure_request_seizure_request_id",
                table: "seizure_request_detail",
                column: "seizure_request_id",
                principalTable: "seizure_request",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_seizure_request_detail_status_status_id",
                table: "seizure_request_detail",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // Preserve attachment files after folder rename: update stored absolute paths.
            migrationBuilder.Sql("""
                UPDATE request_detail_attachment
                SET file_path = REPLACE(file_path, 'GIFTS', 'SEIZURES')
                WHERE POSITION('GIFTS' IN file_path) > 0;
                """);

            // Update seeded menu routes so sidebar matches /seizure-* frontend paths.
            migrationBuilder.Sql("""
                UPDATE page
                SET "to" = REPLACE("to", '/gift-', '/seizure-')
                WHERE "to" LIKE '/gift-%';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_detail_attachment_seizure_request_detail_seizure_re",
                table: "request_detail_attachment");

            migrationBuilder.DropForeignKey(
                name: "fk_seizure_request_firm_firm_id",
                table: "seizure_request");

            migrationBuilder.DropForeignKey(
                name: "fk_seizure_request_detail_item_item_id",
                table: "seizure_request_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_seizure_request_detail_seizure_request_seizure_request_id",
                table: "seizure_request_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_seizure_request_detail_status_status_id",
                table: "seizure_request_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_seizure_request_detail",
                table: "seizure_request_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_seizure_request",
                table: "seizure_request");

            migrationBuilder.RenameTable(
                name: "seizure_request_detail",
                newName: "gift_request_detail");

            migrationBuilder.RenameTable(
                name: "seizure_request",
                newName: "gift_request");

            migrationBuilder.RenameColumn(
                name: "seizure_request_detail_id",
                table: "request_detail_attachment",
                newName: "gift_request_detail_id");

            migrationBuilder.RenameIndex(
                name: "ix_request_detail_attachment_seizure_request_detail_id",
                table: "request_detail_attachment",
                newName: "ix_request_detail_attachment_gift_request_detail_id");

            migrationBuilder.RenameColumn(
                name: "why_seizure",
                table: "gift_request_detail",
                newName: "why_gift");

            migrationBuilder.RenameColumn(
                name: "seizure_value",
                table: "gift_request_detail",
                newName: "gift_value");

            migrationBuilder.RenameColumn(
                name: "seizure_request_id",
                table: "gift_request_detail",
                newName: "gift_request_id");

            migrationBuilder.RenameIndex(
                name: "ix_seizure_request_detail_status_id",
                table: "gift_request_detail",
                newName: "ix_gift_request_detail_status_id");

            migrationBuilder.RenameIndex(
                name: "ix_seizure_request_detail_seizure_request_id",
                table: "gift_request_detail",
                newName: "ix_gift_request_detail_gift_request_id");

            migrationBuilder.RenameIndex(
                name: "ix_seizure_request_detail_item_id",
                table: "gift_request_detail",
                newName: "ix_gift_request_detail_item_id");

            migrationBuilder.RenameIndex(
                name: "IsDeletedSeizureRequestDetailIndex",
                table: "gift_request_detail",
                newName: "IsDeletedGiftRequestDetailIndex");

            migrationBuilder.RenameIndex(
                name: "ix_seizure_request_firm_id",
                table: "gift_request",
                newName: "ix_gift_request_firm_id");

            migrationBuilder.RenameIndex(
                name: "IsDeletedSeizureRequestIndex",
                table: "gift_request",
                newName: "IsDeletedGiftRequestIndex");

            migrationBuilder.AddPrimaryKey(
                name: "pk_gift_request_detail",
                table: "gift_request_detail",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_gift_request",
                table: "gift_request",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_gift_request_firm_firm_id",
                table: "gift_request",
                column: "firm_id",
                principalTable: "firm",
                principalColumn: "firm_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_request_detail_gift_request_gift_request_id",
                table: "gift_request_detail",
                column: "gift_request_id",
                principalTable: "gift_request",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_request_detail_item_item_id",
                table: "gift_request_detail",
                column: "item_id",
                principalTable: "item",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_request_detail_status_status_id",
                table: "gift_request_detail",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_detail_attachment_gift_request_detail_gift_request_",
                table: "request_detail_attachment",
                column: "gift_request_detail_id",
                principalTable: "gift_request_detail",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                UPDATE request_detail_attachment
                SET file_path = REPLACE(file_path, 'SEIZURES', 'GIFTS')
                WHERE POSITION('SEIZURES' IN file_path) > 0;
                """);

            migrationBuilder.Sql("""
                UPDATE page
                SET "to" = REPLACE("to", '/seizure-', '/gift-')
                WHERE "to" LIKE '/seizure-%'
                  AND "to" IN (
                    '/seizure-request',
                    '/seizure-sub-committee',
                    '/seizure-permanent-committee',
                    '/seizure-sales-entity',
                    '/seizure-finance-authority',
                    '/seizure-completed',
                    '/seizure-detail'
                  );
                """);
        }
    }
}
