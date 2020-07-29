using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Migrations
{
    public partial class rewriteAva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "MyAvatar",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyAvatar",
                table: "AspNetUsers");
        }
    }
}
