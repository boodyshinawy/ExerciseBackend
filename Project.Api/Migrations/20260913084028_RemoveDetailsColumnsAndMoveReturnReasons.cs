using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDetailsColumnsAndMoveReturnReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "can_buy",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "delivery_date",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "given_geha",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "job_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "main_category",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "military_number",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "person_finance_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "person_geha_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "person_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "rank_finance_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "rank_geha_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "rank_name",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "seizure_value",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "sub_category",
                table: "seizure_request_detail");

            migrationBuilder.DropColumn(
                name: "why_seizure",
                table: "seizure_request_detail");

            migrationBuilder.AddColumn<string>(
                name: "return_reason_from_permanent_commite",
                table: "seizure_request",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "return_reason_from_sub_commite",
                table: "seizure_request",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "case_number",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "mahdr_number",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "return_reason_from_permanent_commite",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "return_reason_from_sub_commite",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "created_date",
                table: "firm");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "firm");

            migrationBuilder.AddColumn<bool>(
                name: "can_buy",
                table: "seizure_request_detail",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "delivery_date",
                table: "seizure_request_detail",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "given_geha",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "job_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "main_category",
                table: "seizure_request_detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "military_number",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "person_finance_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "person_geha_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "person_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "rank_finance_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rank_geha_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "rank_name",
                table: "seizure_request_detail",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "seizure_value",
                table: "seizure_request_detail",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_category",
                table: "seizure_request_detail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "why_seizure",
                table: "seizure_request_detail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "accept_items",
                table: "firm",
                type: "boolean",
                precision: 1,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "accept_persons",
                table: "firm",
                type: "boolean",
                precision: 1,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "army_code",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "commander_id",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "department_id",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "display_order",
                table: "firm",
                type: "smallint",
                precision: 5,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "division_id",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "electricity_counter_no",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "firm_cat_id",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "firm_level",
                table: "firm",
                type: "smallint",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "firm_like_code",
                table: "firm",
                type: "character varying(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "firm_seq",
                table: "firm",
                type: "smallint",
                precision: 5,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "firm_type_id",
                table: "firm",
                type: "integer",
                precision: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "in_tree_flag",
                table: "firm",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "military_station_id",
                table: "firm",
                type: "smallint",
                precision: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "org_code",
                table: "firm",
                type: "character varying(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "org_orders_no",
                table: "firm",
                type: "character varying(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "parent_firm_id",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_group",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "recrute_slice_id",
                table: "firm",
                type: "boolean",
                precision: 1,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "residence_place_id",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "road_id",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "second_commander_id",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "surgeries_per_day",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "surgery_teams_count",
                table: "firm",
                type: "smallint",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "taabea_code",
                table: "firm",
                type: "character varying(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "train_spec_code",
                table: "firm",
                type: "smallint",
                precision: 5,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "train_type_code",
                table: "firm",
                type: "smallint",
                precision: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_order",
                table: "firm",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "water_counter_no",
                table: "firm",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "within_region",
                table: "firm",
                type: "boolean",
                precision: 1,
                nullable: true);
        }
    }
}
