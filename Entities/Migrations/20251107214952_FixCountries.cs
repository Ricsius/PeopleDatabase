using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class FixCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "Name",
                value: "England");

            migrationBuilder.CreateIndex(
                name: "IX_People_CountryId",
                table: "People",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Countries_CountryId",
                table: "People",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Countries_CountryId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_CountryId",
                table: "People");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "Name",
                value: "China");
        }
    }
}
