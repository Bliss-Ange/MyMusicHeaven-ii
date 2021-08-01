using Microsoft.EntityFrameworkCore.Migrations;

namespace MyMusicHeaven.Migrations.MyMusicHeavenNew
{
    public partial class addratingcolumninproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Product");
        }
    }
}
