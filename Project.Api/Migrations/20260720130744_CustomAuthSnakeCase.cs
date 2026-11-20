using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class CustomAuthSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "firm",
                columns: table => new
                {
                    firm_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    parent_firm_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    comander_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    second_commander_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    department_id = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    firm_type_id = table.Column<int>(type: "integer", precision: 10, nullable: true),
                    firm_cat_id = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    residence_place_id = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    train_spec_code = table.Column<short>(type: "smallint", precision: 5, nullable: true),
                    train_type_code = table.Column<byte>(type: "smallint", precision: 2, nullable: true),
                    army_code = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    name = table.Column<string>(type: "character varying(60)", unicode: false, maxLength: 60, nullable: false),
                    firm_level = table.Column<byte>(type: "smallint", precision: 2, nullable: true),
                    postal_group = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    org_orders_no = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    org_code = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    display_order = table.Column<short>(type: "smallint", precision: 5, nullable: true),
                    taabea_code = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: true),
                    within_region = table.Column<bool>(type: "boolean", precision: 1, nullable: true),
                    firm_seq = table.Column<short>(type: "smallint", precision: 5, nullable: true),
                    firm_like_code = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    electricity_counter_no = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    water_counter_no = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    road_id = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    surgeries_per_day = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    surgery_teams_count = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    military_station_id = table.Column<short>(type: "smallint", precision: 5, nullable: true),
                    accept_items = table.Column<bool>(type: "boolean", precision: 1, nullable: true),
                    accept_persons = table.Column<bool>(type: "boolean", precision: 1, nullable: true),
                    recrute_slice_id = table.Column<bool>(type: "boolean", precision: 1, nullable: true),
                    division_id = table.Column<byte>(type: "smallint", precision: 3, nullable: true),
                    in_tree_flag = table.Column<decimal>(type: "numeric", nullable: true),
                    unit_order = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_firm", x => x.firm_code);
                });

            migrationBuilder.CreateTable(
                name: "item_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "success"),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "page",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    to = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    sort_number = table.Column<int>(type: "integer", precision: 3, nullable: false),
                    is_searchable = table.Column<bool>(type: "boolean", nullable: false),
                    is_navigable = table.Column<bool>(type: "boolean", nullable: false),
                    is_sidebar_item = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_page", x => x.id);
                    table.ForeignKey(
                        name: "FK_PageChildPage",
                        column: x => x.parent_id,
                        principalTable: "page",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "rank",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rank", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_member = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_session",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    session_id = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gift_request",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firm_id = table.Column<string>(type: "character varying(20)", nullable: false),
                    firm_name = table.Column<string>(type: "text", nullable: false),
                    request_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("pk_gift_request", x => x.id);
                    table.ForeignKey(
                        name: "fk_gift_request_firm_firm_id",
                        column: x => x.firm_id,
                        principalTable: "firm",
                        principalColumn: "firm_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    rank_name = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    firm_id = table.Column<string>(type: "character varying(20)", nullable: false),
                    firm_name = table.Column<string>(type: "text", nullable: true),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_firm_firm_temp_id",
                        column: x => x.firm_id,
                        principalTable: "firm",
                        principalColumn: "firm_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    item_category_id = table.Column<int>(type: "integer", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_item_category_item_category_id",
                        column: x => x.item_category_id,
                        principalTable: "item_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "page_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    page_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<string>(type: "character varying(450)", nullable: false),
                    create_action = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    read_action = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    update_action = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    delete_action = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    application_role_id = table.Column<string>(type: "character varying(450)", nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_page_role", x => x.id);
                    table.ForeignKey(
                        name: "FK_PagePageRole",
                        column: x => x.page_id,
                        principalTable: "page",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageRoleRole",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_page_role_roles_application_role_id",
                        column: x => x.application_role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(450)", nullable: false),
                    role_id = table.Column<string>(type: "character varying(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gift_request_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gift_request_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    rank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    military_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    person_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    job_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    given_geha = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    why_gift = table.Column<string>(type: "text", nullable: true),
                    can_buy = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    gift_value = table.Column<float>(type: "real", nullable: true),
                    rank_geha_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    person_geha_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rank_finance_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    person_finance_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("pk_gift_request_detail", x => x.id);
                    table.ForeignKey(
                        name: "fk_gift_request_detail_gift_request_gift_request_id",
                        column: x => x.gift_request_id,
                        principalTable: "gift_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_gift_request_detail_item_item_id",
                        column: x => x.item_id,
                        principalTable: "item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_gift_request_detail_status_status_id",
                        column: x => x.status_id,
                        principalTable: "status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "request_detail_attachment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gift_request_detail_id = table.Column<int>(type: "integer", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: true),
                    is_pdf = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_request_detail_attachment", x => x.id);
                    table.ForeignKey(
                        name: "fk_request_detail_attachment_gift_request_detail_gift_request_",
                        column: x => x.gift_request_detail_id,
                        principalTable: "gift_request_detail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IsDeletedGiftRequestIndex",
                table: "gift_request",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_gift_request_firm_id",
                table: "gift_request",
                column: "firm_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedGiftRequestDetailIndex",
                table: "gift_request_detail",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_gift_request_detail_gift_request_id",
                table: "gift_request_detail",
                column: "gift_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_gift_request_detail_item_id",
                table: "gift_request_detail",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_gift_request_detail_status_id",
                table: "gift_request_detail",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedItemIndex",
                table: "item",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_item_item_category_id",
                table: "item",
                column: "item_category_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedItemCategoryIndex",
                table: "item_category",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IsDeletedPageIndex",
                table: "page",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_page_parent_id",
                table: "page",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "PageSortNumberIndex",
                table: "page",
                column: "sort_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IsDeletedPageRoleIndex",
                table: "page_role",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_page_role_application_role_id",
                table: "page_role",
                column: "application_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_page_role_page_id",
                table: "page_role",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "RolePageIndex",
                table: "page_role",
                columns: new[] { "role_id", "page_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IsDeletedRankIndex",
                table: "rank",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IsDeletedRequestDetailAttachmentIndex",
                table: "request_detail_attachment",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_request_detail_attachment_gift_request_detail_id",
                table: "request_detail_attachment",
                column: "gift_request_detail_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_is_deleted",
                table: "roles",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IsDeletedStatusIndex",
                table: "status",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IsDeletedUserSessionIndex",
                table: "user_session",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_users_firm_id",
                table: "users",
                column: "firm_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_deleted",
                table: "users",
                column: "is_deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "page_role");

            migrationBuilder.DropTable(
                name: "rank");

            migrationBuilder.DropTable(
                name: "request_detail_attachment");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_session");

            migrationBuilder.DropTable(
                name: "page");

            migrationBuilder.DropTable(
                name: "gift_request_detail");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "gift_request");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "status");

            migrationBuilder.DropTable(
                name: "firm");

            migrationBuilder.DropTable(
                name: "item_category");
        }
    }
}
