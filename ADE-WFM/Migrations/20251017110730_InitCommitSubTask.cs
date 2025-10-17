using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class InitCommitSubTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

            migrationBuilder.AlterColumn<int>(
                name: "TodoId",
                table: "SubTask",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "SubTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Desciption",
                table: "SubTask",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "SubTask",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_CompanyId",
                table: "SubTask",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Companies_CompanyId",
                table: "SubTask",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Companies_CompanyId",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

            migrationBuilder.DropIndex(
                name: "IX_SubTask_CompanyId",
                table: "SubTask");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SubTask");

            migrationBuilder.DropColumn(
                name: "Desciption",
                table: "SubTask");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "SubTask");

            migrationBuilder.AlterColumn<int>(
                name: "TodoId",
                table: "SubTask",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id");
        }
    }
}
