using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCancellationInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancellationType",
                table: "OrderItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CancellationType",
                table: "OrderItems");
        }
    }
}
