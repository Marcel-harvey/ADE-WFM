using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedTenantEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "WorkFlows",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Todos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TaskPlanning",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SubTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "StickyNotes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Projects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: true),
                    ConnectionString = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_TenantId",
                table: "WorkFlows",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TenantId",
                table: "Todos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPlanning_TenantId",
                table: "TaskPlanning",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_TenantId",
                table: "SubTasks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StickyNotes_TenantId",
                table: "StickyNotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TenantId",
                table: "Projects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TenantId",
                table: "Comments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tenants_TenantId",
                table: "Comments",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Tenants_TenantId",
                table: "Projects",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StickyNotes_Tenants_TenantId",
                table: "StickyNotes",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Tenants_TenantId",
                table: "SubTasks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskPlanning_Tenants_TenantId",
                table: "TaskPlanning",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Tenants_TenantId",
                table: "Todos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlows_Tenants_TenantId",
                table: "WorkFlows",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tenants_TenantId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Tenants_TenantId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_StickyNotes_Tenants_TenantId",
                table: "StickyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_Tenants_TenantId",
                table: "SubTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskPlanning_Tenants_TenantId",
                table: "TaskPlanning");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Tenants_TenantId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlows_Tenants_TenantId",
                table: "WorkFlows");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlows_TenantId",
                table: "WorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_Todos_TenantId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_TaskPlanning_TenantId",
                table: "TaskPlanning");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_TenantId",
                table: "SubTasks");

            migrationBuilder.DropIndex(
                name: "IX_StickyNotes_TenantId",
                table: "StickyNotes");

            migrationBuilder.DropIndex(
                name: "IX_Projects_TenantId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TenantId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TaskPlanning");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StickyNotes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");
        }
    }
}
