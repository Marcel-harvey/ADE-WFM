using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_WorkFlows_CompanyId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_StickyNotes_WorkFlows_CompanyId",
                table: "StickyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_WorkFlows_CompanyId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CompanyId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_StickyNotes_CompanyId",
                table: "StickyNotes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "StickyNotes");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Projects",
                newName: "WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                newName: "IX_Projects_WorkFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "WorkFlowId",
                table: "Projects",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_WorkFlowId",
                table: "Projects",
                newName: "IX_Projects_CompanyId");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Todos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "StickyNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CompanyId",
                table: "Todos",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StickyNotes_CompanyId",
                table: "StickyNotes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_WorkFlows_CompanyId",
                table: "Projects",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StickyNotes_WorkFlows_CompanyId",
                table: "StickyNotes",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_WorkFlows_CompanyId",
                table: "Todos",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
