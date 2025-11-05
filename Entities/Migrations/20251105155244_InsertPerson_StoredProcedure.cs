using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = "CREATE PROCEDURE [dbo].[InsertPerson]"
                + " (@Id uniqueidentifier, @Name nvarchar(40), @Email nvarchar(40), @DateOfBirth datetime2(7), @Gender nvarchar(10), @CountryId uniqueidentifier, @Address nvarchar(200), @ReceiveNewsLetters bit)"
                + " AS BEGIN"
                + " INSERT INTO [dbo].[People](Id, Name, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsLetters)"
                + " VALUES (@Id, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters)"
                + " END";

            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = "DROP PROCEDURE [dbo].[InsertPerson]";

            migrationBuilder.Sql(procedure);
        }
    }
}
