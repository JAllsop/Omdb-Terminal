using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmdbTerminal.ApiService.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSearchWithMediaTypeAndYearV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchCache",
                table: "SearchCache");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SearchCache",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SearchCache",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "SearchCache",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CachedMovies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchCache",
                table: "SearchCache",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SearchCache_Query_Page_Type_Year",
                table: "SearchCache",
                columns: new[] { "Query", "Page", "Type", "Year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchCache",
                table: "SearchCache");

            migrationBuilder.DropIndex(
                name: "IX_SearchCache_Query_Page_Type_Year",
                table: "SearchCache");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SearchCache");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SearchCache");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SearchCache");

            migrationBuilder.UpdateData(
                table: "CachedMovies",
                keyColumn: "Type",
                keyValue: null,
                column: "Type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CachedMovies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchCache",
                table: "SearchCache",
                columns: new[] { "Query", "Page" });
        }
    }
}
