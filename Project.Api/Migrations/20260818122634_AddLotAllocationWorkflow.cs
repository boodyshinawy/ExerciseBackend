using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLotAllocationWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "remaining_quantity",
                table: "seizure_request_detail",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE seizure_request_detail SET remaining_quantity = quantity;");

            migrationBuilder.Sql(@"
                INSERT INTO status (id, name, is_deleted, time_stamp)
                OVERRIDING SYSTEM VALUE
                SELECT v.id, v.name, false, NOW()
                FROM (VALUES
                    (7, 'معتمدة من اللجنة الدائمة'),
                    (8, 'مخصص جزئياً'),
                    (9, 'مخصص بالكامل')
                ) AS v(id, name)
                WHERE NOT EXISTS (SELECT 1 FROM status s WHERE s.id = v.id);

                SELECT setval(
                    pg_get_serial_sequence('status', 'id'),
                    COALESCE((SELECT MAX(id) FROM status), 1));
            ");

            migrationBuilder.AddColumn<string>(
                name: "sub_committee_opinion",
                table: "seizure_request",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "technical_report",
                table: "seizure_request",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sale_entity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<int>(type: "integer", nullable: false),
                    seizure_request_id = table.Column<int>(type: "integer", nullable: true),
                    from_status_id = table.Column<int>(type: "integer", nullable: false),
                    to_status_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    user_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<int>(type: "integer", precision: 2, nullable: false),
                    new_state = table.Column<int>(type: "integer", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_status_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sale_lot",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sale_entity_id = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    total_amount_collected = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    deposit_receipt_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    deposit_receipt_is_pdf = table.Column<bool>(type: "boolean", nullable: false),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<int>(type: "integer", precision: 2, nullable: false),
                    new_state = table.Column<int>(type: "integer", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale_lot", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_lot_sale_entity_sale_entity_id",
                        column: x => x.sale_entity_id,
                        principalTable: "sale_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lot_item_allocation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sale_lot_id = table.Column<int>(type: "integer", nullable: false),
                    seizure_request_detail_id = table.Column<int>(type: "integer", nullable: false),
                    allocated_quantity = table.Column<int>(type: "integer", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<int>(type: "integer", precision: 2, nullable: false),
                    new_state = table.Column<int>(type: "integer", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lot_item_allocation", x => x.id);
                    table.CheckConstraint("ck_lot_item_allocation_qty", "allocated_quantity > 0");
                    table.ForeignKey(
                        name: "fk_lot_item_allocation_sale_lot_sale_lot_id",
                        column: x => x.sale_lot_id,
                        principalTable: "sale_lot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lot_item_allocation_seizure_request_detail_seizure_request_",
                        column: x => x.seizure_request_detail_id,
                        principalTable: "seizure_request_detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_seizure_detail_remaining_qty",
                table: "seizure_request_detail",
                sql: "remaining_quantity >= 0 AND remaining_quantity <= quantity");

            migrationBuilder.CreateIndex(
                name: "IsDeletedLotItemAllocationIndex",
                table: "lot_item_allocation",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_lot_item_allocation_sale_lot_id_seizure_request_detail_id",
                table: "lot_item_allocation",
                columns: new[] { "sale_lot_id", "seizure_request_detail_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lot_item_allocation_seizure_request_detail_id",
                table: "lot_item_allocation",
                column: "seizure_request_detail_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedSaleEntityIndex",
                table: "sale_entity",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IsDeletedSaleLotIndex",
                table: "sale_lot",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_sale_lot_sale_entity_id",
                table: "sale_lot",
                column: "sale_entity_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedStatusHistoryIndex",
                table: "status_history",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_status_history_entity_type_entity_id",
                table: "status_history",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_status_history_seizure_request_id",
                table: "status_history",
                column: "seizure_request_id");

            migrationBuilder.Sql(@"
                INSERT INTO sale_entity (name, is_deleted, time_stamp)
                SELECT v.name, false, NOW()
                FROM (VALUES
                    ('جهاز الخدمات العامة'),
                    ('المزاد العلني'),
                    ('لجنة البيع المباشر')
                ) AS v(name)
                WHERE NOT EXISTS (SELECT 1 FROM sale_entity s WHERE s.name = v.name);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lot_item_allocation");

            migrationBuilder.DropTable(
                name: "status_history");

            migrationBuilder.DropTable(
                name: "sale_lot");

            migrationBuilder.DropTable(
                name: "sale_entity");

            migrationBuilder.DropCheckConstraint(
                name: "ck_seizure_detail_remaining_qty",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "remaining_quantity",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "sub_committee_opinion",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "technical_report",
                table: "seizure_request");
        }
    }
}
