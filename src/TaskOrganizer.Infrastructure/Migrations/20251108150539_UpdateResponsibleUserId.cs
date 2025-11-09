using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOrganizer.Infrastructure.Context.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResponsibleUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskHistories");
            // Replace int ResponsibleUserId with a new uniqueidentifier column.
            // Direct ALTER failed due to existing int data; safest is drop + add (data would be inconsistent with new Users anyway).
            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert ResponsibleUserId back to int
            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleUserId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
