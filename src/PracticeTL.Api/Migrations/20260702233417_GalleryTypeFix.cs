using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeTL.Api.Migrations
{
    /// <inheritdoc />
    public partial class GalleryTypeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "GalleryItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "GalleryItems");
        }
    }
}
