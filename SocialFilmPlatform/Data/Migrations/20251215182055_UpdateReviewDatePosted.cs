using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialFilmPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewDatePosted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GendreId",
                table: "Movies");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePosted",
                table: "Reviews",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DatePosted",
                table: "Reviews",
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
        }
    }
}
