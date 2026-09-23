using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisMatchmaker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Groups_GroupId",
                table: "Sessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Groups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Groups_GroupId",
                table: "Sessions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Groups_GroupId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Groups");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Groups_GroupId",
                table: "Sessions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
