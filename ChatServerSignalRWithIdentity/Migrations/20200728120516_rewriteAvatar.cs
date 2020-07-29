using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Migrations
{
    public partial class rewriteAvatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesForAvatar");

            migrationBuilder.DropColumn(
                name: "UserAvatar",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Avatars",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Avatars",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Avatars",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Avatars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Avatars");

            migrationBuilder.AddColumn<byte[]>(
                name: "UserAvatar",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilesForAvatar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesForAvatar", x => x.Id);
                });
        }
    }
}
