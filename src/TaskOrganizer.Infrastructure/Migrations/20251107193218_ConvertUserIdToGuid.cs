using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOrganizer.Infrastructure.Context.Migrations
{
    /// <inheritdoc />
    public partial class ConvertUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers");

            // Drop composite primary keys that include UserId
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            // Drop existing indexes on foreign keys
            migrationBuilder.DropIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUsers");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers");

            // Drop UserId columns in dependent tables
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectUsers");

            // Drop and recreate primary key and Id column in Users table
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            // Add new Guid UserId columns
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TaskUsers",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TaskHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ProjectUsers",
                type: "uniqueidentifier",
                nullable: false);

            // Recreate composite primary keys
            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers",
                columns: new[] { "TaskId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                columns: new[] { "ProjectId", "UserId" });

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers",
                column: "UserId");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers");

            // Drop composite primary keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUsers");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers");

            // Drop UserId columns
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectUsers");

            // Drop and recreate Users.Id with IDENTITY
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            // Recreate int UserId columns
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskUsers",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ProjectUsers",
                type: "int",
                nullable: false);

            // Recreate composite primary keys
            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers",
                columns: new[] { "TaskId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                columns: new[] { "ProjectId", "UserId" });

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_UserId",
                table: "TaskHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                table: "ProjectUsers",
                column: "UserId");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Users_UserId",
                table: "TaskHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
