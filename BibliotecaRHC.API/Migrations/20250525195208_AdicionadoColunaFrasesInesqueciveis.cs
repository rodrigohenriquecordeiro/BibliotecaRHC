using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaRHC.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionadoColunaFrasesInesqueciveis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "FrasesInesqueciveis",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autor",
                table: "FrasesInesqueciveis");
        }
    }
}
