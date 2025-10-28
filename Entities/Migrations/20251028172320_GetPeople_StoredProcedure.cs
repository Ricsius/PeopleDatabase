using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPeople_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = "CREATE PROCEDURE [dbo].[GetAllPeople]"
                + " AS BEGIN"
                + " SELECT * FROM [dbo].[People]"
                + " END";

            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = "DROP PROCEDURE [dbo].[GetAllPeople]";

            migrationBuilder.Sql(procedure);
        }
    }
}
