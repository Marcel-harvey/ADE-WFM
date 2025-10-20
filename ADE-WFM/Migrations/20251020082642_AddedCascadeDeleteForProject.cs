using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeleteForProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskPlanning_Projects_ProjectId",
                table: "TaskPlanning");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "TaskPlanning",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TaskPlanning",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPlanning_UserId",
                table: "TaskPlanning",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskPlanning_AspNetUsers_UserId",
                table: "TaskPlanning",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskPlanning_Projects_ProjectId",
                table: "TaskPlanning",
                column: "ProjectId",
                principalTable: "Projects",
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
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskPlanning_AspNetUsers_UserId",
                table: "TaskPlanning");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskPlanning_Projects_ProjectId",
                table: "TaskPlanning");

            migrationBuilder.DropIndex(
                name: "IX_TaskPlanning_UserId",
                table: "TaskPlanning");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskPlanning");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "TaskPlanning",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_WorkFlows_WorkFlowId",
                table: "Projects",
                column: "WorkFlowId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskPlanning_Projects_ProjectId",
                table: "TaskPlanning",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
