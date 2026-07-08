using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsService.DAL.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndEngagementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Category",
                schema: "newsService",
                table: "News",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                schema: "newsService",
                table: "News",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                schema: "newsService",
                table: "News",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReadTimeMinutes",
                schema: "newsService",
                table: "News",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_News_Category",
                schema: "newsService",
                table: "News",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_News_Category",
                schema: "newsService",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "newsService",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Dislikes",
                schema: "newsService",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Likes",
                schema: "newsService",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ReadTimeMinutes",
                schema: "newsService",
                table: "News");
        }
    }
}
