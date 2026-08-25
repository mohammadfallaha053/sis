using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisApi.Migrations
{
    /// <inheritdoc />
    public partial class EditItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "SpecialistAr",
                table: "ItemType");

            migrationBuilder.RenameColumn(
                name: "SpecialistEn",
                table: "ItemType",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "PointsPerKg",
                table: "ItemType",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsPerKg",
                table: "ItemType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ItemType",
                newName: "SpecialistEn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ItemType",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "ItemType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "ItemType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "ItemType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ItemType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecialistAr",
                table: "ItemType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
