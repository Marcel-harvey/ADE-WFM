using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADE_WFM.Migrations
{
    /// <inheritdoc />
    public partial class AddedTitleToStickyNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "StickyNotes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "StickyNotes");
        }
    }
}
