using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsService.DAL.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReadDate",
                schema: "newsService",
                table: "News",
                newName: "CreateDate");

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                schema: "newsService",
                table: "News",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_News_Publisher_Guid",
                schema: "newsService",
                table: "News",
                columns: new[] { "Publisher", "Guid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_News_Publisher_Guid",
                schema: "newsService",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Guid",
                schema: "newsService",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                schema: "newsService",
                table: "News",
                newName: "ReadDate");
        }
    }
}
