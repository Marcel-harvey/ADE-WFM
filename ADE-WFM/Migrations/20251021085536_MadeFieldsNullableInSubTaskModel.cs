using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class MadeFieldsNullableInSubTaskModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask");

            migrationBuilder.AlterColumn<int>(
                name: "WorkFlowId",
                table: "SubTask",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask");

            migrationBuilder.AlterColumn<int>(
                name: "WorkFlowId",
                table: "SubTask",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TodoId",
                table: "SubTask",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
