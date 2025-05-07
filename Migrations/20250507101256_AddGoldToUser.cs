using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace numberFightMayis.Migrations
{
    /// <inheritdoc />
    public partial class AddGoldToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gold",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gold",
                table: "AspNetUsers");
        }
    }
}
