using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyMusicHeaven.Migrations.MyMusicHeavenNew
{
    public partial class addproductimageinsql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProductPicture",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPicture",
                table: "Product");
        }
    }
}
