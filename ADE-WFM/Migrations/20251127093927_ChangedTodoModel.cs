using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTodoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Todos",
                newName: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Todos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Todos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DueDate",
                table: "Todos",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateCreated",
                table: "Todos",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "Task",
                table: "Todos",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Todos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Todos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Todos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Todos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Todos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_UserId",
                table: "Todos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
