using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeSessionFieldsToSeizureRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sub_committee_number",
                table: "seizure_request",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "sub_committee_date",
                table: "seizure_request",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "permanent_committee_number",
                table: "seizure_request",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "permanent_committee_date",
                table: "seizure_request",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sub_committee_number",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "sub_committee_date",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "permanent_committee_number",
                table: "seizure_request");

            migrationBuilder.DropColumn(
                name: "permanent_committee_date",
                table: "seizure_request");
        }
    }
}
