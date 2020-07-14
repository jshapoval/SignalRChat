using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Migrations
{
    public partial class new1407 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ChatModels_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatModels_ChatModelId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatModelId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatModelId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatModelId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatModelId1",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatModelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatModelId1",
                table: "Messages",
                column: "ChatModelId1");

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
                name: "FK_Messages_ChatModels_ChatModelId1",
                table: "Messages",
                column: "ChatModelId1",
                principalTable: "ChatModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
