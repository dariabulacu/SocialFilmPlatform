using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialFilmPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergedColleagueChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Diaries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Diaries",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Diaries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Diaries",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Diaries_UserId",
                table: "Diaries",
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

            migrationBuilder.DropIndex(
                name: "IX_Diaries_UserId",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Diaries");
        }
    }
}
