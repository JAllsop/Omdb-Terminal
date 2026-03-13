using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmdbTerminal.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedCachingV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Runtime",
                keyValue: null,
                column: "Runtime",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Runtime",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Released",
                keyValue: null,
                column: "Released",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Released",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Rated",
                keyValue: null,
                column: "Rated",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Rated",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "PosterUrl",
                keyValue: null,
                column: "PosterUrl",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PosterUrl",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Plot",
                keyValue: null,
                column: "Plot",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Plot",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "ImdbRating",
                keyValue: null,
                column: "ImdbRating",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImdbRating",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Genre",
                keyValue: null,
                column: "Genre",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Director",
                keyValue: null,
                column: "Director",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Actors",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Awards",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BoxOffice",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DVD",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImdbVotes",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsDetailed",
                table: "CachedMovies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Metascore",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Production",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Writer",
                table: "CachedMovies",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RatingsEntity",
                columns: table => new
                {
                    MovieId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingsEntity", x => new { x.MovieId, x.Source });
                    table.ForeignKey(
                        name: "FK_RatingsEntity_CachedMovies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "CachedMovies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SearchCache",
                columns: table => new
                {
                    Query = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Page = table.Column<int>(type: "int", nullable: false),
                    TotalResults = table.Column<int>(type: "int", nullable: false),
                    MovieIds = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CachedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchCache", x => new { x.Query, x.Page });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingsEntity");

            migrationBuilder.DropTable(
                name: "SearchCache");

            migrationBuilder.DropColumn(
                name: "Actors",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Awards",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "BoxOffice",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "DVD",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "ImdbVotes",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "IsDetailed",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Metascore",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Production",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "CachedMovies");

            migrationBuilder.DropColumn(
                name: "Writer",
                table: "CachedMovies");

            migrationBuilder.AlterColumn<string>(
                name: "Runtime",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Released",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Rated",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PosterUrl",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Plot",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ImdbRating",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
