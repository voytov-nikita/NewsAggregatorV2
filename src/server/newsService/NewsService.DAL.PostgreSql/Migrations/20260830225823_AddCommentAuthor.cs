using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsService.DAL.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                schema: "newsService",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                schema: "newsService",
                table: "Comments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            // Comments written before authorship existed keep Guid.Empty, but an empty name would
            // render as a blank byline - label them instead.
            migrationBuilder.Sql(
                """UPDATE "newsService"."Comments" SET "AuthorName" = 'Deleted user' WHERE "AuthorName" = '';""");

            // The defaults exist only to backfill those rows. Leaving them in place would let a
            // future insert silently produce an authorless comment.
            migrationBuilder.Sql(
                """ALTER TABLE "newsService"."Comments" ALTER COLUMN "AuthorId" DROP DEFAULT;""");
            migrationBuilder.Sql(
                """ALTER TABLE "newsService"."Comments" ALTER COLUMN "AuthorName" DROP DEFAULT;""");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                schema: "newsService",
                table: "Comments",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId",
                schema: "newsService",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                schema: "newsService",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                schema: "newsService",
                table: "Comments");
        }
    }
}
