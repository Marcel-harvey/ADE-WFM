using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class RenamedCompanyToWorkFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Companies_CompanyId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_StickyNotes_Companies_CompanyId",
                table: "StickyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Companies_CompanyId",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Companies_CompanyId",
                table: "Todos");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.CreateTable(
                name: "WorkFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_UserId",
                table: "WorkFlows",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_WorkFlows_CompanyId",
                table: "Comments",
                column: "CompanyId",
                principalTable: "WorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_SubTask_WorkFlows_CompanyId",
                table: "SubTask",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_WorkFlows_CompanyId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_WorkFlows_CompanyId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_StickyNotes_WorkFlows_CompanyId",
                table: "StickyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_WorkFlows_CompanyId",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_WorkFlows_CompanyId",
                table: "Todos");

            migrationBuilder.DropTable(
                name: "WorkFlows");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                table: "Companies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Companies_CompanyId",
                table: "Comments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StickyNotes_Companies_CompanyId",
                table: "StickyNotes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Companies_CompanyId",
                table: "SubTask",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Companies_CompanyId",
                table: "Todos",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
