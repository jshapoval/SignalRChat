﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerSignalRWithIdentity.Migrations
{
    public partial class _1707 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastMessageId",
                table: "Dialogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Dialogs");
        }
    }
}
