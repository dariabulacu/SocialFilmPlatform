using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialFilmPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class PivotTagsToDiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryMovieDiary");

            migrationBuilder.DropTable(
                name: "MovieDiaryTag");

            migrationBuilder.CreateTable(
                name: "CategoryDiary",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    DiariesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDiary", x => new { x.CategoriesId, x.DiariesId });
                    table.ForeignKey(
                        name: "FK_CategoryDiary_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDiary_Diaries_DiariesId",
                        column: x => x.DiariesId,
                        principalTable: "Diaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiaryTag",
                columns: table => new
                {
                    DiariesId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryTag", x => new { x.DiariesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DiaryTag_Diaries_DiariesId",
                        column: x => x.DiariesId,
                        principalTable: "Diaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiaryTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDiary_DiariesId",
                table: "CategoryDiary",
                column: "DiariesId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryTag_TagsId",
                table: "DiaryTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDiary");

            migrationBuilder.DropTable(
                name: "DiaryTag");

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
    }
}
