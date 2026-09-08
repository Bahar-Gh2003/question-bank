using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_LevelId_To_ExamAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                table: "ExamAttempts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StudentAnswersJson",
                table: "ExamAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_LevelId",
                table: "ExamAttempts",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamAttempts_Levels_LevelId",
                table: "ExamAttempts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamAttempts_Levels_LevelId",
                table: "ExamAttempts");

            migrationBuilder.DropIndex(
                name: "IX_ExamAttempts_LevelId",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "StudentAnswersJson",
                table: "ExamAttempts");
        }
    }
}
