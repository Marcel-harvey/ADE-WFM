using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class RenamedSubtaskFieldsToWorkFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_WorkFlows_CompanyId",
                table: "SubTask");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "SubTask",
                newName: "WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_CompanyId",
                table: "SubTask",
                newName: "IX_SubTask_WorkFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_WorkFlows_WorkFlowId",
                table: "SubTask");

            migrationBuilder.RenameColumn(
                name: "WorkFlowId",
                table: "SubTask",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_WorkFlowId",
                table: "SubTask",
                newName: "IX_SubTask_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_WorkFlows_CompanyId",
                table: "SubTask",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
