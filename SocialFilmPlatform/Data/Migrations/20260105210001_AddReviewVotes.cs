using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialFilmPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diaries_AspNetUsers_ApplicationUserId",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "GendreId",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Diaries",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Diaries_ApplicationUserId",
                table: "Diaries",
                newName: "IX_Diaries_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Diaries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ReviewVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLike = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewVotes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId_UserId",
                table: "ReviewVotes",
                columns: new[] { "ReviewId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_UserId",
                table: "ReviewVotes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diaries_AspNetUsers_UserId",
                table: "Diaries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diaries_AspNetUsers_UserId",
                table: "Diaries");

            migrationBuilder.DropTable(
                name: "ReviewVotes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Diaries");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Diaries",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Diaries_UserId",
                table: "Diaries",
                newName: "IX_Diaries_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ReleaseDate",
                table: "Movies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "GendreId",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Diaries_AspNetUsers_ApplicationUserId",
                table: "Diaries",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
