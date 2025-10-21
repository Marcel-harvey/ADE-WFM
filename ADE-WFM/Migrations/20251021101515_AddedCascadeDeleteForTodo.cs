using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeleteForTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

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
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTask_Todos_TodoId",
                table: "SubTask",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id");
        }
    }
}
