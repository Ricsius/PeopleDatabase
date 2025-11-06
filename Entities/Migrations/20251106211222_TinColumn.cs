using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TinColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationNumber",
                table: "People",
                type: "varchar(8)",
                nullable: true,
                defaultValue: "ABC12345");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TIN",
                table: "People",
                sql: "len([TaxIdentificationNumber]) = 8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_TIN",
                table: "People");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationNumber",
                table: "People");
        }
    }
}
