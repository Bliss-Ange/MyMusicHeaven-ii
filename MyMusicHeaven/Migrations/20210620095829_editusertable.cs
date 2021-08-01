using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyMusicHeaven.Migrations
{
    public partial class editusertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "User_Address",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User_Age",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "User_DOB",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "User_Full_Name",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_DOB",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_Full_Name",
                table: "AspNetUsers");
        }
    }
}
