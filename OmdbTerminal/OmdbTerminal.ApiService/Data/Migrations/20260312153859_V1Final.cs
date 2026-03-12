using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmdbTerminal.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class V1Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImdbId",
                table: "CachedMovies",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImdbRating",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "CachedMovies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Rated",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Released",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Runtime",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "ImdbRating",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Rated",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Released",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "CachedMovies");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CachedMovies",
                newName: "ImdbId");
        }
    }
}
