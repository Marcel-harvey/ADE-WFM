using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class InitWorkFlowUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlows_AspNetUsers_UserId",
                table: "WorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlows_UserId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkFlows");

            migrationBuilder.CreateTable(
                name: "WorkFlowUser",
                columns: table => new
                {
                    WorkFlowId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowUser", x => new { x.WorkFlowId, x.UserId });
                    table.ForeignKey(
                        name: "FK_WorkFlowUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkFlowUser_WorkFlows_WorkFlowId",
                        column: x => x.WorkFlowId,
                        principalTable: "WorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowUser_UserId",
                table: "WorkFlowUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlowUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WorkFlows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_UserId",
                table: "WorkFlows",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlows_AspNetUsers_UserId",
                table: "WorkFlows",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
