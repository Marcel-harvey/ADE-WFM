using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCommentAndCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_WorkFlows_CompanyId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUser_AspNetUsers_UserId",
                table: "WorkFlowUser");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUser_WorkFlows_WorkFlowId",
                table: "WorkFlowUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkFlowUser",
                table: "WorkFlowUser");

            migrationBuilder.RenameTable(
                name: "WorkFlowUser",
                newName: "WorkFlowUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Comments",
                newName: "WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CompanyId",
                table: "Comments",
                newName: "IX_Comments_WorkFlowId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkFlowUser_UserId",
                table: "WorkFlowUsers",
                newName: "IX_WorkFlowUsers_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Comments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkFlowUsers",
                table: "WorkFlowUsers",
                columns: new[] { "WorkFlowId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_WorkFlows_WorkFlowId",
                table: "Comments",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUsers_AspNetUsers_UserId",
                table: "WorkFlowUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUsers_WorkFlows_WorkFlowId",
                table: "WorkFlowUsers",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_WorkFlows_WorkFlowId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUsers_AspNetUsers_UserId",
                table: "WorkFlowUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUsers_WorkFlows_WorkFlowId",
                table: "WorkFlowUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkFlowUsers",
                table: "WorkFlowUsers");

            migrationBuilder.RenameTable(
                name: "WorkFlowUsers",
                newName: "WorkFlowUser");

            migrationBuilder.RenameColumn(
                name: "WorkFlowId",
                table: "Comments",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_WorkFlowId",
                table: "Comments",
                newName: "IX_Comments_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkFlowUsers_UserId",
                table: "WorkFlowUser",
                newName: "IX_WorkFlowUser_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkFlowUser",
                table: "WorkFlowUser",
                columns: new[] { "WorkFlowId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_WorkFlows_CompanyId",
                table: "Comments",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUser_AspNetUsers_UserId",
                table: "WorkFlowUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUser_WorkFlows_WorkFlowId",
                table: "WorkFlowUser",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
