using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubTaskToContext : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask");

            migrationBuilder.RenameTable(
                name: "SubTask",
                newName: "SubTasks");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_WorkFlowId",
                table: "SubTasks",
                newName: "IX_SubTasks_WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_TodoId",
                table: "SubTasks",
                newName: "IX_SubTasks_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubTasks",
                table: "SubTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Todos_TodoId",
                table: "SubTasks",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_WorkFlows_WorkFlowId",
                table: "SubTasks",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_Todos_TodoId",
                table: "SubTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_WorkFlows_WorkFlowId",
                table: "SubTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubTasks",
                table: "SubTasks");

            migrationBuilder.RenameTable(
                name: "SubTasks",
                newName: "SubTask");

            migrationBuilder.RenameIndex(
                name: "IX_SubTasks_WorkFlowId",
                table: "SubTask",
                newName: "IX_SubTask_WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_SubTasks_TodoId",
                table: "SubTask",
                newName: "IX_SubTask_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask",
                column: "Id");

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
                principalColumn: "Id");
        }
    }
}
