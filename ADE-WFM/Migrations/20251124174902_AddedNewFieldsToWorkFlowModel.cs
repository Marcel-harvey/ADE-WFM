using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFieldsToWorkFlowModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dateCreated",
                table: "WorkFlows",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "userCreated",
                table: "WorkFlows",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "WorkFlows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "WorkFlows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "WorkFlows");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "WorkFlows",
                newName: "dateCreated");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkFlows",
                newName: "userCreated");
        }
    }
}
