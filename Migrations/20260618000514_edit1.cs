using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisApi.Migrations
{
    /// <inheritdoc />
    public partial class edit1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotesAr",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "NotesEn",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "AgentsCount",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "CommissionRate",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "IsCanAccept",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "LastTemporaryPaymentAmount",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "LastTemporaryPaymentStatus",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "TemporaryPaymentNotes",
                table: "Centers");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Regions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Long",
                table: "Regions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Centers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Centers_ManagerId",
                table: "Centers",
                column: "ManagerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Centers_AspNetUsers_ManagerId",
                table: "Centers",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Centers_AspNetUsers_ManagerId",
                table: "Centers");

            migrationBuilder.DropIndex(
                name: "IX_Centers_ManagerId",
                table: "Centers");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Centers");

            migrationBuilder.AddColumn<string>(
                name: "NotesAr",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotesEn",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgentsCount",
                table: "Centers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionRate",
                table: "Centers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsCanAccept",
                table: "Centers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LastTemporaryPaymentAmount",
                table: "Centers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastTemporaryPaymentStatus",
                table: "Centers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryPaymentNotes",
                table: "Centers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
