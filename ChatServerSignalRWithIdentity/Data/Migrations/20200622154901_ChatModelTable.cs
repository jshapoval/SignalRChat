using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Data.Migrations
{
    public partial class ChatModelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatModelId",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Avatars",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatModelId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatModelId",
                table: "Messages",
                column: "ChatModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatModelId",
                table: "AspNetUsers",
                column: "ChatModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ChatModels_ChatModelId",
                table: "AspNetUsers",
                column: "ChatModelId",
                principalTable: "ChatModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatModels_ChatModelId",
                table: "Messages",
                column: "ChatModelId",
                principalTable: "ChatModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ChatModels_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatModels_ChatModelId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "ChatModels");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatModelId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatModelId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Avatars");

            migrationBuilder.DropColumn(
                name: "ChatModelId",
                table: "AspNetUsers");
        }
    }
}
