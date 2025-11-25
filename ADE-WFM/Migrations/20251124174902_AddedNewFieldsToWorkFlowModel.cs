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
                table: "Programs",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "userCreated",
                table: "Programs",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Programs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Programs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Programs",
                newName: "dateCreated");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Programs",
                newName: "userCreated");
        }
    }
}
