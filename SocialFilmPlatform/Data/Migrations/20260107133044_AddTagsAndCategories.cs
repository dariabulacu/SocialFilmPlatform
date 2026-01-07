using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialFilmPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoryMovieDiary",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesMovieId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesDiaryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMovieDiary", x => new { x.CategoriesId, x.MovieDiariesId, x.MovieDiariesMovieId, x.MovieDiariesDiaryId });
                    table.ForeignKey(
                        name: "FK_CategoryMovieDiary_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryMovieDiary_MovieDiaries_MovieDiariesId_MovieDiariesM~",
                        columns: x => new { x.MovieDiariesId, x.MovieDiariesMovieId, x.MovieDiariesDiaryId },
                        principalTable: "MovieDiaries",
                        principalColumns: new[] { "Id", "MovieId", "DiaryId" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MovieDiaryTag",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesMovieId = table.Column<int>(type: "int", nullable: false),
                    MovieDiariesDiaryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieDiaryTag", x => new { x.TagsId, x.MovieDiariesId, x.MovieDiariesMovieId, x.MovieDiariesDiaryId });
                    table.ForeignKey(
                        name: "FK_MovieDiaryTag_MovieDiaries_MovieDiariesId_MovieDiariesMovieI~",
                        columns: x => new { x.MovieDiariesId, x.MovieDiariesMovieId, x.MovieDiariesDiaryId },
                        principalTable: "MovieDiaries",
                        principalColumns: new[] { "Id", "MovieId", "DiaryId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieDiaryTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryMovieDiary_MovieDiariesId_MovieDiariesMovieId_MovieD~",
                table: "CategoryMovieDiary",
                columns: new[] { "MovieDiariesId", "MovieDiariesMovieId", "MovieDiariesDiaryId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieDiaryTag_MovieDiariesId_MovieDiariesMovieId_MovieDiarie~",
                table: "MovieDiaryTag",
                columns: new[] { "MovieDiariesId", "MovieDiariesMovieId", "MovieDiariesDiaryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryMovieDiary");

            migrationBuilder.DropTable(
                name: "MovieDiaryTag");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
