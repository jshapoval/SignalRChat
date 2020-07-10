using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Migrations
{
    public partial class restart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatModelId1",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallerId",
                table: "ChatModels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatModelId1",
                table: "Messages",
                column: "ChatModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatModels_ChatModelId1",
                table: "Messages",
                column: "ChatModelId1",
                principalTable: "ChatModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatModels_ChatModelId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatModelId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatModelId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CallerId",
                table: "ChatModels");
        }
    }
}
